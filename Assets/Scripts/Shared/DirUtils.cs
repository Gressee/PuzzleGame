using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shared.Defines;

namespace Shared.DirUtils
{

    public static class DirUtils
    {

        public static Vector2Int NextPosInDir(Direction dir, Vector2Int pos)
        {
            // Moves the position ONE UNIT in the given direction
            switch (dir)
            {
                case Direction.Right:
                    pos.x ++;
                break;

                case Direction.Up:
                    pos.y ++;
                break;

                case Direction.Left:
                    pos.x --;
                break;

                case Direction.Down:
                    pos.y --;
                break;
            }
            return pos;
        }

        public static Direction RotateLeft(Direction dir)
        {
            // Rotates the direction 90 degrees left
            switch (dir)
            {
                case Direction.Right:
                    return Direction.Up;
                case Direction.Up:
                    return Direction.Left;
                case Direction.Left:
                    return Direction.Down;
                case Direction.Down:
                    return Direction.Right;
                
            }
            return Direction.None;
        }

        public static Direction RotateRight(Direction dir)
        {
            // Rotates the direction 90 degrees right
            switch (dir)
            {
                case Direction.Right:
                    return Direction.Down;
                case Direction.Down:
                    return Direction.Left;
                case Direction.Left:
                    return Direction.Up;
                case Direction.Up:
                    return Direction.Right;
                
            }
            return Direction.None;
        }

        public static float DirInAngle(Direction dir)
        {
            // returns angle between 0 and 360
            // 0 is right and gets greater ccw
            switch (dir)
            {
                case Direction.Right:
                    return 0;
                case Direction.Up:
                    return 90;
                case Direction.Left:
                    return 180;
                case Direction.Down:
                    return 270;
                
            }
            return -1;
        }
    }
}