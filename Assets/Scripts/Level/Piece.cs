using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shared.Defines;
using Shared.DirUtils;

public class Piece : MonoBehaviour
{
    int currentTurnNum = -1;
    List<PieceTurn> currentTurnActions = new List<PieceTurn>();
    
    // If the piece can be 
    bool replaceable;
    
    // If the piece is REPLACEABLE then this is the position when the pice is outside the grid
    // If the piece is NOT REPLACEABLE then this is the grid position 
    // where the piece is before the game execution starts
    Vector2Int startPos;

    // is NULL when the Pieces isn NOT on the GRID
    // This is always the position on the grid the piece has after completing the current turn
    public Vector2Int? gridPos {get; protected set;}


    bool isDraged = false;

    
    // This is the target position of the last turn
    // This is needed for the rotation tile so pices doesn't spinn indefinetly
    Vector2Int prevGridPos = new Vector2Int(-1, -1);

    Direction movingDir = Direction.None;
    Direction prevMovingDir = Direction.None;

    public void Init(bool isReplaceable, Vector2Int pos, Direction dir)
    {
        // Has to be called after creatig object

        // 'pos' argument is either the the pos outside (is replaceable) the grid 
        // or inside the grid (not replaceable)

        movingDir = dir;
        replaceable = isReplaceable;

        if (replaceable)
        {
            gridPos = null;
            startPos = pos;
            SetTransform(position: pos, rotation: DirUtils.DirInAngle(dir));
        }
        else
        {
            gridPos = pos;
            startPos = pos;
            SetTransform(position: pos, rotation: DirUtils.DirInAngle(dir));
        }

    }


    void Update()
    {
        // Execute movement
        if (gridPos != null && GameManager.Instance.CurrentGameState == GameState.Execute)
        {
            // Check if the GameManager is on the next turn
            // If yes calculate the next turn
            if (currentTurnNum != GameManager.Instance.CurrentTurn)
            {
                currentTurnNum = GameManager.Instance.CurrentTurn;
                CalculateNextTurn();
            }

            // Execute the current turn action
            foreach(PieceTurn pt in currentTurnActions)
            {
                if (pt != null)
                {
                    pt.Update(gameObject);
                }
            }
        }

        // Check if drag should be ended
        if (isDraged && Input.GetMouseButtonUp(0))
        {
            EndDrag();
        }
    }

    void OnMouseDown()
    {
        if (replaceable && GameManager.Instance.CurrentGameState == GameState.Building)
        {
            StartDrag();
        }
    }


    void CalculateNextTurn()
    {
        currentTurnActions = new List<PieceTurn>();
        
        // When outside grid then no turn needs the be calculated
        if (gridPos == null) return;
        
        // Only because the var name is shorter
        Vector2Int currentGridPos = gridPos.GetValueOrDefault(new Vector2Int(-1, -1));
        
        Vector2Int newGridPos = currentGridPos;
        Direction newMovingDir = movingDir;
        
        // 'currentGridPos' is the position the piece is currently on
        // so this is the tile the piece is on
        TileBase tile = GameManager.Instance.GetTile(currentGridPos);

        if (tile == null)
        {
            Debug.LogWarning("Tile the piece is on is null");
            return;
        }

        switch (tile.Type)
        {
            // EMPTY TILE -> just move in same direction
            case TileType.Empty:
                newGridPos = DirUtils.NextPosInDir(movingDir, currentGridPos);
                
                // Animation
                currentTurnActions.Add(new PieceTranslate(
                    GameManager.Instance.TurnTime, currentGridPos, newGridPos
                ));
            break;

            // PIECE TARGET TILE -> Just stay here if this is the correct direction when not just go on
            case TileType.PieceTarget:
                if (((TilePieceTarget)tile).pieceTargetDir != movingDir)
                {
                    newGridPos = DirUtils.NextPosInDir(movingDir, currentGridPos);
                
                    // Animation
                    currentTurnActions.Add(new PieceTranslate(
                        GameManager.Instance.TurnTime, currentGridPos, newGridPos
                    ));
                }
            break;

            // REDIRECT TILE -> rotate and translate in one move
            case TileType.Redirect:
                newMovingDir = ((TileRedirect)tile).redirectionDir;
                newGridPos = DirUtils.NextPosInDir(newMovingDir, currentGridPos);
                
                // Animation
                currentTurnActions.Add(new PieceTranslate(
                    GameManager.Instance.TurnTime, currentGridPos, newGridPos
                ));

                currentTurnActions.Add(new PieceRotate(
                    GameManager.Instance.TurnTime/7,
                    DirUtils.DirInAngle(movingDir),
                    DirUtils.DirInAngle(newMovingDir)
                ));
            break;
        }

        // Set the positions for the new turn
        gridPos = currentGridPos;
        prevGridPos = gridPos.GetValueOrDefault(new Vector2Int(-1, -1));
        gridPos = newGridPos;
        
        prevMovingDir = movingDir;
        movingDir = newMovingDir;
    }


    //// DRAG METHODS ////

    void StartDrag()
    {

    }

    void WhileDrag()
    {

    }

    void EndDrag()
    {
        // TODO call this from the GameManager before the Game Execute gets 
        // started so all pices have a well defined place
    }


    //// OTHER ////

    void SetTransform(Vector2? position = null, float? rotation = null, Vector2? scale = null)
    {
        // Everything is in local coords relative to the parent object that holds 
        // all the pieces

        // Position
        // z=0 because the correct z value for this layer is set in the parent object
        if (position != null)
        {
            Vector2 p = position.GetValueOrDefault(new Vector2(-1, -1));
            transform.localPosition = new Vector3(p.x, p.y, 0);
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





//// PIECE TURN CLASSES ////

abstract class PieceTurn
{
    protected float duration;
    protected float startTime;

    // TODO all derived classes should not interact with the gameobject transform
    // directly and instead use the method in the piece call for manipulation the
    // transform because of layers ...
    // (see todo in piece)

    public PieceTurn(float duration)
    {
        this.duration = duration;
        // Because this animation starts always at the beginning of a turn
        startTime = GameManager.Instance.timeSinceLastTurn;
    }

    public virtual void Update(GameObject gameObject)
    {
        // REMEMBER TO CALL THIS TO ACUTALLY UPDATE THE GAMEOBJECT

        // Use:
        //  (GameManager.Instance.timeSinceLastTurn - startTime)
        // to get how much time has passed since the start of the turn
    }

    public virtual void SetTargetValues(GameObject gameObject)
    {
        // This can be used to make sure the values on the 
        // unity gameObject are exactly the target ones
        // and not a bit more or less because calculations 
        // with the frame time can have slightly different results
    }
}


class PieceTranslate: PieceTurn
{

    Vector2 startPos, targetPos;

    public PieceTranslate(float duration, Vector2 startPos, Vector2 targetPos) : base(duration)
    {   
        this.startPos = startPos;
        this.targetPos = targetPos;
    }

    public override void Update(GameObject gameObject)
    {
        // Check if the duration time is already reched
        if (GameManager.Instance.timeSinceLastTurn - startTime < duration)
        {
            Vector2 dir = targetPos - startPos;
            Vector3 p = startPos + dir * (GameManager.Instance.timeSinceLastTurn - startTime)/duration;
            p.z = Layer.Pieces;
            gameObject.transform.position = p;
        }
        else
        {
            SetTargetValues(gameObject);
        }
    }

    public override void SetTargetValues(GameObject gameObject)
    {
        Vector3 p = targetPos;
        p.z = Layer.Pieces;
        gameObject.transform.position = p;
    }
}


class PieceTeleport: PieceTurn
{
    bool setStartPos = false;
    bool changedPos = false;

    Vector2 startPos, targetPos;
    Vector2 maxScale, minScale;

    public PieceTeleport(float duration, Vector2 startPos, Vector2 targetPos, Vector2 maxScale, Vector2 minScale) :  base(duration)
    {
        this.startPos = startPos;
        this.targetPos = targetPos;

        this.maxScale = maxScale;
        this.minScale = minScale;
    }

    public override void Update(GameObject gameObject)
    {
        // Getting smaller
        if (GameManager.Instance.timeSinceLastTurn -  startTime < duration/2)
        {
            if (!setStartPos)
            {
                setStartPos = true;
                Vector3 p = startPos;
                p.z = Layer.Pieces;
                gameObject.transform.position = p;
            }
            Vector2 scaleDir = minScale - maxScale;
            gameObject.transform.localScale = maxScale + scaleDir * (GameManager.Instance.timeSinceLastTurn - startTime)/(duration/2);
        }
        // Getting bigger again
        else
        {
            if (!changedPos)
            {
                changedPos = true;
                Vector3 p = targetPos;
                p.z = Layer.Pieces;
                gameObject.transform.position = p;
            }
            Vector2 scaleDir = maxScale - minScale;
            gameObject.transform.localScale = minScale + scaleDir * (GameManager.Instance.timeSinceLastTurn - startTime - duration/2)/(duration/2);
        }

    }
}


class PieceRotate: PieceTurn
{
    float startRot, targetRot;

    public PieceRotate(float duration, float startRot, float targetRot) : base(duration)
    {
        this.startRot = startRot;
        this.targetRot = targetRot;
    }

    public override void Update(GameObject gameObject)
    {
        // Check if the duration time is already reched
        if (GameManager.Instance.timeSinceLastTurn - startTime < duration)
        {   
            /*
            float rotDir = targetRot - startRot;
            float r = startRot + rotDir * (GameManager.Instance.timeSinceLastTurn - startTime)/duration;
            gameObject.transform.eulerAngles = new Vector3(0, 0, r);
            */
            float angleToTurn;
            if (Mathf.Abs(targetRot-startRot) > 180)
            {
                angleToTurn = -(360 - (targetRot-startRot));
            }
            else
            {
                angleToTurn = targetRot-startRot;
            }
            float newAngle = angleToTurn/duration * (GameManager.Instance.timeSinceLastTurn - startTime);
            gameObject.transform.eulerAngles = new Vector3(0, 0, newAngle);
        }
        else
        {
            SetTargetValues(gameObject);
        }
    }
    
    public override void SetTargetValues(GameObject gameObject)
    {
        gameObject.transform.eulerAngles = new Vector3(0, 0, targetRot);
    }
}

