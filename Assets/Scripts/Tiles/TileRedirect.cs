using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shared.Defines;

public class TileRedirect : TileBase
{
    public Direction RedirectionDir {get; protected set;}
    public virtual void Init(Vector2Int gridPos, Direction dir)
    {
        base.Init(gridPos, TileType.Redirect, Layer.Tiles);
        RedirectionDir = dir;
    }
}
