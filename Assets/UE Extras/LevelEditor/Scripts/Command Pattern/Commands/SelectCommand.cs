using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ultra.LevelEditor
{
    public class SelectCommand : ICommand
    {
        private UBoxSelectTool _boxSelectTool;
        private Vector2Int[] _previousSelectedCells;
        private Vector2Int[] _toBeSelectedCells;
        public SelectCommand(UBoxSelectTool boxSelectTool, Vector2Int[] selectedCells, Vector2Int[] toBeSelectedCells)
        {
            this._boxSelectTool = boxSelectTool;
            _previousSelectedCells = selectedCells;
            _toBeSelectedCells = toBeSelectedCells;
        }
        public void Execute()
        {
            //_boxSelectTool.SelectedCells = _toBeSelectedCells;
            //_boxSelectTool.SetLevelEditorSelectedCells(_toBeSelectedCells);
        }

        public void Undo()
        {
            //_boxSelectTool.SelectedCells = _previousSelectedCells;
            //_boxSelectTool.SetLevelEditorSelectedCells(_previousSelectedCells);
        }
    }
}
