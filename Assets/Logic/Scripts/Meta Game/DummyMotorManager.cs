using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using KoreKrush;

public class DummyMotorManager : MonoBehaviour {

    public TileType m_Tile;
    public float speedDivider = 5;

    void Awake()
    {
        KoreKrush.Events.Logic.TilesMotorManage += OnTilesProcessing;
    }
    // Destroy all events links
    void OnDestroy()
    {
        KoreKrush.Events.Logic.TilesMotorManage -= OnTilesProcessing;
    }

    void OnTilesProcessing(TileType tile, int count, int totalCount, bool isWarp)
    {
        if(m_Tile == tile)
        {
            if (!isWarp)
            {
                float mult = (count * LocalHelpers.Multiplier(totalCount))/speedDivider;
                KoreKrush.Events.Logic.SpeedMultiply(mult);
            }
        }
    }

}
