using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using KoreKrush;

//TODO: sistema de pause y play
public class LevelManager : MonoBehaviour {

	[HideInInspector]
	public Level current_level;

	[HideInInspector]
	public Ship current_ship;

	public PathAgent instanciated_ship;

	public float instantiate_event_distance;

	public GameObject meteor_prefab;

	#region level control variables
	float count_down;
	float last_count;

	int left_movement;
	float turn_duration;
	List<LevelEvent>  events;

		#region collision variables
		public static bool collision;
		float time_start_collision;
		ObstacleManager obstacle;
		#endregion

	public static float distance_to_beat;

	PieceList objectives;
	bool distance_beated;

	#endregion

	IEnumerator<LevelEvent> eventsEnumerator;
	LevelEvent actualEvent;
	bool made;
	bool lastMoveNext;

	float warp_start;

	bool warp;


	void Awake()
	{
		#region esto de aqui se cargará de un json o algo asi
		var obj = new PieceList();
		obj.Add (Piece.blue, 150);
		obj.Add (Piece.green, 200);

		var eve = new List<LevelEvent>(){
			new RewardEvent(){
				Obj = new Obstacle(){ prefab = meteor_prefab, Speed = 300, GearToBreak = 1, SpeedDamageWhenBreak = 100},
				Rewards = new List<PieceReward>(){ new PieceReward(Piece.blue, 6), new PieceReward(Piece.green, 7)}, 
				MinRewardTime = 5, 
				MaxRewardTime = 10, 
				PathPosition = 0.5f},

			new RewardEvent(){
				Obj = new Obstacle(){ prefab = meteor_prefab, Speed = 1300, GearToBreak = 3, SpeedDamageWhenBreak = 1000},
				Rewards = new List<PieceReward>(){ new PieceReward(Piece.blue, 6), new PieceReward(Piece.green, 7)}, 
				MinRewardTime = 5, 
				MaxRewardTime = 10, 
				PathPosition = 1},
		};

		current_level = new Level { Objectives = obj, Turns = 300, Turn_time = 2, Distance = 10000, EventManager = eve };

		current_ship = new BasicShip();

		var x = instanciated_ship.gameObject.GetComponent<ShipManager>();
		x.GearsBox = current_ship.GearsBox;
		x.MinSpeed = current_ship.MinSpeed;
		x.WarpDuration = current_ship.WarpDuration;
		x.WarpBreakDamage = current_ship.WarpBreakDamage;
		x.MaxSpeed = current_ship.MaxSpeed;
		foreach (var item in current_ship.Motors) 
		{
			var y = instanciated_ship.gameObject.AddComponent<MotorManager>();
			y.Multiplier = item.Multiplier;
			y.Tile = item.Tile;
			y.Power = item.Power;
			y.Power_Fill_Count = item.Power_Fill_Count;
		}
		#endregion


		KoreKrush.Events.Logic.TilesSequenceFinish_L    += NextMove;
		KoreKrush.Events.Logic.ShipCollisionStart       += OnCollisionStarted;
		KoreKrush.Events.Logic.ShipWarpStart            += OnWarpStarted;
		KoreKrush.Events.Logic.ShipWarpEnd              += OnWarpEnded;
		KoreKrush.Events.Logic.ShipCollisionFinish 		+= OnCollisionEnded;
	}

	void OnDestroy()
	{
		KoreKrush.Events.Logic.TilesSequenceFinish_L 	-= NextMove;
		KoreKrush.Events.Logic.ShipCollisionStart 		-= OnCollisionStarted;
		KoreKrush.Events.Logic.ShipWarpStart 			-= OnWarpStarted;
		KoreKrush.Events.Logic.ShipWarpEnd 				-= OnWarpEnded;
		KoreKrush.Events.Logic.ShipCollisionFinish 		-= OnCollisionEnded;

	}
	// Use this for initialization
	void Start () {
		
		left_movement = current_level.Turns;
		turn_duration = current_level.Turn_time;
		count_down = current_level.Turn_time;
		distance_to_beat = current_level.Distance;
		objectives = current_level.Objectives;
		events = current_level.EventManager;

		distance_beated = false;
		time_start_collision = 0;
		collision = false;

		KoreKrush.Events.Logic.ObjectivesUiBuild (objectives);

		StartListening ();

		StartCoroutine ("UpdateAtTime", 0.3f);
		
	}		
	// Update is called once per frame
	void Update () {
		
	}


	void StartListening()
	{
		eventsEnumerator = events.GetEnumerator();
		lastMoveNext = eventsEnumerator.MoveNext ();
		if(lastMoveNext)
			actualEvent = eventsEnumerator.Current;
	}

	void NextMove()
	{
		PieceList loot = new PieceList ();

		Board.tilesSequence.ForEach (t => loot.Add ((Piece)(t.color)));
		AddPieces (loot);

		if(!warp && !collision) PassTurn ();
	}

	void AddPieces(PieceList list)
	{
		
		if(!warp) KoreKrush.Events.Logic.ManageSpeed (list);


		objectives.Subtract(list); 
		KoreKrush.Events.Logic.ObjectivesUpdate(objectives);

		if (objectives.Count == 0 && distance_beated) 
		{
			KoreKrush.Events.Logic.LevelCompleted ();
		}
	}

	void PassTurn()
	{
		left_movement -= 1;
		KoreKrush.Events.Logic.TurnsUpdate (left_movement);

		if (left_movement == 0) 
		{
			KoreKrush.Events.Logic.TurnsOut ();
		}

		last_count = Time.realtimeSinceStartup;
	}

	void Check(){
		if (lastMoveNext) {
			if (!made && actualEvent != null) {
				if (actualEvent.PathPosition < Helpers.VirtualDistanceToPathDistance(ShipManager.traveled_distance, 1, 10000) + instantiate_event_distance) {
					ExecuteEvent ();
					made = true;
				}
			}

			if (made) {
				lastMoveNext = eventsEnumerator.MoveNext ();
				if (lastMoveNext)
					actualEvent = eventsEnumerator.Current;
				made = false;
			}
		}
	}

	void ExecuteEvent ()
	{
		if (actualEvent.Obj is Obstacle) //TODO: hacer esto mas automatico
		{
			var obs = actualEvent.Obj as Obstacle;
			var meteor = Instantiate (obs.prefab);
			var agent = meteor.AddComponent<PathAgent> ();
			meteor.AddComponent<MeteorManager> ().info = (RewardEvent)actualEvent;
			agent.path = instanciated_ship.path;
			agent.initialValue = current_level.StartPosition + actualEvent.PathPosition;  //distancia de cinemachine
			agent.maxSpeed = - Helpers.VirtualSpeedToPathSpeed(obs.Speed);
			agent.gameObject.layer = LayerMask.NameToLayer("Obstacle");
			agent.move = true;

		}
	}

	void OnCollisionStarted(ObstacleManager meteor)
	{
		obstacle = meteor;
		collision = true;
		time_start_collision = Time.realtimeSinceStartup;
	}

	void OnCollisionEnded()
	{
		float time_to_destroy = Time.realtimeSinceStartup - time_start_collision;
		ManageReward (time_to_destroy, obstacle.info);
		last_count = Time.realtimeSinceStartup; // el contador de los turnos comienza desde 0
	}

	void ManageReward(float time, RewardEvent e)
	{
		var min = e.MaxRewardTime;
		var max = e.MinRewardTime;
		var rewards = e.Rewards;

		float percent = 0;
		if (time < min)
			percent = 1;
		else if (time > max)
			percent = 0;
		else
		{
			float a1 = -1 / (max - min);
			float a0 = 1 - a1 * min;
			percent = a0 + a1 * time; 
		}

		foreach (var item in rewards) {
			AddReward (item, (int)(percent * item.Count));
		}
	}

	void AddReward(Reward r, int cant)
	{
		if(r is PieceReward)
		{
			var pr = r as PieceReward;
			var reward = pr.tile;
			PieceList rewardList = new PieceList ();
			rewardList.Add (reward, cant);
			//TODO: posible animacion
			AddPieces (rewardList);

		}

	}

	void OnWarpStarted()
	{
		warp = true;
	}

	void OnWarpEnded()
	{
		warp = false;
		last_count = Time.realtimeSinceStartup;
	}

	IEnumerator UpdateAtTime(float time_frequency) //TODO: en caso de que no reste eficiencia significativamente poner esto en el update
	{
		while (true) 
		{
			if(!collision && !warp)
			{
				if (count_down <= 0) 
				{
					PassTurn();
					last_count = Time.realtimeSinceStartup;
				}
				count_down = turn_duration - (Time.realtimeSinceStartup - last_count); //TODO: mover esto de aqui, para un animador en el lugar donde se ven cuantos turnos te quedan
			}


			#region events
			Check (); //check if any event appear
			#endregion

			#region endlevel
			if (ShipManager.traveled_distance >= distance_to_beat) {
				distance_beated = true;
			}
			#endregion

			yield return new WaitForSeconds (time_frequency);

		}
	}






	#region Todo esto estara en otro lado ahora esta aqui para testear
	class BasicShip: Ship
	{

		public BasicShip ()
		{
			Motors = new List<Motor>();
			Motors.Add(new Motor(){Multiplier = 1.5f, Tile = Piece.red});
			Motors.Add(new Motor(){Multiplier = 2, Tile = Piece.blue});
			Motors.Add(new Motor(){Multiplier = 3, Tile = Piece.green});


			Gear gear1 = new Gear(10,180, 5);
			Gear gear2 = new Gear(25,600, 20);
			Gear gear3 = new Gear(50,1400, 40);
			Gear gear4 = new Gear(100,3000, 90);

			MinSpeed = 50;
			WarpDuration = 4;
			WarpBreakDamage = 4000;
			MaxSpeed = 5000;

			GearsBox = new List<Gear>(){gear1, gear2, gear3, gear4};

		}

	}

//	//TODO: hace los motores singleton o como se escriba
//	class RedMotor: Motor
//	{
//		public float Multiplier { get; set;}
//
//			public RedMotor ()
//			{
//				Multiplier = 1.5f;
//			}
//
//	}
//	class BlueMotor: Motor
//	{
//		public float Multiplier { get; set;}
//
//		public BlueMotor ()
//		{
//			Multiplier = 2;
//		}
//
//	}
//	class GreenMotor: Motor
//	{
//		public float Multiplier { get; set;}
//
//		public GreenMotor ()
//		{
//			Multiplier = 3;
//		}
//
//	}

	#endregion
}

