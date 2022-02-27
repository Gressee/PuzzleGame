using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Core.Singleton;
using Shared.Defines;
using Shared.Levels;

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

    public Level currentLevel {get; protected set;}

    Dictionary<Vector2Int, TileBase> tiles = new Dictionary<Vector2Int, TileBase>();

    List<Piece> pieces = new List<Piece>();


    void Start()
    {
        SpawnLevel(1);
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


    void SpawnLevel(int lvlNum)
    {
        // RESET VARIABLES
        CurrentTurn = 0;

        // Destroy the unity game objects and then remove them from the dict/list
        while (tiles.Count > 0)
            Destroy(tiles.ElementAt(0).Value);
        tiles = new Dictionary<Vector2Int, TileBase>();

        while (pieces.Count > 0)
            Destroy(pieces[0]);
        pieces = new List<Piece>();

        
        // SPAWN LEVEL
        currentLevel = null; // Incase lvl fails to load bellow
        currentLevel = new AllLevels().levels[lvlNum];

        int offGridPieces = 0;

        // Spawn empty tiles
        for (int x = 0; x < currentLevel.gridWidth; x++)
        {
            for (int y = 0; y < currentLevel.gridHeight; y ++)
            {
                SpawnTile(false, TileType.Empty, new Vector2Int(x, y), Direction.None);
            }
        }

        // Spawn special tiles
        foreach (var t in currentLevel.tiles)
        {
            SpawnTile(true, t.type, t.pos, t.dir);
        }

        // Spawn Pieces
        // Pices that are of grid get places in a row below the grid
        foreach (var p in currentLevel.pieces)
        {
            if (p.replaceable == false && p.gridPos != null)
            {
                SpawnPiece(p.replaceable, p.gridPos.GetValueOrDefault(new Vector2Int(-1, -1)), p.dir);
            }
            else if (p.replaceable == true && p.gridPos == null)
            {   
                int x = offGridPieces % currentLevel.gridWidth;
                int y = -2 - Mathf.FloorToInt(offGridPieces / currentLevel.gridWidth);
                Vector2Int pos = new Vector2Int(x, y);
                SpawnPiece(p.replaceable, pos, p.dir);
            }
            else if (p.replaceable == false && p.gridPos == null)
            {
                Debug.LogError("Piece is maked NOT REPLACEABLE in the level file but given position is NULL");
            }
        }
    }

    

    void SpawnTile(bool replace, TileType type, Vector2Int pos, Direction dir)
    {
        // When 'replace' is true then if a tile is already on the grid pos
        // then it get replaced.
        // When false then not

        if (GetTile(pos) != null)
        {
            if (replace == false)
            {
                Debug.LogWarning("Tryed creating Tile, but positin was already occupied and not in replace mode !!!");
                return;
            }
            else
            {
                // Delete old tile on this pos
                Destroy(GetTile(pos));
                tiles.Remove(pos);
            }
        }

        TileBase tile;
        switch (type)
        {
            case TileType.Empty:
                tile = (TileEmpty)Instantiate(tileEmptyPrefab, Vector3.zero, Quaternion.identity);
                ((TileEmpty)tile).Init(pos);
                tiles.Add(pos, tile);    
            break;
            
            case TileType.PieceTarget:
                tile = (TilePieceTarget)Instantiate(tilePieceTargetPrefab, Vector3.zero, Quaternion.identity);
                ((TilePieceTarget)tile).Init(pos, dir);
                tiles.Add(pos, tile);    
            break;

            case TileType.Redirect:
                tile = (TileRedirect)Instantiate(tileRedirectPrefab, Vector3.zero, Quaternion.identity);
                ((TileRedirect)tile).Init(pos, dir);
                tiles.Add(pos, tile);    
            break;
        }

    }


    void SpawnPiece(bool replaceable, Vector2Int pos, Direction dir)
    {
        // 'pos' is either the grid pos or the pos outside the grid of the pices
        // what depends on the value of 'replaceable'

        Piece p = Instantiate(piecePrefab, Vector3.zero, Quaternion.identity);
        p.Init(replaceable, pos, dir);
        pieces.Add(p);
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
