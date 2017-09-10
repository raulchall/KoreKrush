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
	List<Gear> GearsBox;
	List<Motor> Motors;
	float MinSpeed;
	float WarpDuration;
	float WarpBreakDamage;

	float actual_speed;

	public static float traveled_distance;
	public static int gearbox_index;
	public static bool warp;

	float damage_per_second;


	void Awake()
	{
		Path_script = GetComponent<PathAgent> ();

		//KoreKrush.Events.Logic.ShipObstacleCollision                  += ManageCollision;    
		KoreKrush.Events.Logic.WarpStarted += OnWarp_L;
		KoreKrush.Events.Logic.SpeedMultiplied += OnSpeedAdd;
		KoreKrush.Events.Logic.ShipCollisionEnded += OnEndCollision;
		KoreKrush.Events.Logic.SpeedSubtracted += OnDamageSpeed;
	}
	// Use this for initialization
	void Start () {

		actual_speed = MinSpeed;
		traveled_distance = 0;
		gearbox_index = 0;

		warp = false;

		damage_per_second = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
		

	void OnTriggerEnter(Collider other) //Collision
	{
		other.gameObject.BroadcastMessage ("OnCollision");
		var obstacle = other.GetComponent<MeteorManager> ();
		damage_per_second = obstacle.info.SpeedDamagePerSecond;
		KoreKrush.Events.Logic.ShipCollisionStarted(obstacle);



		Path_script.move = false;
	}

	void OnWarp_L()
	{
		//TODO: todo
	}

	void OnSpeedAdd(float speed)
	{
		AddSpeed (GearsBox [gearbox_index].base_speed * speed);
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

	void AddSpeed(float additional_speed)
	{

		KoreKrush.Events.Logic.SpeedAdded (additional_speed);


		float additional_speed_tmp = additional_speed;

		while(additional_speed_tmp > 0)
		{		

			if(gearbox_index == GearsBox.Count) //WARP
			{
				KoreKrush.Events.Logic.WarpStarted();
				Invoke ("EndWarp", WarpDuration); 
				warp = true;
				break;
			}


			if (GearsBox [gearbox_index].speed_breaker - actual_speed < additional_speed_tmp) 
			{
				additional_speed_tmp -= GearsBox [gearbox_index].speed_breaker - actual_speed;

				#region Graphics
				bar.size = 1; //TODO:la barra llega al tope... hacer alguna animacion o algo

				//TODO: animacion de cambio de velocidad
				//TODO: posible burst
				bar.size = 0;
				#endregion

				gearbox_index++;
			} 
			else 
			{
				var last_break = (gearbox_index == 0)? MinSpeed: GearsBox [gearbox_index - 1].speed_breaker;
				additional_speed_tmp = 0;

				#region Graphics
				bar.size += additional_speed_tmp / (GearsBox [gearbox_index].speed_breaker - last_break);
				#endregion

				break;
			}

		}

		actual_speed += additional_speed;

	}

	void DamageSpeed(float damage)
	{
		float damage_speed_tmp = damage;

		var actual_speed_tmp = actual_speed;
		actual_speed -= damage;
		if (actual_speed < MinSpeed)
			actual_speed = MinSpeed;

		while(damage_speed_tmp > 0)
		{		

			if(gearbox_index == 0)
			{
				#region Graphics
				bar.size -= damage_speed_tmp / (GearsBox [gearbox_index].speed_breaker);
				if (bar.size < 0)
					bar.size = 0;
				#endregion

				break;
			}

			var last_speed_breaker = GearsBox [gearbox_index - 1].speed_breaker;
			if (actual_speed_tmp - last_speed_breaker < damage_speed_tmp) {
				damage_speed_tmp -= actual_speed_tmp - last_speed_breaker;

				#region Graphics
				bar.size = 0; //TODO:la barra llega al minimo... hacer alguna animacion o algo
				//TODO: animacion de cambio de velocidad
				bar.size = 1;
				#endregion

				gearbox_index--;
			} else {
				var last_break = (gearbox_index == 0) ? MinSpeed : GearsBox [gearbox_index - 1].speed_breaker;
				damage_speed_tmp = 0;

				#region Graphics
				bar.size -= damage_speed_tmp / (GearsBox [gearbox_index].speed_breaker - last_break);
				#endregion
			}
		}


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
			if (!warp) 
			{
				actual_speed -= (GearsBox[gearbox_index].speed_lost_per_second + damage_per_second) * time_frequency;
				if (actual_speed < MinSpeed)
					actual_speed = MinSpeed;
				speed_text.text = "Barra " + (gearbox_index + 1) + ", Speed: " + (int)actual_speed;

			}

			Path_script.Speed = Helpers.VirtualSpeedToPathSpeed (actual_speed);

			traveled_distance += actual_speed * time_frequency;

			distance_text.text = "Distancia: " + (int)traveled_distance;


			yield return new WaitForSeconds (time_frequency);
		}
	}
}
