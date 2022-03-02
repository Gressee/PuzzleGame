using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shared.Defines;

public class TilePieceTarget : TileBase
{
    public Direction pieceTargetDir {get; protected set;}

    public void Init(Vector2Int gridPos, Direction targetDir)
    {
        BaseInit(gridPos, TileType.PieceTarget);
        pieceTargetDir = targetDir;
        SetRotation(pieceTargetDir);
    }
}
