using UnityEngine;
using Shared.Defines;
using Shared.DirUtils;

public abstract class TileBase : MonoBehaviour
{
    public TileType Type {get; protected set;}

    // If the tile can be replaced by the user
    protected bool replaceable;

    // If the grid Pos is currently null that means the tiles isn't on the grid
    public Vector2Int? gridPos {get; protected set;}

    // This is either the place outside the grid (replaceable) or the gridPos (not replaceable)
    protected Vector2Int startPos;

    protected bool isDraged = false;

    // CREATE 'Init' Method in derived class
    // CALL THIS FROM 'Init'
    protected void BaseInit(bool isReplaceable, Vector2Int pos, TileType type)
    {
        // 'pos' argument is either the the pos outside (is replaceable) the grid 
        // or inside the grid (not replaceable)

        Type = type;
        replaceable = isReplaceable;

        if (replaceable)
        {
            gridPos = null;
            startPos = pos;
            SetTransform(position: pos);
        }
        else
        {
            gridPos = pos;
            startPos = pos;
            SetTransform(position: pos);
        }
    }

    protected void Update()
    {
        // Call drag
        if (isDraged)
        {
            WhileDrag();
        }

        // Check if drag should be ended
        if (isDraged && !Input.GetMouseButton(0))
        {
            EndDrag();
        }

    }

    protected void OnMouseDown()
    {
        if (replaceable && GameManager.Instance.CurrentGameState == GameState.Building)
        {
            StartDrag();
        }
    }


    //// DRAG METHODS ////

    protected void StartDrag()
    {
        // When this gets called from 'OnMouseDown'
        // it's already checked if drag is possible
        isDraged = true;
        gridPos = null; // While drag tile is not on grid
    }

    protected void WhileDrag()
    {
        // Go to mouse position
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 target = new Vector2(mousePos.x, mousePos.y);
        SetTransform(position: target, useDragOffset: true);
    }

    protected void EndDrag()
    {
        isDraged = false;

        // Figure out if the current position is on the grid or not
        Vector2Int pos = new Vector2Int(
            Mathf.RoundToInt(transform.localPosition.x),
            Mathf.RoundToInt(transform.localPosition.y)
        );
        if (GameManager.Instance.IsPosOnGrid(pos))
        {
            if (!GameManager.Instance.IsGridPosOccupied(pos))
            {
                // Tile can move onto grid
                gridPos = pos;
                SetTransform(position: pos);
                return;
            }
        }

        // If here is reached that means that the tile doesn't get placed
        // on the grid and moves to its start position
        // The start position must be outside the grid because a non replaceable tile wouldnt be 
        // draged at first
        gridPos = null; // is not on grid
        SetTransform(position: startPos);
    }

    
    protected void SetTransform(Vector2? position = null, float? rotation = null, Vector2? scale = null, bool useDragOffset = false)
    {
        // Everything is in local coords relative to the parent object that holds 
        // all the tiles

        // Position
        // z=0 because the correct z value for this layer is set in the parent object
        // (execpt the drag offset is used so that the tile is in fromt of everything while dragging)
        if (position != null)
        {
            Vector2 p = position.GetValueOrDefault(new Vector2(-1, -1));
            float z;
            if (useDragOffset) z = Layer.DragOffset;
            else z = 0;
            transform.localPosition = new Vector3(p.x, p.y, z);
        }

        // Rotation
        if (rotation != null)
        {
            transform.localEulerAngles = new Vector3(0, 0, rotation.GetValueOrDefault(0));
        }

        // Scale
        if (scale != null)
        {
            Vector2 s = scale.GetValueOrDefault(Vector2.one);
            transform.localScale = new Vector3(s.x, s.y, 1);
        }
    }
}
