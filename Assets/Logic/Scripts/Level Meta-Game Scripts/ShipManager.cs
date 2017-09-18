using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using KoreKrush;

#region Graphics using
using UnityEngine.UI;
#endregion

public class ShipManager : MonoBehaviour {

	#region Graphics Variables
	public Text speed_text;
	public Text distance_text;
	public Scrollbar bar;
	#endregion


	PathAgent Path_script;

	public List<Gear> GearsBox;
	public List<Motor> Motors;
	public float MinSpeed;
	public float MaxSpeed;
	public float WarpDuration;
	public float WarpBreakDamage;

	float actual_speed;

	public static float traveled_distance;
	public static int gearbox_index;
	public static float actual_gear_speed;
	public static bool warp;

	float damage_per_second;


	void Awake()
	{
		Path_script = GetComponent<PathAgent> ();

		//KoreKrush.Events.Logic.ShipObstacleCollision                  += ManageCollision;    
		KoreKrush.Events.Logic.WarpStarted += OnWarp_L;
		KoreKrush.Events.Logic.SpeedMultiplied += OnSpeedMultiplied;
		KoreKrush.Events.Logic.ShipCollisionEnded += OnEndCollision;
		KoreKrush.Events.Logic.SpeedSubtracted += OnDamageSpeed;
	}
	// Use this for initialization
	void Start () {

		actual_speed = MinSpeed;
		traveled_distance = 0;
		actual_gear_speed = 0;
		gearbox_index = 0;

		warp = false;

		damage_per_second = 0;

		Path_script.move = true; //sistema de play y pause

		StartCoroutine ("UpdateSpeed", 0.3f);
	}	
	// Update is called once per frame
	void Update () {
		
	}
		

	void OnTriggerEnter(Collider other) //Collision
	{
		other.gameObject.SendMessage ("OnCollision");
		var obstacle = other.GetComponent<MeteorManager> ();
		KoreKrush.Events.Logic.ShipCollisionStarted(obstacle);

		Path_script.move = false;
	}

	void OnWarp_L()
	{
		//TODO: todo
	}

	void OnSpeedMultiplied(float speed)
	{
		AddSpeed (GearsBox [gearbox_index].additional_base_speed * speed);
	}

	void OnEndCollision()
	{
		Path_script.move = true;
		damage_per_second = 0;
	}

	void OnDamageSpeed(float damage)
	{
		DamageSpeed (damage);
	}

//	void AddSpeed(float additional_speed) Old Add Speed
//	{
//
//		//TODO: asignarle un metodo: KoreKrush.Events.Logic.SpeedAdded (additional_speed);
//
//
//		float additional_speed_tmp = additional_speed;
//
//		while(additional_speed_tmp > 0)
//		{
//			if (GearsBox [gearbox_index].speed_breaker - actual_speed < additional_speed_tmp) 
//			{
//				additional_speed_tmp -= GearsBox [gearbox_index].speed_breaker - actual_speed;
//
//				#region Graphics
//				print("bar updating 0");
//				bar.size = 1; //TODO:la barra llega al tope... hacer alguna animacion o algo
//				//TODO: animacion de cambio de velocidad
//				//TODO: posible burst
//				#endregion
//
//
//
//				if(gearbox_index == GearsBox.Count - 1) //WARP
//				{
//					
//					KoreKrush.Events.Logic.WarpStarted();
//					Invoke ("EndWarp", WarpDuration); 
//					warp = true;
//					break;
//				}
//
//				gearbox_index++;
//
//				#region Graphics
//				bar.size = 0; //esto va debajo de warp
//				#endregion
//			} 
//			else 
//			{
//				var last_break = (gearbox_index == 0)? MinSpeed: GearsBox [gearbox_index - 1].speed_breaker;
//
//
//				#region Graphics
//				print("bar updating 1" + " " + (additional_speed_tmp / (GearsBox [gearbox_index].speed_breaker - last_break)));
//				bar.size += additional_speed_tmp / (GearsBox [gearbox_index].speed_breaker);
//				#endregion
//
//				additional_speed_tmp = 0;
//				break;
//			}
//
//		}
//
//		actual_speed += additional_speed;
//
//	} 

	void AddSpeed(float additional_speed)
	{
		print (additional_speed);
		//TODO: asignarle un metodo: KoreKrush.Events.Logic.SpeedAdded (additional_speed);

		while(additional_speed > 0)
		{
			if (actual_gear_speed + additional_speed > GearsBox [gearbox_index].speed_breaker) 
			{
				#region Graphics
				bar.size = 1; //TODO:la barra llega al tope... hacer alguna animacion o algo
				//TODO: animacion de cambio de velocidad
				//TODO: posible burst
				#endregion

				//print (actual_speed + "," + tmp);

				if(gearbox_index == GearsBox.Count - 1) //WARP
				{
					AffectSpeed (additional_speed);
					actual_gear_speed = GearsBox.Last ().speed_breaker;
					KoreKrush.Events.Logic.WarpStarted();
					Invoke ("EndWarp", WarpDuration); 
					warp = true;
					//TODO: actual_speed = MAX;
					break;
				}

				var tmp = GearsBox [gearbox_index].speed_breaker - actual_gear_speed;
				AffectSpeed (tmp);

				actual_gear_speed = 0;
				additional_speed -= tmp;


				gearbox_index++;

				#region Graphics
				bar.size = 0; //esto va debajo de warp
				#endregion
			} 
			else 
			{
				actual_gear_speed += additional_speed;
				AffectSpeed (additional_speed);

				#region Graphics
				//print(actual_gear_speed + "," + GearsBox[gearbox_index].speed_breaker);
				//print(actual_speed);
				bar.size = actual_gear_speed / GearsBox [gearbox_index].speed_breaker;
				#endregion

				additional_speed = 0;
				break;
			}

		}		

	}

//	void DamageSpeed(float damage) Old Damage Speed
//	{
//		float damage_speed_tmp = damage;
//
//		var actual_speed_tmp = actual_speed;
//		actual_speed -= damage;
//		if (actual_speed < MinSpeed)
//			actual_speed = MinSpeed;
//
//		while(damage_speed_tmp > 0)
//		{		
//
//			if(gearbox_index == 0)
//			{
//				#region Graphics
//				bar.size = (damage_speed_tmp - MinSpeed) / (GearsBox [gearbox_index].speed_breaker);
//				if (bar.size < 0)
//					bar.size = 0;
//				#endregion
//
//				break;
//			}
//
//			var last_speed_breaker = GearsBox [gearbox_index - 1].speed_breaker;
//			if (actual_speed_tmp - last_speed_breaker < damage_speed_tmp) {
//				
//				damage_speed_tmp -= actual_speed_tmp - last_speed_breaker;
//
//				#region Graphics
//				print("bar updating 2");
//				bar.size = 0; //TODO:la barra llega al minimo... hacer alguna animacion o algo
//				//TODO: animacion de cambio de velocidad
//				bar.size = 1;
//				#endregion
//
//				gearbox_index--;
//			} else {
//				var last_break = (gearbox_index == 0) ? MinSpeed : GearsBox [gearbox_index - 1].speed_breaker;
//				damage_speed_tmp = 0;
//
//				#region Graphics
//				print("bar updating 3");
//				bar.size = (damage_speed_tmp - last_break) / (GearsBox [gearbox_index].speed_breaker);
//				#endregion
//			}
//		}
//
//
//	}

	void DamageSpeed(float damage)
	{
		while (damage > 0) 
		{
			if (actual_gear_speed - damage < 0) {

				#region Graphics
				bar.size = 0; //TODO:la barra llega al minimo... hacer alguna animacion o algo
				//TODO: animacion de cambio de velocidad
				#endregion

				AffectSpeed (-actual_gear_speed);

				damage -= actual_gear_speed;

				if (gearbox_index == 0) {
					actual_gear_speed = 0;
					break;
				}

				actual_gear_speed = GearsBox [gearbox_index - 1].speed_breaker;

				gearbox_index--;

				#region Graphics
				bar.size = 1; //TODO:la barra llega al maximo... hacer alguna animacion o algo
				//TODO: animacion de cambio de velocidad
				#endregion
			} else {
				actual_gear_speed -= damage;
				AffectSpeed (-damage);
				damage = 0;
				//print ("else damage");
				#region Graphics
				bar.size = actual_gear_speed/GearsBox [gearbox_index].speed_breaker; //TODO:la barra llega al maximo... hacer alguna animacion o algo
				//TODO: animacion de cambio de velocidad
				#endregion

				break;
			}
		}
	}

	void AffectSpeed(float amount)
	{
		actual_speed += amount;
		if (actual_speed < MinSpeed)
			actual_speed = MinSpeed;
		else if (actual_speed > MaxSpeed)
			actual_speed = MaxSpeed;

		print (actual_speed);
	}

	void EndWarp()
	{
		KoreKrush.Events.Logic.WarpEnded ();
		warp = false;
		DamageSpeed (WarpBreakDamage);
	}

	IEnumerator UpdateSpeed(float time_frequency)
	{
		while (true) {
			
			if (!warp) {
				 DamageSpeed((GearsBox[gearbox_index].speed_lost_per_second + damage_per_second) * time_frequency);

				speed_text.text = "Barra " + (gearbox_index + 1) + ", Speed: " + (int)actual_speed;
			} else
				speed_text.text = "WARP " + ", Speed: " + (int)actual_speed;


			Path_script.Speed = Helpers.VirtualSpeedToPathSpeed (actual_speed);

			traveled_distance += actual_speed * time_frequency;

			distance_text.text = "Distancia: " + (int)traveled_distance;


			yield return new WaitForSeconds (time_frequency);
		}
	}
}
