using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shared.Defines;

public class TileTeleport : TileBase
{

    [SerializeField]
    TileTeleport counterPartTile;

    public void Init(bool isReplaceable, Vector2Int gridPos)
    {
        BaseInit(isReplaceable, gridPos, TileType.Teleport);

        if (counterPartTile == null)
        {
            Debug.LogError("Tile to teleport to is NULL");
        }
    }

    public TileTeleport GetOtherTile()
    {
        return counterPartTile;
    }

    public Vector2Int? GetOtherTilePos()
    {
        // This returns the position of the other tile if it exists
        if (counterPartTile != null)
        {
            return counterPartTile.gridPos;
        }

        return null;
    }
}
