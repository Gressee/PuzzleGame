using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shared.Defines;
using Shared.DirUtils;

public class TileRedirect : TileBase
{

    public Direction redirectionDir {get; protected set;}
    
    public void Init(bool isReplaceable, Vector2 gridPos, Direction dir)
    {
        BaseInit(isReplaceable, gridPos, TileType.Redirect);
        redirectionDir = dir;
        SetTransform(rotation: DirUtils.DirInAngle(redirectionDir));
    }
}
