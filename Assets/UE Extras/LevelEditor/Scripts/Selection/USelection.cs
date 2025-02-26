using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ultra.LevelEditor
{
    public partial class USelection : SerializedMonoBehaviour
    {
        public bool SomethingSelected
        {
            get => !NothingSelected;
        }
        public bool NothingSelected
        {
            get
            {
                if (SelectedLineDict != null)
                {
                    if (SelectedLineDict.Count == 0)
                    {
                        return true;
                    }
                    return false;
                }
                return true;
            }
        }
        public ULevelEditor LevelEditor { get; private set; }
        public void InitializeSelection(ULevelEditor levelEditor)
        {
            LevelEditor = levelEditor;

            _selectedDataDict = new Dictionary<Vector3Int, USelectData>();
            _originalSelectedDataDict = new Dictionary<Vector3Int, USelectData>();

            ActiveLineDict = new Dictionary<int, int[][]>();
            SelectedLineDict = new Dictionary<int, int[][]>();
            _toBeRemovedIndexes = new int[0];

            _shapesPoints = new List<List<Vector2>>();
            _lineSegmentsDict = new Dictionary<Vector2, List<LineSegment>>();
            _shapePoints = new List<Vector2>();
            _drawnActiveLR = new List<LineRenderer>();
            _drawnSelectedLRs = new List<LineRenderer>();
            _drawnSelectedPointsDict = new Dictionary<LineRenderer, Vector3[]>();

            for (int i = 0; i < Enum.GetNames(typeof(RectCornerDirections)).Length; i++)
            {
                _cornerScaleDraggerDict.Add((RectCornerDirections)i, ULevelEditorGUIManager.Instance.InstantiateGUltraUI<UScaleDragger>(ScaleDraggerPrefab, DraggersParent.transform));
            }

            TurnOffScaleDraggers();
            DraggingScaleDraggerPlaceHolder.TurnOffScaleDragger();
        }
        public void ClearActive()
        {
            ClearActiveData();
            ClearDrawnActive();
        }
        public void ClearSelected()
        {
            PutDownSelectedTiles();

            ClearSelectedData();
            ClearDrawnSelected();

        }
        public void PutDownSelected()
        {
            _moveOrigin = Vector3Int.zero;
            
            PutDownSelectedTiles();
        }

        public void MoveSelected(Vector3Int movedDis)
        {
            if (SomethingSelected)
            {
                _selectedDataDict.Clear();
                foreach (KeyValuePair<Vector3Int, USelectData> originalSelectData in _originalSelectedDataDict)
                {
                    _selectedDataDict.Add(originalSelectData.Key + movedDis + _moveOrigin, originalSelectData.Value);
                }

                SetSelectedLineDataClear(_selectedDataDict.Keys.ToArray());
                DrawSelectedPeripheral();

                MoveSelectedTiles();
            }
        }
        public void MoveSelectedMouseUp(Vector3Int currentDistance)
        {
            _moveOrigin += currentDistance;
        }

        public void BuildActive(Vector3Int[] selectedCellPoses)
        {
            SetActiveLineDataClear(selectedCellPoses);
            DrawActive();
        }
        public void BuildSelected(Vector3Int[] selectedCellPoses)
        {
            SetSelectedData(selectedCellPoses);
            PickUpSelectedTiles();
            DrawSelectedPeripheral();
        }
        public void BuildSelected(Vector3Int[] selectedCellPoses, USelectData[] selectDatas)
        {
            SetSelectedData(selectedCellPoses, selectDatas);
            PickUpSelectedTiles(false);
            DrawSelectedPeripheral();
        }
        public void BuildSelected(Vector3Int[] selectedCellPoses, UTileData[] tileDatas)
        {
            SetSelectedData(selectedCellPoses, tileDatas);
            PickUpSelectedTiles(false);
            DrawSelectedPeripheral();
        }
        public void BuildSelectedClear(Vector3Int[] selectedCellPoses)
        {
            SetSelectedDataClear(selectedCellPoses);
            PickUpSelectedTiles();
            DrawSelectedPeripheral();
        }

        public void RebuildSelected()
        {
            //PutDownSelected();
            BuildSelectedClear(_selectedDataDict.Keys.ToArray());
        }

        public void DeleteSelectedTiles()
        {
            Vector3Int[] selectedPoses = _selectedDataDict.Keys.ToArray();
            for (int i = 0; i < selectedPoses.Length; i++)
            {
                if (_selectedDataDict[selectedPoses[i]].TileData.Initialized)
                {
                    _selectedDataDict[selectedPoses[i]] = new USelectData(new UTileData());
                }
            }
            SetOriginalSelectedDataClear();
            RebuildSelected();
        }
        public void DeleteSelected()
        {
            PickUpSelectedTiles();
            Vector3Int[] selectedPoses = _selectedDataDict.Keys.ToArray();
            for (int i = 0; i < selectedPoses.Length; i++)
            {
                if (_selectedDataDict[selectedPoses[i]].TileData.Initialized)
                {
                    _selectedDataDict[selectedPoses[i]] = new USelectData(new UTileData());
                }
            }
            ClearSelected();
        }

        public bool Contains(Vector3Int cellPos)
        {
            return IsInSelection(cellPos.x, cellPos.y, SelectedLineDict);
        }

        public Vector3Int[] GetSelectedTilePoses()
        {
            var selectedTiles = LevelEditor.CurrentLayer.GetTilePoses(GetSelectedCells());
            return selectedTiles;
        }
        public Vector3Int[] GetSelectedCells()
        {
            List<Vector3Int> selectedCells = new List<Vector3Int>();

            foreach (int yi in SelectedLineDict.Keys)
            {
                for (int i = 0; i < SelectedLineDict[yi].Length; i++)
                {
                    _lineXMin = SelectedLineDict[yi][i][LINEXMIN]; _lineXMax = SelectedLineDict[yi][i][LINEXMAX];

                    for (int xi = _lineXMin; xi <= _lineXMax; xi++)
                    {
                        selectedCells.Add(new Vector3Int(xi, yi));
                    }
                }
            }

            return selectedCells.ToArray();
        }
        public UTileData[] GetSelectedTileDatas()
        {
            UTileData[] result = new UTileData[_selectedDataDict.Count];
            int index = 0;
            foreach (var selectedData in _selectedDataDict)
            {
                result[index] = selectedData.Value.TileData;
                index++;
            }
            return result;
        }
        
        private bool IsInSelection(int cellX, int cellY, Dictionary<int, int[][]> selectionLineDict)
        {
            if (selectionLineDict.ContainsKey(cellY))
            {
                for (int i = 0; i < selectionLineDict[cellY].Length; i++)
                {
                    if (cellX >= selectionLineDict[cellY][i][LINEXMIN] && cellX <= selectionLineDict[cellY][i][LINEXMAX])
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
