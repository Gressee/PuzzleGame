using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shared.Defines;
using Shared.DirUtils;

public class TilePieceTarget : TileBase
{
    public Direction pieceTargetDir {get; protected set;}

    public void Init(bool isReplaceable, Vector2Int gridPos, Direction targetDir)
    {
        BaseInit(isReplaceable, gridPos, TileType.PieceTarget);
        pieceTargetDir = targetDir;
        SetTransform(rotation: DirUtils.DirInAngle(pieceTargetDir));
    }
}
