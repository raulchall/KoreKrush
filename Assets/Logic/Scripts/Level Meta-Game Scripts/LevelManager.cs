using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using KoreKrush;

public class LevelManager : MonoBehaviour {

	public Level current_level;
	public Ship current_ship;





	#region level control variables
	float count_down;
	float last_count;

	int left_movement;
	float turn_duration;

	float distance_to_beat;

	BagList<Piece> objectives;
	bool distance_beated;

	float actual_speed;
	float traveled_distance;
	int gearbox_index;


	bool warp;
	#endregion

	void Awake()
	{
		#region esto de aqui se cargará de un json o algo asi
		var obj = new BagList<Piece>();
		obj.Add (Piece.blue, 15);
		obj.Add (Piece.green, 20);

		current_level = new Level { Objectives = obj, Turns = 30, Turn_time = 10 };

		current_ship = new BasicShip();
		#endregion

		KoreKrush.Events.Logic.TilesSequenceCompleted_L               += NextMove;

	}

	// Use this for initialization
	void Start () {



		left_movement = current_level.Turns;
		turn_duration = current_level.Turn_time;
		count_down = current_level.Turn_time;
		distance_to_beat = current_level.Distance;
		objectives = current_level.Objectives;

		distance_beated = false;


		StartCoroutine ("UpdateTime", 0.3f);
		
	}
	
	// Update is called once per frame
	void Update () {

	}




	void NextMove() //TODO: hacer un funcion que haga eso para que puedan regalarte cosas sin pasar el turno
	{
		BagList<Piece> loot = new BagList<Piece> ();
		Board.tilesSequence.ForEach (t => loot.Add ((int)t));

		KoreKrush.Events.Logic.ManageSpeed (loot);  //TODO: hacer script de motores y que escuchen este evento
		objectives.Subtract(loot);

		KoreKrush.Events.Logic.ObjectivesUpdated(objectives);

		PassTurn ();
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

	void ManageCollision()
	{
		//TODO: todo
		var a = (Piece)Board.tilesSequence [0];
	}

	void OnWarp_L()
	{
		//TODO: todo
	}

	void ManageSpeed(List<TileCollection> loot) //por ahora lo unico que se analiza aqui es la velocidad no hay nada de efectos colaterales
	{
		var motors = current_ship.Speed_processor;
		var gears = current_ship.Speed_bars;

		foreach (var item in loot) {
			if (motors.ContainsKey (item.tile)) {
				if(!warp)
				{
					float add_speed = item.Count * gears.gears [gearbox_index].base_speed * motors [item.tile].Multiplier;
					AddSpeed (add_speed);
				}
				}
		}
	}
		
	void AddSpeed(float additional_speed)
	{
		var gears = current_ship.Speed_bars;
		float additional_speed_tmp = additional_speed;



		while(additional_speed_tmp > 0)
		{		

			if(gearbox_index == gears.gears.Count)
			{
				//TODO: warp, animacion, y demas implicaciones
				warp = true;
				break;
			}
				

			if (gears.gears [gearbox_index].speed_breaker - actual_speed < additional_speed_tmp) {
				
				additional_speed_tmp -= gears.gears [gearbox_index].speed_breaker - actual_speed;
				bar.size = 1; //TODO:la barra llega al tope... hacer alguna animacion o algo

				//TODO: animacion de cambio de velocidad
				//TODO: posible burst

				bar.size = 0;
				gearbox_index++;
			} else {
				var last_break = (gearbox_index == 0)? current_ship.MinSpeed: gears.gears [gearbox_index - 1].speed_breaker;
				bar.size += additional_speed_tmp / (gears.gears [gearbox_index].speed_breaker - last_break);
				additional_speed_tmp = 0;
			}

		}

		actual_speed += additional_speed;

	}

	void DamageSpeed(float damage)
	{
		var gears = current_ship.Speed_bars;
		float damage_speed_tmp = damage;

		var actual_speed_tmp = actual_speed;
		actual_speed -= damage;
		if (actual_speed < current_ship.MinSpeed)
			actual_speed = current_ship.MinSpeed;
		//TODO:actualizar barra

		while(damage_speed_tmp > 0)
		{		

			if(gearbox_index == 0)
			{
				
				break;
			}

			var last_speed_breaker = gears.gears [gearbox_index - 1].speed_breaker;
			if (actual_speed_tmp - last_speed_breaker < damage_speed_tmp) {
				damage_speed_tmp -= actual_speed_tmp - last_speed_breaker;
				bar.size = 0; //TODO:la barra llega al minimo... hacer alguna animacion o algo

				//TODO: animacion de cambio de velocidad

				bar.size = 1;
				gearbox_index--;
			} else {
				var last_break = (gearbox_index == 0) ? current_ship.MinSpeed : gears.gears [gearbox_index - 1].speed_breaker;
				bar.size -= damage_speed_tmp / (gears.gears [gearbox_index].speed_breaker - last_break);
				damage_speed_tmp = 0;
			}
		}


	}

	IEnumerator UpdateTime(float time_frequency){
		while (true) 
		{
			
			if (count_down <= 0) {
				PassTurn();
				count_down = turn_duration;
			}

			count_down = turn_duration - (Time.realtimeSinceStartup - last_count); //TODO: mover esto de aqui, para un animador en el lugar donde se ven cuantos turnos te quedan

//			actual_speed -= current_ship.Speed_bars.speed_lost_per_second*time_frequency;
//			speed_text.text = "Barra " + (gearbox_index + 1) + ", Speed: " + (int)actual_speed;
//
//			traveled_distance += actual_speed * time_frequency;
//			distance_text.text = "Distancia: " + (int)traveled_distance;

			if (traveled_distance >= distance_to_beat) {
				distance_beated = true;
			}

			yield return new WaitForSeconds (time_frequency);

		}
	}




	#region Todo esto estara en otro lado ahora esta aqui para testear
	class BasicShip: Ship
	{
		public Dictionary<Piece, Motor> Speed_processor{ get; set;}
		public GearsBox Speed_bars{ get; set;}

		public float MinSpeed { get; set; }

		public BasicShip ()
		{
			Speed_processor = new Dictionary<Piece, Motor>();
			Speed_processor.Add(Piece.red, new RedMotor());
			Speed_processor.Add(Piece.blue, new BlueMotor());
			Speed_processor.Add(Piece.green, new GreenMotor());


			Gear gear1 = new Gear(10,180);
			Gear gear2 = new Gear(25,600);
			Gear gear3 = new Gear(50,1400);
			Gear gear4 = new Gear(100,3000);

			MinSpeed = 50;

			Speed_bars = new GearsBox(new List<Gear>(){gear1, gear2, gear3, gear4},5);

		}

	}

	//TODO: hace los motores singleton o como se escriba
	class RedMotor: Motor
	{
		public float Multiplier { get; set;}

			public RedMotor ()
			{
				Multiplier = 1.5f;
			}

	}
	class BlueMotor: Motor
	{
		public float Multiplier { get; set;}

		public BlueMotor ()
		{
			Multiplier = 2;
		}

	}
	class GreenMotor: Motor
	{
		public float Multiplier { get; set;}

		public GreenMotor ()
		{
			Multiplier = 3;
		}

	}

	#endregion
}

