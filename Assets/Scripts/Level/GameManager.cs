using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Core.Singleton;
using Shared.Defines;
using Shared.Utils;
using Shared.DirUtils;

public class GameManager : Singleton<GameManager>
{

    [SerializeField]
    TileBase tileRedirectPrefab, tilePieceTargetPrefab, tileTeleportPrefab;

    [SerializeField]
    Piece piecePrefab;

    [SerializeField]
    GameObject gridLinePrefab;


    public GameState CurrentGameState {get; protected set;} = GameState.Building;


    // A Turn is for example when a piece translates from one to the next tile
    // The times it takes to execuite on Turn
    public float TurnTime {get; protected set;} = 1.0f;

    // How much time has passed since the last turn has been started
    // Starts not at 0 because then after pressing the execute button pices move 
    // 'TurnTime' secods later
    public float timeSinceLastTurn {get; protected set;} = 0.9f;

    // Level
    List<TileBase> tiles = new List<TileBase>();
    List<Piece> pieces = new List<Piece>();
    public int gridWidth  {get; protected set;} = 0;
    public int gridHeight {get; protected set;} = 0;


    void Start()
    {
        ReadLevel();
        SpawnGridLines();
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
                    UI_Level.Instance.SpawnLevelSolvedOverlay();
                }
                else
                {
                    // Start next turn
                    timeSinceLastTurn = 0;
                    foreach(Piece p in pieces)
                    {
                        p.CalculateNextTurn();
                    }
                }
            }
        }
    }


    void ReadLevel()
    {
        // This reads in the that was created in the scene
        
        // Set the correct layer of the background
        Vector3 gbPos = new Vector3(0, 0, Layer.GridBackground);
        GridBackground.Instance.transform.position = gbPos;

        // read the grid dimensions from the grid bbackgroud object
        gridWidth = GridBackground.Instance.GetGridWidth();
        gridHeight = GridBackground.Instance.GetGridHeight();

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
        foreach(TileBase tile in tilesParent.GetComponentsInChildren<TileBase>())
        {
            Direction dir;
            bool replaceable = false;
            float x = Utils.RoundToHalf(tile.transform.localPosition.x);
            float y = Utils.RoundToHalf(tile.transform.localPosition.y);
            Vector2 tilePos = new Vector2(x, y);
            dir = DirUtils.AngleInDir(tile.transform.localEulerAngles.z);

            // Tile is replaceable when under the grid
            if (y <= -1)
            {
                replaceable = true;
            }

            // Correct the position of the tile
            // rounded values for x and y to account for errors in the editor
            // z=0 because the z value for the layer is already in the parent gameobject
            // Rotation gets handled in the tile
            tile.transform.localPosition = new Vector3(x, y, 0);
            tile.transform.localScale = Vector3.one;
            
            // Add tile to list if possible whennot delete tile
            if (GetTile(Utils.RoundVec2(tilePos)) == null) // null means ther is no tile at this pos
            {
                tiles.Add(tile);
            }
            else
            {
                Debug.LogError($"Couldn't add tile because there already was one at ({tilePos.x}, {tilePos.y})");
                Destroy(tile);
            }

            // Call the Init Method of the tile
            // This has to be done this way because the tile Type varibale
            // in the tile is NOT SET YET
            if (tile is TilePieceTarget)
            {
                ((TilePieceTarget)tile).Init(replaceable, tilePos, dir);
            }
            else if (tile is TileSolid)
            {
                ((TileSolid)tile).Init(replaceable, tilePos);
            }
            else if (tile is TileRedirect)
            {
                ((TileRedirect)tile).Init(replaceable, tilePos, dir);
            }
            else if (tile is TileTeleport)
            {
                ((TileTeleport)tile).Init(replaceable, tilePos);
            }
        }

        // Go through all the pieces
        foreach(Piece piece in piecesParent.GetComponentsInChildren<Piece>())
        {
            Direction dir;
            bool replaceable = false;
            float x = Utils.RoundToHalf(piece.transform.localPosition.x);
            float y = Utils.RoundToHalf(piece.transform.localPosition.y);
            dir = DirUtils.AngleInDir(piece.transform.localEulerAngles.z);
            

            // Piece is replaceable by player if piece is below the grid
            if (y <= -1) replaceable = true;

            // Correct the position of the piece
            // rounded values for x and y to account for errors in the editor
            // z=0 because the z value for the layer is already in the parent gameobject
            // Rotation gets handled in the piece itself
            piece.transform.localPosition = new Vector3(x, y, 0);
            piece.transform.localScale = Vector3.one;

            piece.Init(replaceable, new Vector2(x, y), dir);

            // Add piece to list with all pieces
            pieces.Add(piece);
        }
    }


    void SpawnGridLines()
    {
        int x, y;
        GameObject gridLine;
        
        // Vertical lines
        for (x = 0; x < gridWidth-1; x++)
        {
            for (y = 0; y < gridHeight; y++)
            {
                // Gets a small offset to be infront og the tile background
                gridLine = Instantiate(gridLinePrefab, Vector3.zero, Quaternion.identity);
                gridLine.transform.SetParent(GridBackground.Instance.transform);
                gridLine.transform.localPosition = new Vector3((float)x + 0.5f, y, Layer.GridLineOffset);
            }
        }

        // Horizontal lines
        for (x = 0; x < gridWidth; x++)
        {
            for (y = 0; y < gridHeight-1; y++)
            {
                // Gets a small offset to be infront og the tile background
                gridLine = Instantiate(gridLinePrefab, Vector3.zero, Quaternion.identity);
                gridLine.transform.SetParent(GridBackground.Instance.transform);
                gridLine.transform.localEulerAngles = new Vector3(0, 0, 90); 
                gridLine.transform.localPosition = new Vector3(x, (float)y + 0.5f, Layer.GridLineOffset);
            }
        }
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
            timeSinceLastTurn = TurnTime * 0.9f; // Why ??? see comment at initialisation
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
            timeSinceLastTurn = TurnTime * 0.9f; // Why ??? see comment at initialisation
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

    public bool IsPosOnGrid(Vector2Int pos)
    {
        if (pos.x < gridWidth && pos.y < gridHeight && pos.x >= 0 && pos.y >= 0)
            return true;
        else
            return false;
    }

    public bool IsGridPosOccupied(Vector2Int pos)
    {
        // Every Tile and Piece count as occupied

        // Check the tile
        TileBase t = GetTile(pos);
        if (t != null)
        {
            return true;
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
