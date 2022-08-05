
namespace Shared.Defines
{
    public static class Layer
    {
        // These Layers are the z values of objects.
        // So a lower number is closer to the cam then a heigher number 
        // LOWEST LAYER SHOULD BE 0
        
        // While a Piece or Tile gets draged this value gets added 
        // so its always on top of everything while beeing draged
        public const float DragOffset = -1.5f;

        public const float Pieces = 4.0f;
        public const float Tiles = 5.0f;
        public const float GridBackground = 6.0f;
        public const float GridLineOffset = -0.1f;
    }

    public enum Direction
    {
        None,
        Right,
        Up,
        Left,
        Down
    }

    public enum TileType
    {
        Base,
        PieceTarget,
        Solid, 
        Redirect,
        Teleport
    }

    public enum GameState
    {
        None,
        Building,
        Execute,
        ExecutePause
    }

    public enum Sound
    {
        // Buttons
        BtnClick,

        SceneTransition
    }
}