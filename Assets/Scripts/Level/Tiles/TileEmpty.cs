using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shared.Defines;

public class TileEmpty : TileBase
{
    
    public void Init(bool isReplaceable, Vector2Int gridPos)
    {
        BaseInit(isReplaceable, gridPos, TileType.Empty);
    }
}
