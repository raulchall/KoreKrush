using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


using KoreKrush;

public class MotorManager : MonoBehaviour {

	public float Multiplier;
	public TileType Tile;
	public Ability Power;
	public int Power_Fill_Count;

    int fill_counter;
	void Awake()
	{
		KoreKrush.Events.Logic.ManageSpeed += OnTilesProcesing;
	}

	void OnDestroy()
	{
		KoreKrush.Events.Logic.ManageSpeed -= OnTilesProcesing;
	}

	// Use this for initialization
	void Start () {
        fill_counter = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	//TODO: hacer esto mas eficiente, o sea que a un motor solo le lleguen los elementos que quiere procesar
	void OnTilesProcesing(PieceList plist)
	{
		if (plist.ContainsKey(Tile))
		{
			float mult = plist[Tile] * Multiplier * Helpers.Multiplier(plist.Count);
			KoreKrush.Events.Logic.SpeedMultiply (mult);
		}
	}
}
