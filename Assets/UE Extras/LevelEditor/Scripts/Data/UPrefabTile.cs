using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace Ultra.LevelEditor
{
    [CreateAssetMenu(menuName = ("Prefab Tile"), fileName = ("New Prefab Tile"))]
    public class UPrefabTile : TileBase
    {
        public string m_name;
        public GameObject prefab;
        public Vector2Int gridSize;
        public Sprite prefabSprite;
    }
}
