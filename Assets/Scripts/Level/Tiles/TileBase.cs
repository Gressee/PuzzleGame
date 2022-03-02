using UnityEngine;
using Shared.Defines;
using Shared.DirUtils;

public abstract class TileBase : MonoBehaviour
{
    public TileType Type {get; protected set;}

    protected Vector2Int gridPos;

    // CREATE 'Init' Method in derived class
    // CALL THIS FROM 'Init'
    protected void BaseInit(Vector2Int gridPos, TileType type)
    {
        this.gridPos = gridPos;
        Type = type;
    }

    protected void SetRotation(Direction dir)
    {
        Vector3 r = transform.localEulerAngles;
        r.z = DirUtils.DirInAngle(dir);
        transform.localEulerAngles = r;
    }
}
