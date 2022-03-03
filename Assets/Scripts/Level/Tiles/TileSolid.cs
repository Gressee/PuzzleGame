using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shared.Defines;

public class TileSolid : TileBase
{
    public void Init(Vector2Int gridPos)
    {
        BaseInit(gridPos, TileType.Solid);
    }

}
