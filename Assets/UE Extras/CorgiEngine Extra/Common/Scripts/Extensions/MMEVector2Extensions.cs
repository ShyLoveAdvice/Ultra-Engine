using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.CorgiEngine
{
    public static class MMEVector2Extensions
    {
	    public static Vector2Int MMVector2Int(this Vector2 vector)
        {
            return new Vector2Int((int)vector.x, (int)vector.y);
        }
        public static Vector3Int MMVector3Int(this Vector2 vector) 
        {
            return new Vector3Int((int)vector.x, (int)vector.y);
        }
    }
}
