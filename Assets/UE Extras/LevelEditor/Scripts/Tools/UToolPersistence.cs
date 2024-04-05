using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Ultra.LevelEditor
{
    public class UToolPersistence
    {
        protected ULevelEditor LevelEditor { get; private set; }
        protected ULevelEditorInputManager InputManager { get => LevelEditor.InputManager; }
        protected Vector3Int CurrentMouseCellPos { get; private set; }
        protected Vector3Int LastCellPos { get; private set; }
        public UToolPersistence(ULevelEditor levelEditor)
        {
            LevelEditor = levelEditor;
        }
        public void HandleInput()
        {
            CurrentMouseCellPos = InputManager.CurrentMouseCellPos;

            switch (InputManager.CurrentInput)
            {
                case LevelEditorInputs.None:
                    break;
                case LevelEditorInputs.CtrlC:
                    CtrlCEvent();
                    break;
                case LevelEditorInputs.CtrlV:
                    CtrlVEvent();
                    break;
                case LevelEditorInputs.CtrlD:
                    break;
            }
            LastCellPos = CurrentMouseCellPos;
        }
        private Vector3Int[] _selectedCells;
        private Vector3Int[] _selectedTiles;
        private TileBase[] _selectedTileBases;
        protected virtual void CtrlCEvent()
        {
            if (LevelEditor.Selection.SomethingSelected)
            {
                _selectedCells = LevelEditor.Selection.GetSelectedCells();
                LevelEditor.BoxSelectTool.DeterminePreviewTiles(ref _selectedTiles, ref _selectedTileBases);
                if(_selectedTiles.Length == 0)
                {
                    LevelEditor.BoxSelectTool.DetermineTiles(ref _selectedTiles, ref _selectedTileBases);
                }
            }
        }
        protected virtual void CtrlVEvent()
        {
            LevelEditor.BoxSelectTool.InterruptTool();

            if (_selectedCells != null && _selectedCells.Length > 0)
            {
                LevelEditor.Selection.BuildSelection(_selectedCells);
                LevelEditor.Selection.ClearDrawnSelected();
                LevelEditor.Selection.DrawSelected();
            }
            if (_selectedTiles != null && _selectedTiles.Length > 0)
            {
                LevelEditor.CurrentLayer.DrawTilesPreview(_selectedTiles, _selectedTileBases);
            }

        }
    }
}
