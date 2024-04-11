using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ultra.LevelEditor
{
    public struct LineSegment
    {
        public Vector2Int StartPoint;
        public Vector2Int EndPoint;
        public LineSegment(Vector2Int startPoint, Vector2Int endPoint)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
        }
        public override string ToString()
        {
            return new string($"StartPoint: {StartPoint}; EndPoint: {EndPoint}");
        }
    }
    public partial class USelection: SerializedMonoBehaviour
    {
        [Header("Selection Drawer"), Space(10)]
        public GameObject ActiveSelectionPrefab;
        public GameObject selectedSelectionPrefab;

        private List<List<Vector2>> _shapesPoints;
        private Dictionary<Vector2, List<LineSegment>> _lineSegmentsDict;
        private List<Vector2> _shapePoints;

        private LineRenderer _boxActiveLR;
        private List<LineRenderer> _drawnActiveLR;
        private List<LineRenderer> _drawnSelectedLRs;

        private Dictionary<LineRenderer, Vector3[]> _drawnSelectedPointsDict;

        Vector3[] _lrPositions;
        Vector3[] _previewPoints;
        Vector2 _shapeStartPoint;
        Vector2 _currentStartPoint;
        Vector2 _currentEndPoint;
        Vector2 _currentKey;
        Vector2 _endPointKey;
        LineSegment _currentLineSegment;
        float _shapePerimeter;
        bool _setShapeStartPoint;
        bool _foundNextPoint = false;
        bool _examineEndPointKey;
        public void ClearDrawnActive()
        {
            ClearDrawnSelections(ref _drawnActiveLR);
        }
        public void ClearDrawnSelected()
        {
            ClearDrawnSelections(ref _drawnSelectedLRs);

            TurnOffScaleDraggers();
        }
        private void ClearDrawnSelections(ref List<LineRenderer> drawnLRs)
        {
            if (drawnLRs != null)
            {
                drawnLRs.ForEach(lr => GameObject.Destroy(lr.gameObject));
                drawnLRs.Clear();
            }
            else
            {
                drawnLRs = new List<LineRenderer>();
            }
        }

        public void DrawBoxActive()
        {

        }
        public void DrawActive()
        {
            DrawSelection(ActiveSelectionPrefab, ref _drawnActiveLR, ActiveSelectionLineDict);
        }
        public void DrawSelected(bool setScaleDraggers = true)
        {
            DrawSelection(selectedSelectionPrefab, ref _drawnSelectedLRs, SelectedSelectionLineDict);

            if(setScaleDraggers)
            {
                TurnOnScaleDraggers();
                SetScaleDraggersWorldPos(DetermineBoundingBox(SelectedSelectionLineDict));
            }
        }

        public void MoveSelectionPreview(Vector3Int movedDis)
        {
            foreach (LineRenderer lr in _drawnSelectedLRs)
            {
                if(_drawnSelectedPointsDict.ContainsKey(lr))
                {
                    _previewPoints = new Vector3[_drawnSelectedPointsDict[lr].Length];
                    for (int i = 0; i < _previewPoints.Length; i++)
                    {
                        _previewPoints[i].x = _drawnSelectedPointsDict[lr][i].x + movedDis.x;
                        _previewPoints[i].y = _drawnSelectedPointsDict[lr][i].y + movedDis.y;
                    }
                    lr.SetPositions(_previewPoints);
                }
            }

            SetScaleDraggersWorldPos(DetermineBoundingBox(_previewPoints));
        }
        private void SaveDrawnSelectedPoints()
        {
            _drawnSelectedPointsDict.Clear();
            foreach (LineRenderer lr in _drawnSelectedLRs)
            {
                _lrPositions = new Vector3[lr.positionCount];
                lr.GetPositions(_lrPositions);
                _drawnSelectedPointsDict.Add(lr, _lrPositions);
            }
        }
        private void DrawSelection(GameObject selectionPrefab, ref List<LineRenderer> drawnLRs, Dictionary<int, int[][]> selectionLineDict)
        {
            if (_lineSegmentsDict == null)
            {
                _lineSegmentsDict = new Dictionary<Vector2, List<LineSegment>>();
            }
            _lineSegmentsDict.Clear();

            if (_shapesPoints == null)
            {
                _shapesPoints = new List<List<Vector2>>();
            }
            _shapesPoints.Clear();

            ClearDrawnSelections(ref drawnLRs);

            //Determine all line segments of all vertexes
            for (int i = 0; i < selectionLineDict.Count; i++)
            {
                int y = selectionLineDict.ElementAt(i).Key;
                for (int j = 0; j < selectionLineDict[y].Length; j++)
                {
                    for (int x = selectionLineDict[y][j][LINEXMIN]; x <= selectionLineDict[y][j][LINEXMAX]; x++)
                    {
                        List<LineSegment> lineSegments = DetermineLineSegments(x, y, selectionLineDict);
                        if (lineSegments.Count > 0)
                        {
                            _currentKey = new Vector2Int(x, y);
                            if (!_lineSegmentsDict.ContainsKey(_currentKey))
                            {
                                _lineSegmentsDict.Add(_currentKey, lineSegments);
                            }
                            else
                            {
                                for (int a = 0; a < selectionLineDict[y][j].Length; a++)
                                {
                                    Debug.Log(selectionLineDict[y][j][a]);
                                }
                            }
                        }
                    }
                }
            }

            _setShapeStartPoint = true;

            int _numOfTries = 0;
            while (_lineSegmentsDict.Count > 0 && _numOfTries < 10000)
            {
                _numOfTries++;
                _foundNextPoint = false;

                if (_setShapeStartPoint)
                {
                    _shapePoints.Clear();

                    _currentKey = _lineSegmentsDict.ElementAt(0).Key;
                    _currentLineSegment = _lineSegmentsDict[_currentKey][0];

                    _shapeStartPoint = _currentLineSegment.StartPoint;

                    _currentStartPoint = _currentLineSegment.StartPoint;
                    _currentEndPoint = _currentLineSegment.EndPoint;
                    _endPointKey = _currentKey;

                    _examineEndPointKey = true;

                    _shapePoints.Add(_currentStartPoint);

                    _setShapeStartPoint = false;
                }

                if (_examineEndPointKey)
                {
                    _examineEndPointKey = false;

                    if (_lineSegmentsDict.ContainsKey(_endPointKey))
                    {
                        for (int i = 0; i < _lineSegmentsDict[_endPointKey].Count; i++)
                        {
                            _currentLineSegment = _lineSegmentsDict[_endPointKey][i];
                            _currentStartPoint = _currentLineSegment.StartPoint;

                            if (_currentStartPoint == _currentEndPoint)
                            {
                                FoundNewLineSegment(_currentLineSegment);
                                break;
                            }
                        }
                    }
                }

                for (int i = 0; i < _lineSegmentsDict.Count; i++)
                {
                    if (_foundNextPoint)
                    {
                        break;
                    }

                    _currentKey = _lineSegmentsDict.ElementAt(i).Key;
                    for (int j = 0; j < _lineSegmentsDict[_currentKey].Count; j++)
                    {
                        _currentLineSegment = _lineSegmentsDict[_currentKey][j];
                        _currentStartPoint = _currentLineSegment.StartPoint;

                        if (_currentStartPoint == _currentEndPoint)
                        {
                            FoundNewLineSegment(_currentLineSegment);
                            break;
                        }
                    }
                }

                if (!_foundNextPoint || _currentStartPoint == _shapeStartPoint)
                {
                    InstantiateNewSelectionLR(selectionPrefab, ref drawnLRs);
                }

            }

            SaveDrawnSelectedPoints();
        }
        private void FoundNewLineSegment(LineSegment newLineSegment)
        {
            _shapePoints.Add(_currentStartPoint);
            _lineSegmentsDict[_currentKey].Remove(newLineSegment);
            if (_lineSegmentsDict[_currentKey].Count == 0)
            {
                _lineSegmentsDict.Remove(_currentKey);
            }
            _currentEndPoint = newLineSegment.EndPoint;

            _shapePerimeter += 1;
            _foundNextPoint = true;
            _endPointKey = _currentKey;

            _examineEndPointKey = true;
        }
        private void InstantiateNewSelectionLR(GameObject selectionPrefab, ref List<LineRenderer> drawnLRs)
        {
            _shapesPoints.Add(_shapePoints.ToArray().ToList());
            _setShapeStartPoint = true;

            LineRenderer currentLR = GameObject.Instantiate(selectionPrefab, LevelEditor.transform).GetComponent<LineRenderer>();
            drawnLRs.Add(currentLR);
            currentLR.positionCount = _shapePoints.Count;
            Vector3[] shapeVector3Points = new Vector3[_shapePoints.Count];
            for (int i = 0; i < _shapePoints.Count; i++)
            {
                shapeVector3Points[i] = (Vector3)_shapePoints[i];
            }
            currentLR.SetPositions(shapeVector3Points);

            SetTiling(currentLR, _shapePerimeter);
            _shapePerimeter = 0;

            currentLR.enabled = true;
        }
        private void SetTiling(LineRenderer lr, float tilingAmount)
        {
            lr.material.SetFloat("_TilingAmount", tilingAmount);
        }
        private List<LineSegment> DetermineLineSegments(int x, int y, Dictionary<int, int[][]> selectionLineDict)
        {
            bool hasLineOnTop = false;
            bool hasLineOnBottom = false;
            bool hasLineOnRight = false;
            bool hasLineOnLeft = false;

            List<LineSegment> currentLineSegmentList = new List<LineSegment>();

            if (IsInSelection(x, y + 1, selectionLineDict))
            {
                hasLineOnTop = true;
            }
            if (IsInSelection(x, y - 1, selectionLineDict))
            {
                hasLineOnBottom = true;
            }
            if (IsInSelection(x + 1, y, selectionLineDict))
            {
                hasLineOnRight = true;
            }
            if (IsInSelection(x - 1, y, selectionLineDict))
            {
                hasLineOnLeft = true;
            }

            if (!hasLineOnTop)
            {
                currentLineSegmentList.Add(new LineSegment(new Vector2Int(x, y + 1), new Vector2Int(x + 1, y + 1)));
            }
            if (!hasLineOnBottom)
            {
                currentLineSegmentList.Add(new LineSegment(new Vector2Int(x + 1, y), new Vector2Int(x, y)));
            }
            if (!hasLineOnRight)
            {
                currentLineSegmentList.Add(new LineSegment(new Vector2Int(x + 1, y + 1), new Vector2Int(x + 1, y)));
            }
            if (!hasLineOnLeft)
            {
                currentLineSegmentList.Add(new LineSegment(new Vector2Int(x, y), new Vector2Int(x, y + 1)));
            }

            return currentLineSegmentList;
        }
    }
}
