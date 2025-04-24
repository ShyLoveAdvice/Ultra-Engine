using MoreMountains.Tools;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Ultra.LevelEditor
{
    public class ULevelEditorGUIManager : MMSingleton<ULevelEditorGUIManager>
    {
        public Camera UICamera;
        public UScaleDraggerManager ScaleDraggerManager;
        public ULevelEditorToolTypes InitiallySelectedTool;

        private USelectButton[] _selectButtons;
        public List<UltraGUI> AllUltraGUIs { get => _allUltraGUIs; }
        public List<UltraGUI> _allUltraGUIs;

        private ULevelEditorToolTypes _currentSelectedTool;
        private TileBase _currentSelectedTileBase;
        private bool _isMouseOverUI;

        public bool IsMouseOverUI { get => _isMouseOverUI; }
        public void InitializeGUIManager(ULevelEditor levelEditor)
        {
            _allUltraGUIs = transform.GetComponentsInChildren<UltraGUI>().ToList();

            for (int i = 0; i < _allUltraGUIs.Count; i++)
            {
                _allUltraGUIs[i].InitializeGUI(this);
            }

            InitializeSelectButtons();
        }
        private void InitializeSelectButtons()
        {
            _selectButtons = transform.GetComponentsInChildren<USelectButton>();

            UToolSelectButton initiallySelectedToolButton = null;
            bool initializedTileSelectButton = false;

            for (int i = 0; i < _selectButtons.Length; i++)
            {
                if (_selectButtons[i].GetType() == typeof(UToolSelectButton))
                {
                    initiallySelectedToolButton = _selectButtons[i] as UToolSelectButton;
                    if (initiallySelectedToolButton.ButtonToolType == InitiallySelectedTool)
                    {
                        _currentSelectedTool = initiallySelectedToolButton.ButtonToolType;
                        UpdateSelectButton(initiallySelectedToolButton);
                    }
                }
                else if (!initializedTileSelectButton && _selectButtons[i].GetType() == typeof(UTileSelectButton))
                {
                    UpdateSelectButton(_selectButtons[i]);
                    initializedTileSelectButton = true;
                }                                                                                                                                     
            }
        }

        public TUltraGUI InstantiateGUltraUI<TUltraGUI>(GameObject prefab, Transform parentTransform) where TUltraGUI: UltraGUI
        {
            GameObject instantiatedGO = Instantiate(prefab, parentTransform);
            TUltraGUI ultraUI = instantiatedGO.GetComponent<TUltraGUI>();

            if(ultraUI == null)
            {
                ultraUI = instantiatedGO.GetComponentInChildren<TUltraGUI>();
            }

            ultraUI.InitializeGUI(this);
            _allUltraGUIs.Add(ultraUI);
            return ultraUI;
        }
        public void DestroyUltraUI<TUltraUI>(TUltraUI ultraUI) where TUltraUI : UltraGUI
        {
            _allUltraGUIs.Remove(ultraUI);
            Destroy(ultraUI.gameObject);
        }

        public ULevelEditorToolTypes CurrentSelectedTool()
        {
            return _currentSelectedTool;
        }
        public TileBase CurrentSelectedTileBase()
        {
            return _currentSelectedTileBase;
        }
        #region Update Methods
        public void UpdateGUIs()
        {
            for (int i = 0; i < _allUltraGUIs.Count; i++)
            {
                _allUltraGUIs[i].UpdateGUI();
            }
        }
        public void UpdateCurrentSelectedTool(ULevelEditorToolTypes toolType)
        {
            _currentSelectedTool = toolType;
            ULevelEditorToolTypes[] toolTypes = Enum.GetValues(typeof(ULevelEditorToolTypes)) as ULevelEditorToolTypes[];

            for (int i = 0; i < toolTypes.Length; i++)
            {
                if (toolTypes[i] == toolType)
                {
                    ULevelEditor.Instance.ToolSelected(toolTypes[i]);
                }
                else
                {
                    ULevelEditor.Instance.ToolUnSelected(toolTypes[i]);
                }
            }
        }
        public void UpdateMouseOverUI(bool isMouseOverUI)
        {
            _isMouseOverUI = isMouseOverUI;
        }
        public void UpdateSelectButton(USelectButton button)
        {
            foreach (USelectButton b in _selectButtons)
            {
                if (b.GetType() == button.GetType())
                {
                    if (b == button)
                    {
                        b.Select();
                    }
                    else
                    {
                        b.UnSelect();
                    }
                }
            }
        }
        public void UpdateSelectButton(ULevelEditorToolTypes toolType)
        {
            foreach (USelectButton b in _selectButtons)
            {
                UToolSelectButton uToolSelectButton = b as UToolSelectButton;
                if (uToolSelectButton != null)
                {
                    if (uToolSelectButton.ButtonToolType == toolType)
                    {
                        b.Select();
                    }
                    else
                    {
                        b.UnSelect();
                    }
                }
            }
        }
        #endregion
    }
}
