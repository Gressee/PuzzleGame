using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shared.Defines;

public class TileEmpty : TileBase
{
    public virtual void Init(Vector2Int gridPos)
    {
        base.Init(gridPos, TileType.Empty, Layer.Tiles);
    }
}
