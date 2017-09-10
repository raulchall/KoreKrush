using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using KoreKrush;

public class LevelManager : MonoBehaviour {

	[HideInInspector]
	public Level current_level;

	[HideInInspector]
	public Ship current_ship;

	public PathAgent instanciated_ship;

	public float instantiate_event_distance;



	#region level control variables
	float count_down;
	float last_count;

	int left_movement;
	float turn_duration;
	List<MeteorAppear>  events;

		#region collision variables
		bool collision;
		float time_start_collision;
		MeteorManager obstacle;
		#endregion

	float distance_to_beat;

	PieceList objectives;
	bool distance_beated;

	#endregion

	IEnumerator<MeteorAppear> eventsEnumerator;
	MeteorAppear actualEvent;
	private bool made;
	private bool lastMoveNext = true;


	void Awake()
	{
		#region esto de aqui se cargará de un json o algo asi
		var obj = new PieceList();
		obj.Add (Piece.blue, 15);
		obj.Add (Piece.green, 20);

		current_level = new Level { Objectives = obj, Turns = 30, Turn_time = 10 };

		current_ship = new BasicShip();
		#endregion

		KoreKrush.Events.Logic.TilesSequenceCompleted_L               += NextMove;
		KoreKrush.Events.Logic.ShipCollisionStarted += ManageCollision;

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


		StartCoroutine ("UpdateAtTime", 0.3f);
		
	}

	void StartListening()
	{
		eventsEnumerator = events.GetEnumerator();
		eventsEnumerator.MoveNext ();
		actualEvent = eventsEnumerator.Current;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void NextMove() //TODO: hacer un funcion que haga eso para que puedan regalarte cosas sin pasar el turno
	{
		PieceList loot = new PieceList ();
		Board.tilesSequence.ForEach (t => loot.Add ((Piece)t.color));

		AddPieces (loot);

		PassTurn ();
	}

	void AddPieces(PieceList list)
	{
		KoreKrush.Events.Logic.ManageSpeed (list);  //TODO: hacer script de motores y que escuchen este evento
		objectives.Subtract(list);

		KoreKrush.Events.Logic.ObjectivesUpdated(objectives);
	}

	void PassTurn()
	{
		left_movement -= 1;
		KoreKrush.Events.Logic.TurnsUpdated (left_movement);

		if (objectives.Count == 0 && distance_beated) 
		{
			KoreKrush.Events.Logic.LevelCompleted ();
		}
		else if (left_movement == 0) 
		{
			KoreKrush.Events.Logic.TurnsOut ();
		}

		last_count = Time.realtimeSinceStartup;
	}

	void Check(){
		if (lastMoveNext) {
			if (!made && actualEvent != null) {
				if (actualEvent.PathPosition < ShipManager.traveled_distance + instantiate_event_distance) {
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
		if (actualEvent is MeteorAppear) //TODO: hacer esto mas automatico
		{
			var meteor = Instantiate (actualEvent.prefab);
			var agent = meteor.AddComponent<PathAgent> ();
			meteor.AddComponent<MeteorManager> ().info = actualEvent;
			agent.path = instanciated_ship.path;
			agent.pathAmount = current_level.StartPosition + actualEvent.PathPosition;  //distancia de cinemachine
			agent.Speed = - Helpers.VirtualSpeedToPathSpeed(actualEvent.Speed);
			agent.move = true;

		}
	}

	void ManageCollision(MeteorManager meteor)
	{
		obstacle = meteor;
		collision = true;
		time_start_collision = Time.realtimeSinceStartup;
	}

	void ManageReward(float time, float min, float max, List<Reward> rewards)
	{
		float a1 = - 1 / (max - min);
		float a0 = 1 - a1 * min;
		float percent = a0 + a1 * time;

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

	IEnumerator UpdateAtTime(float time_frequency) //TODO: en caso de que no reste eficiencia significativamente poner esto en el update
	{
		while (true) 
		{
			if (collision) 
			{
				if (ShipManager.gearbox_index > obstacle.info.GearToBreak) 
				{
					float time_to_destroy = Time.realtimeSinceStartup - time_start_collision;
					ManageReward (time_to_destroy, obstacle.info.MinRewardTime, obstacle.info.MaxRewardTime, obstacle.info.Rewards);
					obstacle.SendMessage ("Destroy");
					KoreKrush.Events.Logic.SpeedSubtracted (obstacle.info.SpeedDamageWhenBreak);
					KoreKrush.Events.Logic.ShipCollisionEnded ();
				}
				if (ShipManager.gearbox_index < obstacle.info.GearToBreak) 
				{
					KoreKrush.Events.Logic.Defeated ();
				}
			}
			
			#region turn duration
			if (count_down <= 0) {
				PassTurn();
				count_down = turn_duration;
			}

			count_down = turn_duration - (Time.realtimeSinceStartup - last_count); //TODO: mover esto de aqui, para un animador en el lugar donde se ven cuantos turnos te quedan
			#endregion

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

