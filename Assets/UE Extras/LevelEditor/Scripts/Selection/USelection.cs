using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
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
        public USelection(ULevelEditor levelEditor)
        {
            LevelEditor = levelEditor;
        }
        public void InitializeSelection(ULevelEditor levelEditor)
        {
            LevelEditor = levelEditor;

            ActiveLineDict = new Dictionary<int, int[][]>();
            SelectedLineDict = new Dictionary<int, int[][]>();
            _selectedTiles = new Vector3Int[0];
            _toBeRemovedIndexes = new int[0];

            _shapesPoints = new List<List<Vector2>>();
            _lineSegmentsDict = new Dictionary<Vector2, List<LineSegment>>();
            _shapePoints = new List<Vector2>();
            _drawnActiveLR = new List<LineRenderer>();
            _drawnSelectedLRs = new List<LineRenderer>();
            _drawnSelectedPointsDict = new Dictionary<LineRenderer, Vector3[]>();
        }
        public void ClearActive()
        {
            ClearActiveData();
            ClearDrawnActive();
        }
        public void ClearSelection()
        {
            ClearSelectionData();
            ClearDrawnSelected();
        }
        public void BuildSelection(Vector3Int[] selectedCellPoses)
        {
            BuildSelectionData(selectedCellPoses);
        }
        public void BuildBoxSelection(Vector2Int boxStart, Vector2Int boxEnd)
        {
            int xMin = boxStart.x <= boxEnd.x? boxStart.x : boxEnd.x;
            int xMax = xMin == boxStart.x? boxEnd.x : boxStart.x;
            int yMin = boxStart.y <= boxEnd.y? boxStart.y : boxEnd.y;
            int yMax = yMin == boxStart.y? boxEnd.y : boxStart.y;
            
            BuildBoxSelectionData(xMin, yMin, xMax, yMax);
        }
        public void BuildBoxSelectionActive(Vector2Int boxStart, Vector2Int boxEnd)
        {
            int xMin = boxStart.x <= boxEnd.x ? boxStart.x : boxEnd.x;
            int xMax = xMin == boxStart.x ? boxEnd.x : boxStart.x;
            int yMin = boxStart.y <= boxEnd.y ? boxStart.y : boxEnd.y;
            int yMax = yMin == boxStart.y ? boxEnd.y : boxStart.y;

            BuildBoxSelectionActiveData(xMin, yMin, xMax, yMax);
        }
        public void BuildBoxSelectionAdditive(Vector2Int boxStart, Vector2Int boxEnd)
        {
            int xMin = boxStart.x <= boxEnd.x ? boxStart.x : boxEnd.x;
            int xMax = xMin == boxStart.x ? boxEnd.x : boxStart.x;
            int yMin = boxStart.y <= boxEnd.y ? boxStart.y : boxEnd.y;
            int yMax = yMin == boxStart.y ? boxEnd.y : boxStart.y;

            BuildBoxSelectionDataAdditive(xMin, yMin, xMax, yMax);
        }
        public bool Contains(Vector2Int cellPos)
        {
            return IsInSelection(cellPos.x, cellPos.y, SelectedLineDict);
        }
        public bool Contains(Vector3Int cellPos)
        {
            return IsInSelection(cellPos.x, cellPos.y, SelectedLineDict);
        }
        public bool Contains(Vector2 cellPos)
        {
            return IsInSelection((int)cellPos.x, (int)cellPos.y, SelectedLineDict);
        }
        public bool Contains(Vector3 cellPos)
        {
            return IsInSelection((int)cellPos.x, (int)cellPos.y, SelectedLineDict);
        }
        public Vector3Int[] GetSelectedTiles()
        {
            _selectedTiles = LevelEditor.CurrentLayer.GetTilePoses(GetSelectedCells());
            return _selectedTiles;
        }
        public Vector3Int[] GetSelectedPreviewTiles()
        {
            _selectedTiles = LevelEditor.CurrentLayer.GetPreviewTilePoses(GetSelectedCells());
            return _selectedTiles;
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
