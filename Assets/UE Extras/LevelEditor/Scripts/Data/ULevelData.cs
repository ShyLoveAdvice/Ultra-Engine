using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ultra.LevelEditor
{
    [Serializable]
    public struct ULevelData
    {
        public string Name;
        public string Description;
        public Vector2Int Size;
        public ULayerData[] LayerDatas; 

        public ULevelData(string name, string description, Vector2Int size)
        {
            Name = name;
            Description = description;
            Size = size;
            LayerDatas = new ULayerData[4];
        }
    }
}
