using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shared.Defines;
using Shared.DirUtils;

public class Piece : MonoBehaviour
{
    // A pice can be placed on the grid then its moving when the game is executing
    // If the piece still needs to get placed on the grid by the player then it's not
    // moving on execution
    bool onGrid;

    // If the piece can be 
    bool replaceable;

    int currentTurnNum = -1;

    List<PieceTurn> currentTurnActions = new List<PieceTurn>();


    // This is always the position on the grid the piece has after completing the 
    // current turn
    Vector2Int gridPos = new Vector2Int(-1,-1);
    // This is the target position of the last turn
    Vector2Int prevGridPos = new Vector2Int(-1, -1);

    Direction movingDir = Direction.None;
    Direction prevMovingDir = Direction.None;

    public void Init(Direction dir, Vector2Int? gridPos = null, Vector2? offGridPos = null)
    {
        // Has to be called after creatig object
        
        movingDir = dir;

        // DON'T set 'gridPos' and 'offGridPos'
        if (gridPos != null && offGridPos != null)
        {
            Debug.LogError("Piece.Init should only use position one argument");
        }

        // If 'gridPos' is given it assumes that the player cant 
        // drag the piece arround and replace it
        if (gridPos != null)
        {
            this.gridPos = gridPos.GetValueOrDefault(new Vector2Int(-1,-1));
            SetWorldCoords(this.gridPos.x, this.gridPos.y);

            replaceable = false;
            onGrid = true;
        }

        if (offGridPos != null)
        {
            Vector2 p = offGridPos.GetValueOrDefault(new Vector2(-1, -1));
            SetWorldCoords(p.x, p.y);

            replaceable = true;
            onGrid = false;
        }
    }


    void Update()
    {
        if (GameManager.Instance.CurrentGameState == GameState.Execute)
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
    }


    void CalculateNextTurn()
    {
        currentTurnActions = new List<PieceTurn>();
        
        Vector2Int newGridPos = gridPos;
        Direction newMovingDir = movingDir;
        
        /*
        currentTurnActions.Add(new PieceTranslate(
            GameManager.Instance.TurnTime,
            gridPos, newGridPos
        ));
        */

        // 'gridPos' is the position the piece is currently on
        // so this is the tile the piece is on
        TileBase tile = GameManager.Instance.GetTile(gridPos);

        if (tile == null)
        {
            Debug.LogError("Tile the piece is on is null");
            return;
        }

        switch (tile.Type)
        {
            // EMPTY TILE -> just move in same direction
            case TileType.Empty:
                newGridPos = DirUtils.NextPosInDir(movingDir, gridPos);
                
                // Animation
                currentTurnActions.Add(new PieceTranslate(
                    GameManager.Instance.TurnTime, gridPos, newGridPos
                ));
            break;

            // PIECE TARGET TILE -> Just stay here if this is the correct direction when not just go on
            case TileType.PieceTarget:
                if (((TilePieceTarget)tile).pieceTargetDir != movingDir)
                {
                    newGridPos = DirUtils.NextPosInDir(movingDir, gridPos);
                
                    // Animation
                    currentTurnActions.Add(new PieceTranslate(
                        GameManager.Instance.TurnTime, gridPos, newGridPos
                    ));
                }
            break;

            // REDIRECT TILE -> rotate and translate in one move
            case TileType.Redirect:
                newMovingDir = ((TileRedirect)tile).RedirectionDir;
                newGridPos = DirUtils.NextPosInDir(newMovingDir, gridPos);
                
                // Animation
                currentTurnActions.Add(new PieceTranslate(
                    GameManager.Instance.TurnTime, gridPos, newGridPos
                ));

                currentTurnActions.Add(new PieceRotate(
                    GameManager.Instance.TurnTime/7,
                    DirUtils.DirInAngle(movingDir),
                    DirUtils.DirInAngle(newMovingDir)
                ));
            break;
        }

        // Set the positions for the new turn
        prevGridPos = gridPos;
        gridPos = newGridPos;

        prevMovingDir = movingDir;
        movingDir = newMovingDir;
    }


    void SetWorldCoords(float x, float y)
    {
        // Needed to ensure that the z coord is always the layer of the object
        transform.position = new Vector3(x, y, Layer.Pieces);
    }
}





//// PIECE TURN CLASSES ////

abstract class PieceTurn
{
    protected float duration;
    protected float startTime;

    public PieceTurn(float duration)
    {
        startTime = Time.time;
        this.duration = duration;
    }

    public virtual void Update(GameObject gameObject)
    {
        // REMEMBER TO CALL THIS TO ACUTALLY UPDATE THE GAMEOBJECT
        // Update GameObject like this
        /*
        gameObject.transform.position = pos;
        gameObject.transform.eulerAngles = rot; // TODO Check if this works 
        gameObject.transform.localScale = scale;
        */
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

    // TODO finish this class
    public PieceTranslate(float duration, Vector2 startPos, Vector2 targetPos) : base(duration)
    {   
        this.startPos = startPos;
        this.targetPos = targetPos;
    }

    public override void Update(GameObject gameObject)
    {
        // Check if the duration time is already reched
        if (Time.time - startTime < duration)
        {
            Vector2 dir = targetPos - startPos;
            Vector3 p = startPos + dir * (Time.time - startTime)/duration;
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
        if (Time.time -  startTime < duration/2)
        {
            if (!setStartPos)
            {
                setStartPos = true;
                Vector3 p = startPos;
                p.z = Layer.Pieces;
                gameObject.transform.position = p;
            }
            Vector2 scaleDir = minScale - maxScale;
            gameObject.transform.localScale = maxScale + scaleDir * (Time.time - startTime)/(duration/2);
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
            gameObject.transform.localScale = minScale + scaleDir * (Time.time - startTime - duration/2)/(duration/2);
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
        if (Time.time - startTime < duration)
        {   
            // TODO Fix that rotation can loop arround For example from 0 to 270
            /*
            float rotDir = targetRot - startRot;
            float r = startRot + rotDir * (Time.time - startTime)/duration;
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
            float newAngle = angleToTurn/duration * (Time.time - startTime);
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

