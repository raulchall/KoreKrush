using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using KoreKrush;

public class ShipManager : MonoBehaviour {

	public Ship current_ship;


	float actual_speed; //como se comportará esto si es en red?
	public static float traveled_distance;
	int gearbox_index;

	bool warp;



	void Awake()
	{
		#region por si acaso
		traveled_distance = 0;
		#endregion

		//KoreKrush.Events.Logic.ShipObstacleCollision                  += ManageCollision;    
		KoreKrush.Events.Logic.Warp += OnWarp_L;
	}
	// Use this for initialization
	void Start () {

		actual_speed = current_ship.MinSpeed;
		traveled_distance = 0;
		gearbox_index = 0;

		warp = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
		

	void OnTriggerEnter(Collider other) //Collision
	{
		other.gameObject.BroadcastMessage ("OnCollision");
		KoreKrush.Events.Logic.ShipCollisionStarted();


		var paht_script = GetComponent<PathAgent> ();
		paht_script.Speed = 0;
	}

	void OnWarp_L()
	{
		//TODO: todo
	}

	void ManageSpeed(List<TileCollection> loot) //por ahora lo unico que se analiza aqui es la velocidad no hay nada de efectos colaterales
	{
//		var motors = current_ship.Speed_processor;
//		var gears = current_ship.Speed_bars;
//
//		foreach (var item in loot) {
//			if (motors.ContainsKey (item.tile)) {
//				if(!warp)
//				{
//					float add_speed = item.Count * gears.gears [gearbox_index].base_speed * motors [item.tile].Multiplier;
//					AddSpeed (add_speed);
//				}
//			}
//		}
	}

	void AddSpeed(float additional_speed)
	{
//		var gears = current_ship.Speed_bars;
//		float additional_speed_tmp = additional_speed;
//
//
//
//		while(additional_speed_tmp > 0)
//		{		
//
//			if(gearbox_index == gears.gears.Count)
//			{
//				//TODO: warp, animacion, y demas implicaciones
//				warp = true;
//				break;
//			}
//
//
//			if (gears.gears [gearbox_index].speed_breaker - actual_speed < additional_speed_tmp) {
//
//				additional_speed_tmp -= gears.gears [gearbox_index].speed_breaker - actual_speed;
//				bar.size = 1; //TODO:la barra llega al tope... hacer alguna animacion o algo
//
//				//TODO: animacion de cambio de velocidad
//				//TODO: posible burst
//
//				bar.size = 0;
//				gearbox_index++;
//			} else {
//				var last_break = (gearbox_index == 0)? current_ship.MinSpeed: gears.gears [gearbox_index - 1].speed_breaker;
//				bar.size += additional_speed_tmp / (gears.gears [gearbox_index].speed_breaker - last_break);
//				additional_speed_tmp = 0;
//			}
//
//		}
//
//		actual_speed += additional_speed;

	}

	void DamageSpeed(float damage)
	{
//		var gears = current_ship.Speed_bars;
//		float damage_speed_tmp = damage;
//
//		var actual_speed_tmp = actual_speed;
//		actual_speed -= damage;
//		if (actual_speed < current_ship.MinSpeed)
//			actual_speed = current_ship.MinSpeed;
//		//TODO:actualizar barra
//
//		while(damage_speed_tmp > 0)
//		{		
//
//			if(gearbox_index == 0)
//			{
//
//				break;
//			}
//
//			var last_speed_breaker = gears.gears [gearbox_index - 1].speed_breaker;
//			if (actual_speed_tmp - last_speed_breaker < damage_speed_tmp) {
//				damage_speed_tmp -= actual_speed_tmp - last_speed_breaker;
//				bar.size = 0; //TODO:la barra llega al minimo... hacer alguna animacion o algo
//
//				//TODO: animacion de cambio de velocidad
//
//				bar.size = 1;
//				gearbox_index--;
//			} else {
//				var last_break = (gearbox_index == 0) ? current_ship.MinSpeed : gears.gears [gearbox_index - 1].speed_breaker;
//				bar.size -= damage_speed_tmp / (gears.gears [gearbox_index].speed_breaker - last_break);
//				damage_speed_tmp = 0;
//			}
//		}


	}
}
