using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using KoreKrush;

#region Graphics using
using UnityEngine.UI;
#endregion

public class ShipManager : MonoBehaviour {

	#region Graphics Variables
	Text speed_text;
	Text distance_text;
	Scrollbar bar;
	#endregion


	PathAgent Path_script;

	public List<Gear> GearsBox;
	public List<Motor> Motors;
	public float MinSpeed;
	public float MaxSpeed;
	public float WarpDuration;
	public float WarpBreakDamage;

	public static float actual_speed;

	public static float traveled_distance;
	public static int gearbox_index;
	public static float actual_gear_speed;
	public static bool warp;

	float damage_per_second;

	bool collision;

	void Awake()
	{
		Path_script = GetComponent<PathAgent> ();

		KoreKrush.Events.Logic.ShipWarpStart 		+= OnWarp_L;
		KoreKrush.Events.Logic.SpeedMultiply 		+= OnSpeedMultiplied;
		KoreKrush.Events.Logic.SpeedSubtract 		+= OnDamageSpeed;


	}
	// Destroy all events links
	void OnDestroy()
	{
		KoreKrush.Events.Logic.ShipWarpStart 		-= OnWarp_L;
		KoreKrush.Events.Logic.SpeedMultiply 		-= OnSpeedMultiplied;
		KoreKrush.Events.Logic.SpeedSubtract 		-= OnDamageSpeed;
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

		LoadUI ();


		StartCoroutine ("UpdateSpeed", 0.3f);
	}	
	// Update is called once per frame
	void Update () {
		
	}
		

	void LoadUI()
	{
		bar = GameObject.Find ("Scrollbar").GetComponent<Scrollbar>();
		speed_text = GameObject.Find ("Speed text").GetComponent<Text>();
		distance_text = GameObject.Find ("Distance text").GetComponent<Text>();
	}

	void OnTriggerEnter(Collider other) //Collision
	{
		var obstacle = other.GetComponent<ObstacleManager> ();
		obstacle.OnCollision ();
		//TODO: diferenciar los tipos de colision
		StartCoroutine(ManageObstacleCollision (obstacle));
	}

	IEnumerator ManageObstacleCollision(ObstacleManager m)
	{
		KoreKrush.Events.Logic.ShipCollisionStart (m);
		Path_script.move = false;
		collision = true;

		while (true) {
			if (collision) {
				if (warp || gearbox_index > m.obstacle_info.GearToBreak) {
					m.OnEndCollision();
					if (!warp)
						KoreKrush.Events.Logic.SpeedSubtract (m.obstacle_info.SpeedDamageWhenBreak);

					collision = false;
					Path_script.move = true;
					damage_per_second = 0;
                    KoreKrush.Events.Logic.ShipCollisionFinish();
                    yield break;
				}
				if (gearbox_index < m.obstacle_info.GearToBreak) {
					collision = false;
					//Destroy (m);
					KoreKrush.Events.Logic.PlayerDefeat ();
				}
			}

			//if(gearbox_index == m.info.GearToBreak) => siguen fajaos!
			yield return new WaitForSeconds(0.2f);
		}
	}

//	void OnEndCollision()
//	{
//		//TODO:diferenciar entre los diferentes tipos de eventos a terminar, tip: el nombre de la corutina depende del evento asi se podrian sumar los strings
//
//		StopCoroutine ("ManageObstacleCollision");
//	}

	void OnWarp_L()
	{
		
	}

	void OnSpeedMultiplied(float speed)
	{
		AddSpeed (GearsBox [gearbox_index].additional_base_speed * speed);
	}


	void OnDamageSpeed(float damage)
	{
		DamageSpeed (damage);
	}

	void AddSpeed(float additional_speed)
	{
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

				if(gearbox_index == GearsBox.Count - 1) //WARP
				{
					AffectSpeed (additional_speed);
					actual_gear_speed = GearsBox.Last ().speed_breaker;
					KoreKrush.Events.Logic.ShipWarpStart();
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
				GearBoxUpdate ();
				#region Graphics
				bar.size = 0; //esto va debajo de warp
				#endregion
			} 
			else 
			{
				actual_gear_speed += additional_speed;
				AffectSpeed (additional_speed);

				#region Graphics
				bar.size = actual_gear_speed / GearsBox [gearbox_index].speed_breaker;
				#endregion

				additional_speed = 0;
				break;
			}

		}		

	}

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
				GearBoxUpdate ();

				#region Graphics
				bar.size = 1; //TODO:la barra llega al maximo... hacer alguna animacion o algo
				//TODO: animacion de cambio de velocidad
				#endregion
			} else {
				actual_gear_speed -= damage;
				AffectSpeed (-damage);
				damage = 0;
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

	}

	void GearBoxUpdate()
	{
		//TODO: quitar la corutina que que pregunta en cada frame si cambio el gearindex
	}

	void EndWarp()
	{
		KoreKrush.Events.Logic.ShipWarpEnd ();
		warp = false;
		DamageSpeed (WarpBreakDamage);
	}

	IEnumerator UpdateSpeed(float time_frequency)
	{
		while (true) {
			
			if (!warp) {
				 DamageSpeed((GearsBox[gearbox_index].speed_lost_per_second + damage_per_second) * time_frequency);

				speed_text.text = "Bar " + (gearbox_index + 1) + ", Speed: " + (int)actual_speed;
			} else
				speed_text.text = "WARP " + ", Speed: " + (int)actual_speed;


			Path_script.maxSpeed = LocalHelper.VirtualSpeedToPathSpeed (actual_speed);

			traveled_distance = Path_script.pathAmount*10000;
			distance_text.text = "Distance: " + (int)traveled_distance;


			yield return new WaitForSeconds (time_frequency);
		}
	}
}
