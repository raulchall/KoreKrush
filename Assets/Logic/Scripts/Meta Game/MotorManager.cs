using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


using KoreKrush;

public class MotorManager : MonoBehaviour {

    public Motor m_Motor;

    int fill_counter;

	void Awake()
	{
		KoreKrush.Events.Logic.ManageSpeed += OnTilesProcessing;
	}
	void OnDestroy()
	{
		KoreKrush.Events.Logic.ManageSpeed -= OnTilesProcessing;
	}
	// Use this for initialization
	void Start () {
        fill_counter = 0;
	}	
	// Update is called once per frame
	void Update () {
		
	}

	//TODO: hacer esto mas eficiente, o sea que a un motor solo le lleguen los elementos que quiere procesar
	void OnTilesProcessing(PieceList plist)
	{
		if (plist.ContainsKey(m_Motor.Tile))
		{
            fill_counter += plist[m_Motor.Tile];
            float mult = plist[m_Motor.Tile] * m_Motor.Multiplier * LocalHelper.Multiplier(plist.Count);
			KoreKrush.Events.Logic.SpeedMultiply (mult);
		}

        CheckIsFilled();
	}

    private void CheckIsFilled()
    {
        if(fill_counter >= m_Motor.PowerFillCount)
        {
            if (m_Motor.TileGenerated != null)
            {
                //Josue instancia tu prefab!!!
                fill_counter = 0;
            }
        }
    }
}
