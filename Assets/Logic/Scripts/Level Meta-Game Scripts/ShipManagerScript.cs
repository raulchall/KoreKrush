using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using KoreKrush;

public class ShipManagerScript : MonoBehaviour {

	public Ship current_ship;

	float distance_to_beat;

	float actual_speed;
	float traveled_distance;
	int gearbox_index;

	bool warp;



	void Awake()
	{


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

	void OnWarp_L()
	{
		
	}

}
