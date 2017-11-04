using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UniversalTile : BaseTile
{
    public override bool IsCompatible(BaseTile other)
    {
        return true;
    }
}
