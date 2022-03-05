using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Core.Singleton;
using Shared.Defines;
using Shared.DirUtils;

public class GameManager : Singleton<GameManager>
{

    [SerializeField]
    TileBase tileEmptyPrefab, tileRedirectPrefab, tilePieceTargetPrefab, tileTeleportPrefab;

    [SerializeField]
    Piece piecePrefab;

    public GameState CurrentGameState {get; protected set;} = GameState.Building;


    // A Turn is for example when a piece translates from one to the next tile
    // The times it takes to execuite on Turn
    public float TurnTime {get; protected set;} = 1.0f;
    // When in 'Execute' State this gets increased according to the 'TurnTime'
    // Pieces use this var to calc when they have to be where and in what orientation
    public int CurrentTurn {get; protected set;} = 0;

    // How much time has passed since the last turn has been started
    public float timeSinceLastTurn {get; protected set;} = 0.0f;

    // Level
    List<TileBase> tiles = new List<TileBase>();
    List<Piece> pieces = new List<Piece>();
    public int gridWidth  {get; protected set;} = 0;
    public int gridHeight {get; protected set;} = 0;


    void Start()
    {
        ReadLevel();
    }


    void Update()
    {
        if (CurrentGameState == GameState.Execute)
        {
            timeSinceLastTurn += Time.deltaTime;
            if (timeSinceLastTurn >= TurnTime)
            {   
                // Check if the level has been solved in the last turn
                if (CheckLevelSolved())
                {
                    LevelUI.Instance.SpawnLevelSolvedOverlay();
                }
                else
                {
                    // Start next turn
                    // Nothing more to do here because pieces 
                    // Check themself when a new turn has begun
                    CurrentTurn ++;
                    timeSinceLastTurn = 0;
                }
            }
        }
    }


    void ReadLevel()
    {
        // This reads in the that was created in the scene
        
        // Get the parent objects that contain the pieces and tiles
        GameObject tilesParent = GameObject.Find("Tiles");
        GameObject piecesParent = GameObject.Find("Pieces");
        if (tilesParent == null || piecesParent == null)
        {
            Debug.LogError("Can't get tiles or parent GameObject. Abort Level reading");
            return;
        }

        // Correct the possitions of parent objects to correct layer as z and x,y origin
        tilesParent.transform.position = new Vector3(0, 0, Layer.Tiles);
        tilesParent.transform.rotation = Quaternion.identity;
        tilesParent.transform.localScale = Vector3.one;
        piecesParent.transform.position = new Vector3(0, 0, Layer.Pieces);
        piecesParent.transform.rotation = Quaternion.identity;
        piecesParent.transform.localScale = Vector3.one;

        // Go through all tiles
        int maxX = 0; // To get the grid dimensions
        int maxY = 0; 
        foreach(TileBase tile in tilesParent.GetComponentsInChildren<TileBase>())
        {
            int x, y;
            Direction dir;
            bool replaceable = false;
            x = Mathf.RoundToInt(tile.transform.localPosition.x);
            y = Mathf.RoundToInt(tile.transform.localPosition.y);
            dir = DirUtils.AngleInDir(tile.transform.localEulerAngles.z);
            Vector2Int gridPos = new Vector2Int(x, y);
            maxX = Mathf.Max(maxX, x);
            maxY = Mathf.Max(maxY, y);

            // Tile is replaceable when under the grid
            if (y <= -1)
                replaceable = true;

            // Correct the position of the tile
            // rounded values for x and y to account for errors in the editor
            // z=0 because the z value for the layer is already in the parent gameobject
            // Rotation gets handled in the tile
            tile.transform.localPosition = new Vector3(x, y, 0);
            tile.transform.localScale = Vector3.one;
            
            // Add tile to list if possible whennot delete tile
            if (GetTile(gridPos) == null) // null means ther is no tile at this pos
            {
                tiles.Add(tile);
            }
            else
            {
                Debug.LogError($"Couldn't add tile because there already was one at ({gridPos.x}, {gridPos.y})");
                Destroy(tile);
            }

            // Call the Init Method of the tile
            // This has to be done this way because the tile Type varibale
            // in the tile is NOT SET YET
            if (tile is TileEmpty)
            {
                ((TileEmpty)tile).Init(replaceable, gridPos);
            }
            else if (tile is TilePieceTarget)
            {
                ((TilePieceTarget)tile).Init(replaceable, gridPos, dir);
            }
            else if (tile is TileSolid)
            {
                ((TileSolid)tile).Init(replaceable, gridPos);
            }
            else if (tile is TileRedirect)
            {
                ((TileRedirect)tile).Init(replaceable, gridPos, dir);
            }
            else if (tile is TileTeleport)
            {
                ((TileTeleport)tile).Init(replaceable, gridPos);
            }
        }

        // Set the grid width from the max x,y recorded when initialising the tiles
        gridWidth = maxX + 1;
        gridHeight = maxY + 1;

        // Go through all the pieces
        foreach(Piece piece in piecesParent.GetComponentsInChildren<Piece>())
        {
            int x, y;
            Direction dir;
            bool replaceable = false;
            x = Mathf.RoundToInt(piece.transform.localPosition.x);
            y = Mathf.RoundToInt(piece.transform.localPosition.y);
            dir = DirUtils.AngleInDir(piece.transform.localEulerAngles.z);
            

            // Piece is replaceable by player if piece is below the grid
            if (y <= -1) replaceable = true;

            // Correct the position of the piece
            // rounded values for x and y to account for errors in the editor
            // z=0 because the z value for the layer is already in the parent gameobject
            // Rotation gets handled in the piece itself
            piece.transform.localPosition = new Vector3(x, y, 0);
            piece.transform.localScale = Vector3.one;

            piece.Init(replaceable, new Vector2Int(x, y), dir);

            // Add piece to list with all pieces
            pieces.Add(piece);
        }
    }


    //// USED BY DRAGABLE TILES ////

    public bool CreateEmptyTile(Vector2Int pos)
    {
        // Returns success
        // Called by a dragable tile when is gets removed from the grid
        // So that the grid has no hole
        GameObject tilesParent = GameObject.Find("Tiles");
        if (tilesParent != null)
        {
            TileEmpty t = (TileEmpty)Instantiate(tileEmptyPrefab, Vector3.zero, Quaternion.identity);
            t.transform.SetParent(tilesParent.transform);
            t.name = "TileEmpty";
            t.Init(false, pos);
            tiles.Add(t);
            return true;
        }

        return false;
    }

    public bool RemoveEmptyTile(Vector2Int pos)
    {
        // Returns if tile can be replces on the pos
        // Called by a dragable tile when is gets replaced on the grid
        // to remove the empty tile and make place for the draggable tile
        TileBase t = GetTile(pos);
        
        if (t == null)
        {
            // If for some reason the space was empty
            // (its the responsibility of the tile to check if the 
            // pos in even on the grid)
            return true;
        }
        else if (t.Type == TileType.Empty)
        {
            // Remove the tile from list and then destroy the gameObject
            tiles.Remove(t);
            Destroy(t.gameObject);
            return true;
        }
        return false;
    }



    bool CheckLevelSolved()
    {
        // Check every piece if its on the correct target tile
        foreach (Piece p in pieces)
        {
            if (p.HasReachedTarget() == false)
            {
                return false;
            }
        }
        return true;
    }    

    public void SetGameState(GameState newGameState)
    {

        if (CurrentGameState == GameState.Building && newGameState == GameState.Execute)
        {
            CurrentGameState = newGameState;
            // Prepare pieces
            foreach (var p in pieces)
            {
                p.PrepareExecution();
            }
            // Reset other variables
            CurrentTurn = 0;
            timeSinceLastTurn = 0;
            return;
        }
        if ((CurrentGameState == GameState.Execute || CurrentGameState == GameState.ExecutePause) && newGameState == GameState.Building)
        {
            CurrentGameState = newGameState;
            // Reset pieces
            foreach(var p in pieces)
            {
                p.ResetAfterExecution();
            }
            // Reset other variables
            CurrentTurn = 0;
            timeSinceLastTurn = 0;
            return;
        }
        if (CurrentGameState == GameState.Execute && newGameState == GameState.ExecutePause)
        {
            CurrentGameState = newGameState;
            return;
        }
        if (CurrentGameState == GameState.ExecutePause && newGameState == GameState.Execute)
        {
            CurrentGameState = newGameState;
            return;
        }
        Debug.LogError("Chage of game states requested that was not available");
    }


    //// OTHERS ////

    public TileBase GetTile(Vector2Int pos)
    {
        foreach (TileBase t in tiles)
        {
            if (t.gridPos == pos)
                return t;
        }
        return null;
    }

    public bool IsGridPosOccupied(Vector2Int pos)
    {
        // Every Tile and Piece count as occupied
        // EXCEPT an EMPTY TILE

        // Check the tile
        TileBase t = GetTile(pos);
        if (t != null)
        {
            if (t.Type != TileType.Empty)
            {
                return true;
            }
        }

        // Check Pieces
        foreach(var pic in pieces)
        {
            // When the tile is not on the grid girdPos would be null
            if (pic.gridPos == pos)
            {
                return true;
            }
        }

        return false;
    }

}
