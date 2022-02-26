using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shared.Defines;

public class TilePieceTarget : TileBase
{
    public Direction pieceTargetDir {get; protected set;}

    public virtual void Init(Vector2Int gridPos, Direction targetDir)
    {
        base.Init(gridPos, TileType.PieceTarget, Layer.Tiles);
        pieceTargetDir = targetDir;
    }
}
