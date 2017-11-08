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
		KoreKrush.Events.Logic.TilesMotorManage += OnTilesProcessing;
	}
	void OnDestroy()
	{
		KoreKrush.Events.Logic.TilesMotorManage -= OnTilesProcessing;
	}
	// Use this for initialization
	void Start () {
        fill_counter = 0;
	}	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTilesProcessing(TileType plist, int count, int totalCount,  bool isWarp)
	{
        fill_counter += count;
        if (!isWarp)
        {
            float mult = count * m_Motor.Multiplier * LocalHelper.Multiplier(totalCount);
            KoreKrush.Events.Logic.SpeedMultiply(mult);
        }

        CheckIsFilled();
	}

    private void CheckIsFilled()
    {
        if(fill_counter >= m_Motor.PowerFillCount)
        {
            if (m_Motor.TileGenerated != null)
            {
                KoreKrush.Events.Logic.MotorTileSpawn(m_Motor.TileGenerated);
                fill_counter = 0;
            }
        }
    }
}
