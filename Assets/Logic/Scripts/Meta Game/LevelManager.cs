using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

using KoreKrush;

//TODO: sistema de pause y play
public class LevelManager : MonoBehaviour {


	public Level current_level;

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
//			var obj = new PieceList();
//			obj.Add (Piece.blue, 150);
//			obj.Add (Piece.green, 200);
//////
//			current_level = ScriptableObject.CreateInstance<Level>();
//////			
//////			Obstacle obs1 =  ScriptableObject.CreateInstance<Obstacle>();
//////			obs1.prefab = meteor_prefab;
//////			obs1.Speed = 300;
//////			obs1.GearToBreak = 1;
//////			obs1.SpeedDamageWhenBreak = 100;
//////
//////			Obstacle obs2 =  ScriptableObject.CreateInstance<Obstacle>();
//////			obs2.prefab = meteor_prefab;
//////			obs2.Speed = 1300;
//////			obs2.GearToBreak = 3;
//////			obs2.SpeedDamageWhenBreak = 1000;
//////			AssetDatabase.CreateAsset(obs1, "Assets/Scenes/Levels/Collision/Resources/obstacle1.asset");
//////			AssetDatabase.CreateAsset(obs2, "Assets/Scenes/Levels/Collision/Resources/obstacle2.asset");
//////			
////					
//			var obs1 = AssetDatabase.LoadAssetAtPath<Obstacle>("Assets/Scenes/Levels/Collision/Resources/obstacle1.asset");	
//			var obs2 = AssetDatabase.LoadAssetAtPath<Obstacle>("Assets/Scenes/Levels/Collision/Resources/obstacle2.asset");	
////	
//			var eve = new List<LevelEvent>(){
//			new LevelEvent(){
//					Obj = obs1,
//					Rewards = new List<PieceReward>(){ new PieceReward(Piece.blue, 6), new PieceReward(Piece.green, 7)}, 
//					MinRewardTime = 5, 
//					MaxRewardTime = 10, 
//					PathPosition = 0.5f},
//
//			new LevelEvent(){
//					Obj = obs2,
//					Rewards = new List<PieceReward>(){ new PieceReward(Piece.blue, 6), new PieceReward(Piece.green, 7)}, 
//					MinRewardTime = 5, 
//					MaxRewardTime = 10, 
//					PathPosition = 1},
//			};
//////
//			current_level.Objectives = obj;
//			current_level.Turns = 300;
//			current_level.Turn_time = 2;
//			current_level.Distance = 10000;
//			current_level.EventManager = eve;
//			
//			AssetDatabase.CreateAsset(current_level, "Assets/Scenes/Levels/Collision/Resources/Level.asset");
//			AssetDatabase.SaveAssets();
		#endregion
			
		print (current_level.EventManager[0].Obj.Speed);
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
		foreach (var item in current_level.Objectives) {
			print (item.Key + " --> " + item.Value);
		}

		objectives = new PieceList(current_level.Objectives);
		Debug.Log ("Start level "+ objectives.lCount);

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
		print ("start add debug");
		print (loot.Count);
		print (loot.lCount);
		print ("end add debug");

		AddPieces (loot);

		if(!warp && !collision) PassTurn ();
	}

	void AddPieces(PieceList list)
	{
		
		if(!warp) KoreKrush.Events.Logic.ManageSpeed (list);


		objectives.Subtract(list); 
		print (objectives.Count);

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
			meteor.AddComponent<MeteorManager> ().info = (LevelEvent)actualEvent;
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

//		if(obstacle.info is RewardEvent)
//			ManageReward (time_to_destroy, obstacle.info as RewardEvent);
		ManageReward (time_to_destroy, obstacle.info);

		last_count = Time.realtimeSinceStartup; // el contador de los turnos comienza desde 0
	}

	void ManageReward(float time, LevelEvent e)
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





//
//	#region Todo esto estara en otro lado ahora esta aqui para testear
//	class BasicShip: Ship
//	{
//
//		public BasicShip ()
//		{
//			Motors = new List<Motor>();
//			Motor redmotor = ScriptableObject.CreateInstance<Motor>();
//			redmotor.Multiplier = 1.5f;
//			redmotor.Tile = Piece.red;
//			AssetDatabase.CreateAsset(redmotor, "Assets/Scenes/Levels/Collision/Resources/redMotor.asset");
//
//			Motor bluemotor = ScriptableObject.CreateInstance<Motor>();
//			bluemotor.Multiplier = 2;
//			bluemotor.Tile = Piece.blue;
//			AssetDatabase.CreateAsset(bluemotor, "Assets/Scenes/Levels/Collision/Resources/blueMotor.asset");
//
//			Motor greenmotor = ScriptableObject.CreateInstance<Motor>();
//			greenmotor.Multiplier = 3;
//			greenmotor.Tile = Piece.green;
//			AssetDatabase.CreateAsset(greenmotor, "Assets/Scenes/Levels/Collision/Resources/greenMotor.asset");
//
//			Ship basicShip = ScriptableObject.CreateInstance<Ship>();
//
//			Motor red = AssetDatabase.LoadAssetAtPath<Motor>("Assets/Scenes/Levels/Collision/Resources/redMotor.asset");
//			Motor blue = AssetDatabase.LoadAssetAtPath<Motor>("Assets/Scenes/Levels/Collision/Resources/blueMotor.asset");
//			Motor green = AssetDatabase.LoadAssetAtPath<Motor>("Assets/Scenes/Levels/Collision/Resources/greenMotor.asset");
//
//			basicShip.Motors = new List<Motor>();
//			basicShip.Motors.Add(red);
//			basicShip.Motors.Add(blue);
//			basicShip.Motors.Add(green);
//
//
//
//			Motors.Add(new Motor(){Multiplier = 1.5f, Tile = Piece.red});
//			Motors.Add(new Motor(){Multiplier = 2, Tile = Piece.blue});
//			Motors.Add(new Motor(){Multiplier = 3, Tile = Piece.green});
//
//
//			Gear gear1 = new Gear(10,180, 5);
//			Gear gear2 = new Gear(25,600, 20);
//			Gear gear3 = new Gear(50,1400, 40);
//			Gear gear4 = new Gear(100,3000, 90);
//
//			MinSpeed = 50;
//			WarpDuration = 4;
//			WarpBreakDamage = 4000;
//			MaxSpeed = 5000;
//
//			basicShip.MinSpeed = MinSpeed;
//			basicShip.WarpDuration = WarpDuration;
//			basicShip.WarpBreakDamage = WarpBreakDamage;
//			basicShip.MaxSpeed = MaxSpeed;
//
//			GearsBox = new List<Gear>(){gear1, gear2, gear3, gear4};
//
//			basicShip.GearsBox = GearsBox;
//
//			AssetDatabase.CreateAsset(basicShip, "Assets/Scenes/Levels/Collision/Resources/basicShip.asset");
//
//			AssetDatabase.SaveAssets();
//
//		}
//
//	}
//
//	#endregion
}

