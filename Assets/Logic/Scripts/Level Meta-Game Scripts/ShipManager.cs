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


	float actual_speed; //como se comportará esto si es en red?
	public static float traveled_distance;
	int gearbox_index;

	bool warp;



	void Awake()
	{
		Path_script = GetComponent<PathAgent> ();

		//KoreKrush.Events.Logic.ShipObstacleCollision                  += ManageCollision;    
		KoreKrush.Events.Logic.WarpStarted += OnWarp_L;
		KoreKrush.Events.Logic.SpeedMultiplied += OnSpeedAdd;
	}
	// Use this for initialization
	void Start () {

		actual_speed = MinSpeed;
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



		Path_script.Speed = 0;
	}

	void OnWarp_L()
	{
		//TODO: todo
	}

	void OnSpeedAdd(float speed)
	{
		AddSpeed (GearsBox [gearbox_index].base_speed * speed);
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
		KoreKrush.Events.Logic.SpeedSubtracted (damage);

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

	float TransformVirtualSpeedToPathSpeed(float virtualSpeed)
	{
		float a1 = 0.09f / 2950;
		float a0 = 0.01f - 50 * a1;
		return a0 + virtualSpeed*a1;
	}

	IEnumerator UpdateSpeed(float time_frequency)
	{
		while (true) {
			if (!warp) 
			{
				actual_speed -= GearsBox[gearbox_index].speed_lost_per_second * time_frequency;
				if (actual_speed < MinSpeed)
					actual_speed = MinSpeed;
				speed_text.text = "Barra " + (gearbox_index + 1) + ", Speed: " + (int)actual_speed;

			}

			Path_script.Speed = TransformVirtualSpeedToPathSpeed (actual_speed);

			traveled_distance += actual_speed * time_frequency;

			distance_text.text = "Distancia: " + (int)traveled_distance;


			yield return new WaitForSeconds (time_frequency);
		}
	}
}
