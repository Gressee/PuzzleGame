using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Core.Singleton;
using Shared.Defines;
using Shared.Levels;

public class GameManager : Singleton<GameManager>
{

    [SerializeField]
    TileBase tileEmptyPrefab, tileRedirectPrefab;

    [SerializeField]
    Piece piecePrefab;

    public GameState CurrentGameState {get; protected set;} = GameState.Execute;


    // A Turn is for example when a piece translates from one to the next tile
    // The times it takes to execuite on Turn
    public float TurnTime {get; protected set;} = 1.0f;
    // When in 'Execute' State this gets increased according to the 'TurnTime'
    // Pieces use this var to calc when they have to be where and in what orientation
    public int CurrentTurn {get; protected set;} = 0;

    // How much time has passed since the last turn has been started
    float timeSinceLastTurn = 0.0f;



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
        // load
        Level lvl = new Levels().levels[lvlNum];


        // Spawn empty tiles
        for (int x = 0; x < lvl.gridWidth; x++)
        {
            for (int y = 0; y < lvl.gridHeight; y ++)
            {
                SpawnTile(false, TileType.Empty, new Vector2Int(x, y), Direction.None);
            }
        }

        // Spawn special tiles
        foreach (var t in lvl.tiles)
        {
            if (t.onGridAtStart)
            {
                SpawnTile(true, t.type, t.pos, t.dir);
            }
            else
            {
                // TODO handle when tile is not on grid
            }
        }

        // Spawn Pieces
        foreach (var p in lvl.pieces)
        {
            if (p.onGridAtStart)
            {
                SpawnPiece(p.pos, p.dir);
            }
            else
            {
                // TODO handle when piece is not on grid
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

            case TileType.Redirect:
                tile = (TileRedirect)Instantiate(tileRedirectPrefab, Vector3.zero, Quaternion.identity);
                ((TileRedirect)tile).Init(pos, dir);
                tiles.Add(pos, tile);    
            break;
        }

    }

    void SpawnPiece(Vector2Int pos, Direction dir)
    {
        Piece p = Instantiate(piecePrefab, Vector3.zero, Quaternion.identity);
        p.Init(dir, pos);
        pieces.Add(p);
    }

    public TileBase GetTile(Vector2Int pos)
    {
        if (tiles.ContainsKey(pos))
        {
            return tiles[pos];
        }
        return null;
    }

}
