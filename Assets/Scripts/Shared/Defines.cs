
namespace Shared.Defines
{
    public static class Layer
    {
        // These Layers are the z values of objects.
        // So a lower number is closer to the cam then a heigher number 
        // LOWEST LAYER SHOULD BE 0
        
        public const float DrageTiles = 1.0f;
        public const float Pieces = 2.0f;
        public const float Tiles = 5.0f;
    }

    public enum Direction
    {
        None,
        Right,
        Up,
        Left,
        Down
    }

    // When adding a new type also add in 'GameManager.SpawnTile'
    public enum TileType
    {
        Base,
        Empty,
        PieceTarget,
        Solid,
        DestroyPiece,
        Redirect,
        Rotate,
        Teleport,
    }

    public enum GameState
    {
        Building,
        Execute,
    }
}