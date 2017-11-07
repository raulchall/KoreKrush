using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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

	bool warp;


	void Awake()
	{
		var x = instanciated_ship.gameObject.GetComponent<ShipManager>();

		x.GearsBox = current_ship.GearsBox;
		x.MinSpeed = current_ship.MinSpeed;
		x.WarpDuration = current_ship.WarpDuration;
		x.WarpBreakDamage = current_ship.WarpBreakDamage;
		x.MaxSpeed = current_ship.MaxSpeed;
		foreach (var motor in current_ship.Motors) 
		{
			var y = instanciated_ship.gameObject.AddComponent<MotorManager>();
            y.m_Motor = motor;
		}


		KoreKrush.Events.Logic.TilesSequenceFinish_L    += NextMove;
		KoreKrush.Events.Logic.ShipCollisionStart       += OnCollisionStarted;
		KoreKrush.Events.Logic.ShipWarpStart            += OnWarpStarted;
		KoreKrush.Events.Logic.ShipWarpEnd              += OnWarpEnded;
		KoreKrush.Events.Logic.ShipCollisionFinish 		+= OnCollisionEnded;
	}
	// Destroy all events links
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
		turn_duration = current_level.TurnTime;
		count_down = current_level.TurnTime;
		distance_to_beat = current_level.Distance;

		objectives = new PieceList(current_level.Objectives);

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


		Board.tilesSequence.ForEach (t => loot.Add (t.TileType, t.Value));

		AddPieces (loot);

		if(!warp && !collision) PassTurn ();
	}

	void AddPieces(PieceList list)
	{
		if(!warp) KoreKrush.Events.Logic.ManageSpeed (list);


		objectives.Subtract(list); 

		KoreKrush.Events.Logic.ObjectivesUpdate(objectives);


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
				if (actualEvent.PathPosition < LocalHelper.VirtualDistanceToPathDistance(ShipManager.traveled_distance, 1, 10000) + instantiate_event_distance) {
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
			agent.maxSpeed = - LocalHelper.VirtualSpeedToPathSpeed(obs.Speed);
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
            PieceList rewardList = new PieceList
            {
                { reward, cant }
            };
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
                KoreKrush.Events.Logic.ShipTravelFinish();
			}
			#endregion

			#region winlevel
			//TODO: quitar esto de aqui y preguntarlo mas eficientemente
			if (objectives.Count == 0 && distance_beated) 
			{
				KoreKrush.Events.Logic.LevelCompleted ();
			}
			#endregion

			yield return new WaitForSeconds (time_frequency);

		}
	}

}