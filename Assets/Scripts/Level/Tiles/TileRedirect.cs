using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shared.Defines;

public class TileRedirect : TileBase
{

    public Direction redirectionDir {get; protected set;}
    
    public void Init(Vector2Int gridPos, Direction dir)
    {
        BaseInit(gridPos, TileType.Redirect);
        redirectionDir = dir;
        SetRotation(redirectionDir);
    }
}
