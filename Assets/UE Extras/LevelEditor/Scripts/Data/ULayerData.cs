using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ultra.LevelEditor
{
    public enum LayerTypes
    {
        Tile,
        Entity
    }
    [Serializable]
    public struct ULayerData
    {
	    public LayerTypes LayerType;
        public float CellSize;
        public UTileDataSave[] Tiles;
        public ULayerData(LayerTypes layerType, float cellSize, UTileDataSave[] tileDatas)
        {
            this.LayerType = layerType;
            this.CellSize = cellSize;
            this.Tiles = tileDatas;
        }
    }
}
