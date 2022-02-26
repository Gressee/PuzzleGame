using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shared.Defines;

namespace Shared.Levels
{

    // This defines stuff that is uses when spawning the level
    // Applies to all tiles
    public class TileStartCondition
    {
        public Vector2Int pos {get; private set;}
        public bool onGridAtStart {get; private set;}
        public TileType type {get; private set;}
        public Direction dir  {get; private set;}

        public TileStartCondition(int x, int y, bool isOnGridAtStart, TileType tileType, Direction direction)
        {
            pos = new Vector2Int(x, y);
            onGridAtStart = isOnGridAtStart;
            type = tileType;
            dir = direction;
        }
    }

    // This defines stuff that is uses when spawning the level
    // Applies to all pieces
    public class PieceStartCondition
    {
        public Vector2Int pos {get; private set;}
        public bool onGridAtStart {get; private set;}
        public Direction dir  {get; private set;}

        public PieceStartCondition(int x, int y, bool isOnGridAtStart, Direction direction)
        {
            pos = new Vector2Int(x, y);
            onGridAtStart = isOnGridAtStart;
            dir = direction;
        }
    }


    public class Level
    {
        public int gridWidth {get; protected set;}
        public int gridHeight {get; protected set;}

        // This includes all tiles (on/off grid at start)
        // On every place where no tile is specified there gets an EMPTY TILE placed
        public List<TileStartCondition> tiles {get; protected set;} = new List<TileStartCondition>()
        {
        };

        // This includes all pieces
        public List<PieceStartCondition> pieces {get; protected set;} = new List<PieceStartCondition>()
        {
        };

        public Level(int gridWidth, int gridHeight, List<TileStartCondition> tiles, List<PieceStartCondition> pieces)
        {
            this.gridWidth = gridWidth;
            this.gridHeight = gridHeight;
            this.tiles = tiles;
            this.pieces = pieces;
        }
    }



    //// DEFINES OF ALL LEVELS ////

    public class AllLevels
    {
        // The key of this dict is the level number
        public Dictionary<int, Level> levels = new Dictionary<int, Level>()
        {
            
            // LEVEL 001
            {1, new Level(
                // Grid Width Height
                10, 10,
                // Tiles
                new List<TileStartCondition>()
                {
                    new TileStartCondition(2, 2, true, TileType.Redirect, Direction.Down),
                },
                // Pieces
                new List<PieceStartCondition>()
                {
                    new PieceStartCondition(1, 2, true, Direction.Right)
                }
            )}



            // LEVEL 002



            // LEVEL 003
        };
    }
}
