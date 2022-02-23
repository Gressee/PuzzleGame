using UnityEngine;
using Shared.Defines;

public abstract class TileBase : MonoBehaviour
{
    public TileType Type {get; protected set;} = TileType.Base;

    protected Vector2Int gridPos;

    public virtual void Init(Vector2Int gridPos, TileType type, float layer)
    {
        // Use 'virtual' and 'base.Init' for the 'Init' method in the derived class
        // See: https://stackoverflow.com/a/53097853

        // Should always get called after creation of the object
        
        this.gridPos = gridPos;
        transform.position = new Vector3(gridPos.x, gridPos.y, layer);

        this.Type = type;
    }
}
