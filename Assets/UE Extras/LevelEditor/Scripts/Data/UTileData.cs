using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Ultra.LevelEditor
{
    [Serializable]
    public struct UTileData
    {
        public bool Initialized;
        public TileBase TileBase;
        public UTileData(TileBase tileBase, bool initialized = true)
        {
            Initialized = initialized;
            this.TileBase = tileBase;
        }
        public static bool operator == (UTileData a, UTileData b)
        {
            return a.TileBase == b.TileBase;
        }
        public static bool operator !=(UTileData a, UTileData b)
        {
            return a.TileBase != b.TileBase;
        }
    }

    public struct UTileDataSave
    {
        public Vector3Int WorldPos;
        public TileBase TileBase;
        public UTileDataSave(Vector3Int worldPos, TileBase tileBase)
        {
            WorldPos = worldPos;
            TileBase = tileBase;
        }
    }
}
