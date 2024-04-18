using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ultra.LevelEditor
{
    public partial class USelection: SerializedMonoBehaviour
    {
        public bool SomethingSelected
        {
            get => !NothingSelected;
        }
        public bool NothingSelected
        {
            get
            {
                if(SelectedLineDict != null)
                {
                    if(SelectedLineDict.Count == 0)
                    {
                        return true;
                    }
                    return false;
                }
                return true;
            }
        }
        public ULevelEditor LevelEditor {  get; private set; }
        public void InitializeSelection(ULevelEditor levelEditor)
        {
            LevelEditor = levelEditor;

            ActiveLineDict = new Dictionary<int, int[][]>();
            SelectedLineDict = new Dictionary<int, int[][]>();
            _selectedTileDatas = new UTileData[0];
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
            _selectedCellPoses = new Vector3Int[0];

            ClearSelectedData();
            ClearDrawnSelected();

            _originalSelectedCellPoses = new Vector3Int[0];
            _moveOrigin = Vector3Int.zero;

            DrawTiles(_selectedTileDatas);
            ClearSelectedTileData();
        }
        public void ClearAndSetSelectedTiles()
        {
            _originalSelectedCellPoses = new Vector3Int[0];
            _moveOrigin = Vector3Int.zero;

            DrawTiles(_selectedTileDatas);
            ClearSelectedTileData();
        }

        public void MoveSelected(Vector3Int movedDis)
        {
            if(SomethingSelected)
            {
                _selectedCellPoses = new Vector3Int[_originalSelectedCellPoses.Length];
                for (int i = 0; i < _selectedCellPoses.Length; i++)
                {
                    _selectedCellPoses[i] = _originalSelectedCellPoses[i] + movedDis + _moveOrigin;
                }

                BuildSelectedData(_selectedCellPoses);
                DrawSelected();

                Vector3Int movedPos;
                for (int i = 0; i < _selectedTileDatas.Length; i++)
                {
                    _selectedTileDatasDict.Remove(_selectedTileDatas[i].Pos);
                    _selectedTileDatas[i] = _originalSelectedTileDatas[i];
                    movedPos = _originalSelectedTileDatas[i].Pos + movedDis + _moveOrigin;
                    
                    _selectedTileDatas[i].Pos = movedPos;
                    if (!_selectedTileDatasDict.ContainsKey(movedPos))
                    {
                        _selectedTileDatasDict.Add(movedPos, _selectedTileDatas[i]);
                    }
                    else
                    {
                        _selectedTileDatasDict[movedPos] = _selectedTileDatas[i];
                    }
                }

                LevelEditor.PreviewLayer.ClearPreviewTiles();
                LevelEditor.PreviewLayer.DrawPreviewTiles(_selectedTileDatas);
            }
        }
        public void MoveSelectedMouseUp(Vector3Int currentDistance)
        {
            _moveOrigin += currentDistance;
        }

        public void BuildActive(Vector3Int[] selectedCellPoses)
        {
            BuildActiveData(selectedCellPoses);
            DrawActive();
        }
        public void BuildActive(Vector3Int selectedBoxMin, Vector3Int selectedBoxMax)
        {
            int xMin = selectedBoxMin.x <= selectedBoxMax.x ? selectedBoxMin.x : selectedBoxMax.x;
            int xMax = xMin == selectedBoxMin.x ? selectedBoxMax.x : selectedBoxMin.x;
            int yMin = selectedBoxMin.y <= selectedBoxMax.y ? selectedBoxMin.y : selectedBoxMax.y;
            int yMax = yMin == selectedBoxMin.y ? selectedBoxMax.y : selectedBoxMin.y;

            BuildBoxActiveData(xMin, yMin, xMax, yMax);
            DrawActive();
        }
        public void BuildSelected(Vector3Int[] selectedCellPoses)
        {
            BuildSelectedData(selectedCellPoses);
            SaveSelectedData();
            BuildSelectedTiles(selectedCellPoses);
            DrawSelected();
        }
        public void BuildSelected(Vector3Int[] selectedCellPoses, UTileData[] tileDatas, bool eraseSelectedTiles = true)
        {
            BuildSelectedData(selectedCellPoses);
            SaveSelectedData();
            BuildSelectedTiles(selectedCellPoses, tileDatas, eraseSelectedTiles);
            DrawSelected();
        }
        public void BuildSelected(Vector3Int selectedBoxMin, Vector3Int selectedBoxMax)
        {
            int xMin = selectedBoxMin.x <= selectedBoxMax.x ? selectedBoxMin.x : selectedBoxMax.x;
            int xMax = xMin == selectedBoxMin.x ? selectedBoxMax.x : selectedBoxMin.x;
            int yMin = selectedBoxMin.y <= selectedBoxMax.y ? selectedBoxMin.y : selectedBoxMax.y;
            int yMax = yMin == selectedBoxMin.y ? selectedBoxMax.y : selectedBoxMin.y;

            BuildBoxSelectedData(xMin, yMin, xMax, yMax);
            SaveSelectedData();
            BuildSelectedTiles(_selectedCellPoses);
            DrawSelected();
        }
        public void BuildSelectedAdditive(Vector3Int selectedBoxMin, Vector3Int selectedBoxMax)
        {
            Debug.Log($"_selectedCellPoses.Length (Before Additive Select): {_selectedCellPoses.Length}");
            Vector3Int[] originalSelectedCellPoses = new Vector3Int[_selectedCellPoses.Length];
            for (int i = 0; i < originalSelectedCellPoses.Length; i++)
            {
                originalSelectedCellPoses[i] = _selectedCellPoses[i];
            }

            int xMin = selectedBoxMin.x <= selectedBoxMax.x ? selectedBoxMin.x : selectedBoxMax.x;
            int xMax = xMin == selectedBoxMin.x ? selectedBoxMax.x : selectedBoxMin.x;
            int yMin = selectedBoxMin.y <= selectedBoxMax.y ? selectedBoxMin.y : selectedBoxMax.y;
            int yMax = yMin == selectedBoxMin.y ? selectedBoxMax.y : selectedBoxMin.y;

            BuildBoxAdditiveSelectedData(xMin, yMin, xMax, yMax);
            SaveSelectedData();
            Vector3Int[] selectedCellPosesAdditive = _selectedCellPoses.Except(originalSelectedCellPoses).ToArray();
            Debug.Log($"_selectedCellPoses.Length: {_selectedCellPoses.Length}");
            Debug.Log($"selectedCellPosesAdditive.Length: {selectedCellPosesAdditive.Length}");
            for (int i = 0; i < selectedCellPosesAdditive.Length; i++)
            {
                Debug.Log(selectedCellPosesAdditive[i]);
            }
            BuildSelectedTilesAdditive(selectedCellPosesAdditive);
            DrawSelected();
        }

        public void RebuildSelected()
        {
            BuildSelected(_selectedCellPoses);
        }

        public bool Contains(Vector3Int cellPos)
        {
            return IsInSelection(cellPos.x, cellPos.y, SelectedLineDict);
        }

        public Vector3Int[] GetSelectedTiles()
        {
            var selectedTiles = LevelEditor.CurrentLayer.GetTilePoses(GetSelectedCells());
            return selectedTiles;
        }
        public Vector3Int[] GetSelectedPreviewTiles()
        {
            var selectedTiles = LevelEditor.CurrentLayer.GetPreviewTilePoses(GetSelectedCells());
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
            var selectedTileDatas = LevelEditor.CurrentLayer.GetTileDatas(GetSelectedCells());
            return selectedTileDatas;
        }

        private void BuildSelectedTiles(Vector3Int[] selectedCellPoses, UTileData[] tileDatas = null, bool eraseSelectedTiles = true)
        {
            if (tileDatas == null)
            {
                BuildSelectedTileData(selectedCellPoses);
            }
            else
            {
                BuildSelectedTileData(tileDatas);
            }
            LevelEditor.PreviewLayer.DrawPreviewTiles(_selectedTileDatas);

            if(eraseSelectedTiles)
            {
                LevelEditor.CurrentLayer.EraseTiles(_selectedTileDatas);
            }
        }
        private void BuildSelectedTilesAdditive(Vector3Int[] selectedCellPosesAdditive)
        {
            BuildSelectedTileDataAdditive(selectedCellPosesAdditive);
            LevelEditor.PreviewLayer.DrawPreviewTiles(_selectedTileDatas);
            LevelEditor.CurrentLayer.EraseTiles(selectedCellPosesAdditive);
        }
        private void SaveSelectedData()
        {
            _originalSelectedCellPoses = GetSelectedCells();
            _selectedCellPoses = _originalSelectedCellPoses;
        }
        private bool IsInSelection(int cellX, int cellY, Dictionary<int, int[][]> selectionLineDict)
        {
            if(selectionLineDict.ContainsKey(cellY))
            {
                for(int i = 0; i < selectionLineDict[cellY].Length; i++)
                {
                    if(cellX >= selectionLineDict[cellY][i][LINEXMIN] && cellX <= selectionLineDict[cellY][i][LINEXMAX])
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
