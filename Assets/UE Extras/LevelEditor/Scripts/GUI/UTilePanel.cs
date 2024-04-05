using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace Ultra.LevelEditor
{
    public class UTilePanel : UltraGUI
    {
        public GameObject TileSelectButtonPrefab;
        private TileBase[] _tiles;
        private GridLayoutGroup _tilesGrid;
        protected override void Initialize()
        {
            _tiles = Resources.LoadAll<TileBase>("UTiles");
            _tilesGrid = transform.GetComponentInChildren<GridLayoutGroup>();

            foreach (TileBase tile in _tiles)
            {
                UTileSelectButton tileSelectButton = GUIManager.InstantiateGUltraUI<UTileSelectButton>(TileSelectButtonPrefab, _tilesGrid.transform);
                tileSelectButton.InitializeTileSelectButton(tile);
            }
        }
    }
}
