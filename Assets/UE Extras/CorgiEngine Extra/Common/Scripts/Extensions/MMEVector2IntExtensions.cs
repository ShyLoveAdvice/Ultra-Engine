using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.CorgiEngine
{
    public static class MMEVector2IntExtensions
    {
        public static Vector2 MMVector2(this Vector2Int vector)
        {
            return new Vector2(vector.x, vector.y);
        }
    }
}
