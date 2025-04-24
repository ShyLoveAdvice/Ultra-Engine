using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace Ultra.LevelEditor
{
    public class UPrefabSelectButton : USelectButton
    {
        [SerializeField, MMReadOnly] private UPrefabTile _prefabTile;
        public Image Icon;
        public Image NameBG;
        public Text NameText;
        
        public Color NameBGSelectedColor;
        public Color NameBGUnSelectedColor;

        public void Initialize(UPrefabTile prefabTile)
        {
            _prefabTile = prefabTile;
            NameText.text = prefabTile.m_name;
            Icon.sprite = _prefabTile.prefabSprite;
        }

        public override void Select()
        {
            base.Select();
            
            NameBG.color = NameBGSelectedColor;
        }
    }
}
