using System;
using System.Collections;
using System.Collections.Generic;
using KoreKrush;
using UnityEngine;


public abstract class ColoredTile : BaseTile
{
    public override bool IsCompatible(BaseTile other)
    {
        var o = other as ColoredTile;
        return o && TileType == o.TileType;
    }
}
