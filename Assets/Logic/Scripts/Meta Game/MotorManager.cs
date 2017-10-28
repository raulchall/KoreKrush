using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using KoreKrush;

public class MotorManager : MonoBehaviour {

	public float Multiplier;
	public Piece Tile;
	public Ability Power;
	public int Power_Fill_Count;

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
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	//TODO: hacer esto mas eficiente, o sea que a un motor solo le lleguen los elementos que quiere procesar
	void OnTilesProcesing(PieceList list)
	{
		if (list.ContainsKey(Tile))
		//if(list.list.ContainsKey(Tile))
		{
			float mult = list[Tile] * Multiplier * Helpers.Multiplier(list.Count);
			print ("mult "+mult);
			KoreKrush.Events.Logic.SpeedMultiply (mult);			
		}

	}

}
