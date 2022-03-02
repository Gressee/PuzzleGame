using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Core.Singleton;
using Shared.Defines;
using Shared.DirUtils;

public class GameManager : Singleton<GameManager>
{

    [SerializeField]
    TileBase tileEmptyPrefab, tileRedirectPrefab, tilePieceTargetPrefab;

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
    Dictionary<Vector2Int, TileBase> tiles = new Dictionary<Vector2Int, TileBase>();
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
                // Nothing more to do here because pieces 
                // Check themself when a new turn has begun
                CurrentTurn ++;
                timeSinceLastTurn = 0;
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
            x = Mathf.RoundToInt(tile.transform.localPosition.x);
            y = Mathf.RoundToInt(tile.transform.localPosition.y);
            dir = DirUtils.AngleInDir(tile.transform.localEulerAngles.z);
            Vector2Int gridPos = new Vector2Int(x, y);
            maxX = Mathf.Max(maxX, x);
            maxY = Mathf.Max(maxY, y);

            // Correct the position of the tile
            // rounded values for x and y to account for errors in the editor
            // z=0 because the z value for the layer is already in the parent gameobject
            // Rotation gets handled in the tile
            tile.transform.localPosition = new Vector3(x, y, 0);
            tile.transform.localScale = Vector3.one;
            
            // Add tile to dict if possible whennot delete tile
            if (!tiles.ContainsKey(gridPos))
            {
                tiles.Add(gridPos, tile);
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
                ((TileEmpty)tile).Init(gridPos);
            }
            else if (tile is TilePieceTarget)
            {
                ((TilePieceTarget)tile).Init(gridPos, dir);
            }
            else if (tile is TileRedirect)
            {
                ((TileRedirect)tile).Init(gridPos, dir);
            }
        }

        // Go through all the pices
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
        }

        // Set the grid width from the max x,y recorded when initialising the tiles
        gridWidth = maxX + 1;
        gridHeight = maxY + 1;
        // Check if enogh tiles are in the grid to fill the rectangle
        if (tiles.Count != gridHeight * gridWidth)
        {
            Debug.LogError($"There isn't the right amount of Tiles in the grid for its width/height\nTiles={tiles.Count} w={gridWidth} h={gridHeight}");
        }


    }

    

    public void SetGameState(GameState gameState)
    {
        // TODO Implement the chnage of game states in game manager
        CurrentGameState = gameState;
    }

    //// OTHERS ////

    public TileBase GetTile(Vector2Int pos)
    {
        if (tiles.ContainsKey(pos))
        {
            return tiles[pos];
        }
        return null;
    }

    public bool IsGridPosOccupied(Vector2Int pos)
    {
        // Check the tiles
        if (tiles.ContainsKey(pos))
        {
            return false;
        }

        // Check Pieces
        foreach(var pic in pieces)
        {
            // When the tile is not on the grid girdPos would be null
            if (pic.gridPos == pos)
            {
                return false;
            }
        }

        return true;
    }

}
