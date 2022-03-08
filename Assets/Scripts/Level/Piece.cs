using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shared.Defines;
using Shared.Utils;
using Shared.DirUtils;

public class Piece : MonoBehaviour
{
    List<PieceTurn> currentTurnActions = new List<PieceTurn>();
    
    // If the piece can be 
    bool replaceable;
    
    // If the piece is REPLACEABLE then this is the position when the pice is outside the grid
    // If the piece is NOT REPLACEABLE then this is the grid position 
    // where the piece is before the game execution starts
    Vector2 startPos;

    // is NULL when the Pieces isn NOT on the GRID
    // This is always the position on the grid the piece has after completing the current turn
    public Vector2Int? gridPos {get; protected set;}
    public Direction movingDir {get; protected set;} = Direction.None;

    // A Piece can get broken if it collides with an other Piece or gets destroy by
    // an obstacle (thats maybe comming in the future)
    public bool broken {get; protected set;} = false;

    bool isDraged = false;

    // Variabels to return to the state before the execution
    Vector2Int? preExecutionPos; // Null means that the piece wasn't on grid at executon time
    Direction preExecutionDir;

    
    // This is the target position of the last turn
    public Vector2Int prevGridPos {get; protected set;} = new Vector2Int(-1, -1);
    public Direction prevMovingDir {get; protected set;} = Direction.None;


    public void Init(bool isReplaceable, Vector2 pos, Direction dir)
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
            gridPos = Utils.RoundVec2(pos);
            startPos = pos;
            SetTransform(position: pos, rotation: DirUtils.DirInAngle(dir));
        }

    }


    //// UNITY METHODS ////

    void Update()
    {
        // Execute movement
        if (gridPos != null && GameManager.Instance.CurrentGameState == GameState.Execute)
        {
            // Execute the current turn action
            foreach(PieceTurn pt in currentTurnActions)
            {
                if (pt != null)
                {
                    pt.Update(gameObject);
                }
            }
        }

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

    void OnMouseDown()
    {
        if (replaceable && GameManager.Instance.CurrentGameState == GameState.Building)
        {
            StartDrag();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Gets called when colliding with a object with a collider
        // Make piece broken when colliding with another piece
        Piece p = other.gameObject.GetComponent<Piece>();
        if (p != null)
        {
            SetBroken();
        }
    }


    //// MOVEMENT ////

    public void CalculateNextTurn()
    {
        currentTurnActions = new List<PieceTurn>();
        
        // When outside grid then no turn needs the be calculated
        if (gridPos == null) return;

        // If the piece is broken down then don't move anymore
        if (broken == true) return;
        
        // Only because the var name is shorter
        Vector2Int currentGridPos = gridPos.GetValueOrDefault(new Vector2Int(-1, -1));
        
        Vector2Int newGridPos = currentGridPos;
        Direction newMovingDir = movingDir;
        
        // 'currentGridPos' is the position the piece is currently on
        // so this is the tile the piece is on
        TileBase tile = GameManager.Instance.GetTile(currentGridPos);

        // This can be used as a variabe to store a possible newGridPos
        // until it can be verrified that this is the newGridPos
        Vector2Int possiblePos;
        
        // WHEN THERE IS NO TILE GO STRAIGHT
        if (tile == null)
        {
            possiblePos = DirUtils.NextPosInDir(movingDir, currentGridPos);
            if (CanMoveToGridPos(possiblePos))
            {
                // Animation
                newGridPos = possiblePos;
                currentTurnActions.Add(new PieceTranslate(
                    GameManager.Instance.TurnTime, currentGridPos, newGridPos
                ));
            }
        }
        else
        {
            // SPECIAL TILE TYPES
            switch (tile.Type)
            {
                // PIECE TARGET TILE -> stay here if this is the correct direction when not just go on
                case TileType.PieceTarget:
                    if (((TilePieceTarget)tile).pieceTargetDir != movingDir)
                    {
                        possiblePos = DirUtils.NextPosInDir(movingDir, currentGridPos);
                        if (CanMoveToGridPos(possiblePos))
                        {
                            // Animation
                            newGridPos = possiblePos;
                            currentTurnActions.Add(new PieceTranslate(
                                GameManager.Instance.TurnTime, currentGridPos, newGridPos
                            ));
                        }
                    }
                break;

                // REDIRECT TILE -> rotate and translate in one move
                case TileType.Redirect:
                    // Rotation should happen everytime
                    newMovingDir = ((TileRedirect)tile).redirectionDir;
                    currentTurnActions.Add(new PieceRotate(
                        GameManager.Instance.TurnTime/7,
                        DirUtils.DirInAngle(movingDir),
                        DirUtils.DirInAngle(newMovingDir)
                    ));
                    
                    // Translation depends on the tiles
                    possiblePos = DirUtils.NextPosInDir(newMovingDir, currentGridPos);
                    if (CanMoveToGridPos(possiblePos))
                    {
                        newGridPos = possiblePos;
                        currentTurnActions.Add(new PieceTranslate(
                            GameManager.Instance.TurnTime, currentGridPos, newGridPos
                        ));

                    }
                break;

                // TELEPORT TILE -> chnage pos to the teleport tile in the tile to piece is on
                // If the other tile isn't on the grid or just is not there then don't change pos
                // When the tile on the prev tile is exactly the other tile from this teleport tile
                // that means the pice got teleported lat turn should not port again this turn
                case TileType.Teleport:
                    TileBase prevTile = GameManager.Instance.GetTile(prevGridPos);
                    if (((TileTeleport)tile).GetOtherTile() == prevTile)
                    {
                        // Just try to go straight
                        possiblePos = DirUtils.NextPosInDir(movingDir, currentGridPos);
                        if (CanMoveToGridPos(possiblePos))
                        {
                            // Animation
                            newGridPos = possiblePos;
                            currentTurnActions.Add(new PieceTranslate(
                                GameManager.Instance.TurnTime, currentGridPos, newGridPos
                            ));
                        }
                    }
                    else
                    {
                        // Try teleport
                        Vector2Int? otherPos = ((TileTeleport)tile).GetOtherTilePos();
                        if (otherPos != null)
                        {
                                newGridPos = otherPos.GetValueOrDefault(new Vector2Int(-1, -1));
                        }
                        else
                        {
                            newGridPos = currentGridPos;
                        }

                        // Animation
                        currentTurnActions.Add(new PieceTeleport(
                            GameManager.Instance.TurnTime, currentGridPos, newGridPos, Vector2.one, Vector2.one/4
                        ));
                    }
                break;
            }
        }

        // Set the positions for the new turn
        gridPos = currentGridPos;
        prevGridPos = gridPos.GetValueOrDefault(new Vector2Int(-1, -1));
        gridPos = newGridPos;
        
        prevMovingDir = movingDir;
        movingDir = newMovingDir;
    }


    bool CanMoveToGridPos(Vector2Int pos)
    {
        // This checks if it is possible that the pice can move on this tile.
        // A Piece can't move on a tile if it is solid or there is no tile

        // ONLY TAKES IN ACCOUNT TILES AND NOT OTHER PIECES
        if (GameManager.Instance.IsPosOnGrid(pos))
        {
            TileBase tile = GameManager.Instance.GetTile(pos);
            if (tile != null)
            {
                if (tile.Type == TileType.Solid)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }
        return false;
    }


    public bool HasReachedTarget()
    {
        // If this piece in on the correct target tile
        if (gridPos == null) return false;
        TileBase t = GameManager.Instance.GetTile(gridPos.GetValueOrDefault(new Vector2Int(-1, -1)));
        if (t != null)
        {
            if (t.Type == TileType.PieceTarget)
            {
                if (((TilePieceTarget)t).pieceTargetDir == movingDir)
                {
                    return true;
                }
            }
        }
        return false;
    }

    //// GAME EXECUTION ////

    public void PrepareExecution()
    {
        // Gets called from GameManager
        
        // Save the current pos/dir to return to after the execution
        preExecutionPos = gridPos;
        preExecutionDir = movingDir;
    }

    public void ResetAfterExecution()
    {
        // Gets called after execution by the GameManager

        // Delete turn actions
        currentTurnActions = new List<PieceTurn>();

        // Return to the state before the execution
        gridPos = preExecutionPos;
        movingDir = preExecutionDir;
        SetTransform(position: ((Vector2?)gridPos).GetValueOrDefault(startPos), rotation: DirUtils.DirInAngle(movingDir), scale: Vector2.one);

        // Make the piece unbroken if it might have beed broken
        SetUnbroken();
    }


    //// HANDLE BROKEN PIECE ////
    
    void SetBroken()
    {
        if (broken == false)
        {
            // TODO Chnage sprite
            broken = true;
            
            // Stop all animations
            currentTurnActions = new List<PieceTurn>();
        }
    }

    void SetUnbroken()
    {
        if (broken == true)
        {
            // TODO Change sprite
            broken = false;
        }
    }

    //// DRAG METHODS ////

    void StartDrag()
    {
        // When this gets called from 'OnMouseDown'
        // it's already checked if drag is possible
        isDraged = true;
        gridPos = null; // While drag piece is not on grid
    }

    void WhileDrag()
    {
        // Go to mouse position
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 target = new Vector2(mousePos.x, mousePos.y);
        SetTransform(position: target, useDragOffset: true);
    }

    void EndDrag()
    {
        isDraged = false;

        // Figure out if the current position is on the grid or not
        // And if on grid if the gridPos is already taken by a 
        // tile or a piece
        Vector2Int pos = new Vector2Int(
            Mathf.RoundToInt(transform.localPosition.x),
            Mathf.RoundToInt(transform.localPosition.y)
        );
        if (GameManager.Instance.IsPosOnGrid(pos))
        {
            // Is in grid
            if (!GameManager.Instance.IsGridPosOccupied(pos))
            {
                // Piece can move onto grid
                gridPos = pos;
                SetTransform(position: pos);
                return;
            }
        }

        // If here is reached that means that the piece doesn't get placed
        // on the grid and moves to its start position
        // The start position must be outside the grid because a non replaceable piece wouldnt be 
        // draged at first
        gridPos = null; // is not on grid
        SetTransform(position: startPos);
    }


    //// OTHER ////

    void SetTransform(Vector2? position = null, float? rotation = null, Vector2? scale = null, bool useDragOffset = false)
    {
        // Everything is in local coords relative to the parent object that holds 
        // all the pieces

        // Position
        // z=0 because the correct z value for this layer is set in the parent object
        // (execpt the drag offset is used so that the piece is in fromt of everything while dragging)
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

