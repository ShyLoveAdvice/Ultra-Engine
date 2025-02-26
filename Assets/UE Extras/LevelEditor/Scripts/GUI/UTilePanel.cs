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
        private Animator _anim;
        private TileBase[] _tiles;
        private GridLayoutGroup _tilesGrid;
        private bool _folded;
        protected override void Initialize()
        {
            _tiles = Resources.LoadAll<TileBase>("UTiles");
            _tilesGrid = transform.GetComponentInChildren<GridLayoutGroup>();
            _anim = GetComponent<Animator>();

            foreach (TileBase tile in _tiles)
            {
                UTileSelectButton tileSelectButton = GUIManager.InstantiateGUltraUI<UTileSelectButton>(TileSelectButtonPrefab, _tilesGrid.transform);
                tileSelectButton.InitializeTileSelectButton(tile);
            }
        }

        public void FoldNUnfold()
        {
            _folded = !_folded;
            _anim.SetBool("Folded", _folded);
        }
    }
}
