using MoreMountains.Tools;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace Ultra.LevelEditor
{
    public class UTileSelectButton : USelectButton
    {
        
        [SerializeField, MMReadOnly] protected TileBase _tileBase;
        public Image Icon;
        public Image NameBG;
        public Text NameText;
        public Image RuleTileIcon;

        public Color NameBGSelectedColor;
        public Color NameBGUnSelectedColor;

        public void InitializeTileSelectButton(TileBase tileBase)
        {
            BlockMouseEvent = false;

            _tileBase = tileBase;

            NameText.text = tileBase.name;
            RuleTileIcon.gameObject.SetActive(false);

            if (tileBase != null && tileBase is Tile)
            {
                Tile tile = (Tile)tileBase;
                Icon.sprite = tile.sprite;
            }
            else if(tileBase != null && tileBase is RuleTile)
            {
                RuleTile ruleTile = (RuleTile)tileBase;
                Icon.sprite = ruleTile.m_DefaultSprite;
                RuleTileIcon.gameObject.SetActive(true);
            }
        }
        public override void Select()
        {
            base.Select();

            NameBG.color = NameBGSelectedColor;
            GUIManager.UpdateCurrentSelectedTileBase(_tileBase);
        }
        public override void UnSelect()
        {
            base.UnSelect();

            NameBG.color= NameBGUnSelectedColor;
        }
    }
}
