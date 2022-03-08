using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shared.Utils
{
    public static class Utils
    {
        public static float RoundToHalf(float value)
        {
            value *= 2;
            value = Mathf.Round(value);
            value *= 0.5f;
            return value;
        }

        public static Vector2Int RoundVec2(Vector2 v)
        {
            return new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
        }
    }
}