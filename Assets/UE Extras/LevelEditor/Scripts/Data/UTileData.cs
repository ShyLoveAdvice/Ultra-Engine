using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

namespace Ultra.LevelEditor
{
    [Serializable]
    public struct UTileData
    {
        public Vector3Int Pos;
        public TileBase TileBase;
        public UTileData(Vector3Int pos, TileBase tileBase)
        {
            this.Pos = pos;
            this.TileBase = tileBase;
        }
    }
}
