using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shared.Defines;

public class TileSolid : TileBase
{
    public void Init(bool isReplaceable, Vector2 gridPos)
    {
        BaseInit(isReplaceable, gridPos, TileType.Solid);
    }

}
