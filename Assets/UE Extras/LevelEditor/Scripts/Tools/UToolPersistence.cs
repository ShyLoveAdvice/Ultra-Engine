using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Ultra.LevelEditor
{
    public class UToolPersistence: MonoBehaviour
    {
        protected ULevelEditor LevelEditor { get => ULevelEditor.Instance; }
        protected ULevelEditorInputManager InputManager { get => LevelEditor.InputManager; }
        protected Vector3Int CurrentMouseCellPos { get; private set; }
        protected Vector3Int LastCellPos { get; private set; }

        private void OnEnable()
        {
            LevelEditor.ToolPersistenceEvent += HandleInput;
        }
        private void OnDisable()
        {
            LevelEditor.ToolPersistenceEvent -= HandleInput;
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
        private UTileData[] _selectedTileDatas;
        protected virtual void CtrlCEvent()
        {
            if (LevelEditor.Selection.SomethingSelected)
            {
                _selectedCells = LevelEditor.Selection.GetSelectedCells();
                _selectedTileDatas =  LevelEditor.Selection.GetSelectedTileDatas();
            }
        }
        protected virtual void CtrlVEvent()
        {
            LevelEditor.BoxSelectTool.InterruptTool();

            if (_selectedCells != null && _selectedTileDatas != null)
            {
                LevelEditor.Selection.ClearSelected();
                LevelEditor.Selection.BuildSelected(_selectedCells, _selectedTileDatas);
            }
        }
    }
}
