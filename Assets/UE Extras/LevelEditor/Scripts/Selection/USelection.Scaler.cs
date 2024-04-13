using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Ultra.LevelEditor
{
    public partial class USelection : SerializedMonoBehaviour
    {
        public enum RectCornerDirections { LeftBottom, LeftTop, RightBottom, RightTop }
        public struct CellData
        {
            public bool IsEmpty;
            public TileBase TileBase;
            public CellData(bool isEmpty, TileBase tileBase)
            {
                this.IsEmpty = isEmpty; this.TileBase = tileBase;
            }
        }

        [Header("Scaler")]
        public RectTransform DraggersParent;
        public GameObject ScaleDraggerPrefab;
        public UScaleDragger DraggingScaleDraggerPlaceHolder;

        public Vector3Int[] scaledCells;
        public Rect ScaledRect;

        private Dictionary<Vector3Int, CellData> _cellDataAtScaledCellPosDict;
        private Vector3Int[] _beforeScaleSelectedTiles;
        private bool _begunDragging;
        private Rect _currentSelectedRect;
        private RectCornerDirections _draggingCornerDir;
        private RectCornerDirections _originCornerDir;
        private Vector3Int _draggingCornerInitialPos;
        private Vector3Int _draggingCornerPos;
        private Vector3Int _originCornerPos;
        private Vector3Int _lastDraggingCornerPos;
        public Dictionary<RectCornerDirections, UScaleDragger> _cornerScaleDraggerDict = new Dictionary<RectCornerDirections, UScaleDragger>();
        private Vector3 SelectionBoundingBoxLeftBottom(Rect selectionBoundingBox)
        {
            return selectionBoundingBox.min;
        }
        private Vector3 SelectionBoundingBoxLeftTop(Rect selectionBoundingBox)
        {
            return new Vector3(selectionBoundingBox.xMin, selectionBoundingBox.yMax);
        }
        private Vector3 SelectionBoundingBoxRightBottom(Rect selectionBoundingBox)
        {
            return new Vector3(selectionBoundingBox.xMax, selectionBoundingBox.yMin);
        }
        private Vector3 SelectionBoundingBoxRightTop(Rect selectionBoundingBox)
        {
            return selectionBoundingBox.max;
        }
        public Dictionary<Vector3Int, CellData> ScaleSelectedCells(Vector3Int scalingPos, Vector3Int scaleOrigin, out Vector3Int[] scaledCells)
        {
            Dictionary<Vector3Int, CellData> cellDataAtScaledCellPosDict = new Dictionary<Vector3Int, CellData>();

            Vector3Int scaleOriginCellPos = RectCornerToCellPos(scaleOrigin, _originCornerDir);
            Vector3Int[] distanceVectorsToOrigin = GetVectorsToPoint(GetAllCellsInSelectedRect(_currentSelectedRect), scaleOriginCellPos);

            Vector3Int initialDistanceVector = _draggingCornerInitialPos - scaleOrigin;
            Vector3Int currentDistanceVector = scalingPos - scaleOrigin;

            float xScale = (float)currentDistanceVector.x / (float)initialDistanceVector.x;
            float yScale = (float)currentDistanceVector.y / (float)initialDistanceVector.y;

            int xDisMult = initialDistanceVector.x >= 0 ? 1 : -1; xDisMult *= (int)Mathf.Sign(xScale);
            int yDisMult = initialDistanceVector.y >= 0 ? 1 : -1; yDisMult *= (int)Mathf.Sign(yScale);

            int xStartMinus = xScale >= 0 ? 0 : 1;
            int yStartMinus = yScale >= 0 ? 0 : 1;

            xScale = MathF.Abs(xScale); yScale = MathF.Abs(yScale);

            int unscaledXDis, unscaledYDis, scaledXDis, scaledYDis, scaledXDisAbs, scaledYDisAbs, xCeil, yCeil;
            xCeil = 1; yCeil = 1;
            Vector3Int currentDis, currentWorldPos;
            CellData currentScaleData;
            int count = 0;
            int lastYDis = 0;int lastXDis = 0;
            int lastScaledXDis = 0;int lastScaledYDis = 0;

            for (int i = 0; i < distanceVectorsToOrigin.Length; i++)
            {
                unscaledXDis = distanceVectorsToOrigin[i].x;
                unscaledYDis = distanceVectorsToOrigin[i].y;

                scaledXDis = Mathf.RoundToInt(unscaledXDis * xScale);
                scaledYDis = Mathf.RoundToInt(unscaledYDis * yScale);

                scaledXDisAbs = Mathf.Abs(scaledXDis);
                scaledYDisAbs = Mathf.Abs(scaledYDis);

                currentDis = distanceVectorsToOrigin[i];
                if (initialDistanceVector.x < 0) currentDis.x *= -1;
                if (initialDistanceVector.y < 0) currentDis.y *= -1;

                currentWorldPos = currentDis + scaleOriginCellPos;
                currentScaleData = new CellData(!Contains(currentWorldPos), LevelEditor.CurrentLayer.GetTileBase(currentWorldPos));

                if(currentDis.x != lastXDis)
                {
                    lastXDis = currentDis.x;
                    xCeil = lastScaledXDis + 1;
                    lastScaledXDis = scaledXDis;
                    lastScaledYDis = 0;
                    lastYDis = 0;
                }

                for (int x = xCeil; x <= scaledXDisAbs; x++)
                {
                    if(currentDis.y != lastYDis)
                    {
                        lastYDis = currentDis.y;
                        yCeil = lastScaledYDis + 1;
                        lastScaledYDis = scaledYDis;
                    }
                    for (int y = yCeil; y <= scaledYDisAbs; y++)
                    {
                        Vector3Int scaledCellPos = new Vector3Int((x - xStartMinus) * xDisMult + scaleOriginCellPos.x, (y - yStartMinus) * yDisMult + scaleOriginCellPos.y);
                        count++;
                        if (!cellDataAtScaledCellPosDict.ContainsKey(scaledCellPos))
                        {
                            cellDataAtScaledCellPosDict[scaledCellPos] = currentScaleData;
                        }
                    }
                }
            }
            Debug.Log(count);

            scaledCells = cellDataAtScaledCellPosDict.Keys.ToArray();
            return cellDataAtScaledCellPosDict;
        }
        private Vector3Int[] GetAllCellsInSelectedRect(Rect selectedRect)
        {
            List<Vector3Int> allCells = new List<Vector3Int>();

            int index = 0;
            for (int x = 0; x < selectedRect.width; x++)
            {
                for (int y = 0; y < selectedRect.height; y++)
                {
                    Vector3Int cellPos = new Vector3Int(x + (int)selectedRect.min.x, y + (int)selectedRect.min.y);
                    allCells.Add(cellPos);
                    index++;
                }
            }
            return allCells.ToArray();
        }
        private Vector3Int[] GetVectorsToPoint(Vector3Int[] cellPoses, Vector3Int point)
        {
            Vector3Int[] result = new Vector3Int[cellPoses.Length];
            for (int i = 0; i < cellPoses.Length; i++)
            {
                int xDis = Mathf.Abs(cellPoses[i].x - point.x);
                int yDis = Mathf.Abs(cellPoses[i].y - point.y);
                result[i] = new Vector3Int(xDis, yDis);
            }
            result.Sort((d1, d2) =>
            {
                if (d1.x != d2.x)
                {
                    return d1.x.CompareTo(d2.x);
                }
                else if (d1.y != d2.y)
                {
                    return d1.y.CompareTo(d2.y);
                }
                else
                {
                    return d1.z.CompareTo(d2.z);
                }
            });
            return result;
        }
        public Rect DetermineBoundingBox(Dictionary<int, int[][]> selectedLineDict)
        {
            float xMin, yMin, xMax, yMax;
            xMin = int.MaxValue; yMin = int.MaxValue; xMax = int.MinValue; yMax = int.MinValue;
            foreach (int y in selectedLineDict.Keys)
            {
                if (y < yMin)
                {
                    yMin = y;
                }
                if (y > yMax)
                {
                    yMax = y;
                }
                for (int i = 0; i < selectedLineDict[y].Length; i++)
                {
                    for (int ix = 0; ix < selectedLineDict[y][i].Length; ix++)
                    {
                        if (selectedLineDict[y][i][ix] < xMin)
                        {
                            xMin = selectedLineDict[y][i][ix];
                        }
                        if (selectedLineDict[y][i][ix] > xMax)
                        {
                            xMax = selectedLineDict[y][i][ix];
                        }
                    }
                }
            }

            xMax++; yMax++;

            Rect selectedBoundingBox = new Rect();
            selectedBoundingBox.xMin = xMin;
            selectedBoundingBox.yMin = yMin;
            selectedBoundingBox.xMax = xMax;
            selectedBoundingBox.yMax = yMax;

            return selectedBoundingBox;
        }
        public Rect DetermineBoundingBox(Vector3[] points)
        {
            float xMin, yMin, xMax, yMax;
            xMin = int.MaxValue; yMin = int.MaxValue; xMax = int.MinValue; yMax = int.MinValue;
            for (int i = 0; i < points.Length; i++)
            {
                if (points[i].x > xMax)
                {
                    xMax = points[i].x;
                }
                if (points[i].x < xMin)
                {
                    xMin = points[i].x;
                }
                if (points[i].y > yMax)
                {
                    yMax = points[i].y;
                }
                if (points[i].y < yMin)
                {
                    yMin = points[i].y;
                }
            }
            Rect selectedBoundingBox = new Rect();
            selectedBoundingBox.xMin = xMin;
            selectedBoundingBox.yMin = yMin;
            selectedBoundingBox.xMax = xMax;
            selectedBoundingBox.yMax = yMax;

            return selectedBoundingBox;
        }
        public Rect DetermineBoundingBox(Vector3Int[] cellPoses)
        {
            float xMin, yMin, xMax, yMax;
            xMin = int.MaxValue; yMin = int.MaxValue; xMax = int.MinValue; yMax = int.MinValue;
            for (int i = 0; i < cellPoses.Length; i++)
            {
                if (cellPoses[i].x > xMax)
                {
                    xMax = cellPoses[i].x;
                }
                if (cellPoses[i].x < xMin)
                {
                    xMin = cellPoses[i].x;
                }
                if (cellPoses[i].y > yMax)
                {
                    yMax = cellPoses[i].y;
                }
                if (cellPoses[i].y < yMin)
                {
                    yMin = cellPoses[i].y;
                }
            }
            Rect selectedBoundingBox = new Rect();
            selectedBoundingBox.xMin = xMin;
            selectedBoundingBox.yMin = yMin;
            selectedBoundingBox.xMax = xMax + 1;
            selectedBoundingBox.yMax = yMax + 1;

            return selectedBoundingBox;
        }
        private void InitializeScaleDraggers(Rect selectionBoundingBox)
        {
            SetScaleDraggersWorldPos(selectionBoundingBox);
            _currentSelectedRect = selectionBoundingBox;
        }
        private void SetScaleDraggersWorldPos(Rect selectionBoundingBox)
        {
            Vector3 leftBottom = SelectionBoundingBoxLeftBottom(selectionBoundingBox);
            Vector3 leftTop = SelectionBoundingBoxLeftTop(selectionBoundingBox);
            Vector3 rightBottom = SelectionBoundingBoxRightBottom(selectionBoundingBox);
            Vector3 rightTop = SelectionBoundingBoxRightTop(selectionBoundingBox);

            Vector3[] cornerWorldPoses = new Vector3[4] { leftBottom, leftTop, rightBottom, rightTop };

            for (int i = 0; i < cornerWorldPoses.Length; i++)
            {
                _cornerScaleDraggerDict[(RectCornerDirections)i].SetWorldPos(cornerWorldPoses[i], (RectCornerDirections)i);
            }

            if(_begunDragging)
            {
                DraggingScaleDraggerPlaceHolder.SetWorldPos(cornerWorldPoses[(int)_draggingCornerDir], _draggingCornerDir);
            }
        }
        private void TurnOnScaleDraggers()
        {
            for (int i = 0; i < _cornerScaleDraggerDict.Values.Count; i++)
            {
                _cornerScaleDraggerDict.Values.ToArray()[i].TurnOnScaleDragger();
            }
        }
        private void TurnOffScaleDraggers()
        {
            for (int i = 0; i < _cornerScaleDraggerDict.Values.Count; i++)
            {
                _cornerScaleDraggerDict.Values.ToArray()[i].TurnOffScaleDragger();
            }
        }
        public void BeginDrag(UScaleDragger dragging)
        {
            _begunDragging = true;
            _draggingCornerDir = dragging.CornerDirection;

            switch (dragging.CornerDirection)
            {
                case RectCornerDirections.LeftBottom:
                    _originCornerDir = RectCornerDirections.RightTop; break;
                case RectCornerDirections.LeftTop:
                    _originCornerDir = RectCornerDirections.RightBottom; break;
                case RectCornerDirections.RightBottom:
                    _originCornerDir = RectCornerDirections.LeftTop; break;
                case RectCornerDirections.RightTop:
                    _originCornerDir = RectCornerDirections.LeftBottom; break;
            }

            _draggingCornerPos = _cornerScaleDraggerDict[_draggingCornerDir].CornerWorldPos;
            _draggingCornerInitialPos = _draggingCornerPos;
            _originCornerPos = _cornerScaleDraggerDict[_originCornerDir].CornerWorldPos;

            DraggingScaleDraggerPlaceHolder.TurnOnScaleDragger();

            _beforeScaleSelectedTiles = GetSelectedPreviewTiles();
            if(_beforeScaleSelectedTiles.Length == 0)
            {
                _beforeScaleSelectedTiles = GetSelectedTiles();
            }
            LevelEditor.CurrentLayer.EraseTilesPreview(_beforeScaleSelectedTiles);
            LevelEditor.PreviewLayer.DrawTilesPreview(_beforeScaleSelectedTiles, LevelEditor.CurrentLayer.GetTileBases(_beforeScaleSelectedTiles));
        }
        public void Drag(Vector3Int draggingCornerPos)
        {
            if (_lastDraggingCornerPos != draggingCornerPos)
            {
                _draggingCornerPos = draggingCornerPos;

                _cellDataAtScaledCellPosDict = ScaleSelectedCells(_draggingCornerPos, _originCornerPos, out scaledCells);
                List<Vector3Int> nonEmptyCellList = new List<Vector3Int>();
                List<TileBase> nonEmptyTileAtCellList = new List<TileBase>();
                if (_cellDataAtScaledCellPosDict != null)
                {
                    foreach (var cellDataAtScaledCellPos in _cellDataAtScaledCellPosDict)
                    {
                        if (!cellDataAtScaledCellPos.Value.IsEmpty)
                        {
                            nonEmptyCellList.Add(cellDataAtScaledCellPos.Key);
                            nonEmptyTileAtCellList.Add(cellDataAtScaledCellPos.Value.TileBase);
                        }
                    }
                }
                LevelEditor.PreviewLayer.ClearPreviewTiles();
                LevelEditor.PreviewLayer.DrawTilesPreview(nonEmptyCellList.ToArray(), nonEmptyTileAtCellList.ToArray());

                if (scaledCells.Length != 0)
                {
                    ScaledRect = DetermineBoundingBox(scaledCells);
                }
                else
                {
                    ScaledRect.x = _originCornerPos.x;
                    ScaledRect.y = _originCornerPos.y;
                    ScaledRect.width = _draggingCornerPos.x - _originCornerPos.x;
                    ScaledRect.height = _draggingCornerPos.y - _originCornerPos.y;
                }
                SetScaleDraggersWorldPos(ScaledRect);

                BuildActiveSelectionData(nonEmptyCellList.ToArray());
                DrawActive();
                _lastDraggingCornerPos = draggingCornerPos;
            }

        }
        public void EndDrag()
        {
            List<Vector3Int> nonEmptyCellList = new List<Vector3Int>();
            List<TileBase> nonEmptyTileAtCellList = new List<TileBase>();
            if (_cellDataAtScaledCellPosDict != null)
            {
                foreach (var cellDataAtScaledCellPos in _cellDataAtScaledCellPosDict)
                {
                    if (!cellDataAtScaledCellPos.Value.IsEmpty)
                    {
                        nonEmptyCellList.Add(cellDataAtScaledCellPos.Key);
                        nonEmptyTileAtCellList.Add(cellDataAtScaledCellPos.Value.TileBase);
                    }
                }

            }
            SetScaleDraggersWorldPos(ScaledRect);
            DraggingScaleDraggerPlaceHolder.TurnOffScaleDragger();

            ClearActive();
            BuildSelection(nonEmptyCellList.ToArray());
            DrawSelected();

            LevelEditor.PreviewLayer.ClearPreviewTiles();
            LevelEditor.CurrentLayer.EraseTiles(_beforeScaleSelectedTiles);
            LevelEditor.CurrentLayer.DrawTiles(nonEmptyCellList.ToArray(), nonEmptyTileAtCellList.ToArray());

            _begunDragging = false;
        }
        private Vector3Int RectCornerToCellPos(Vector3Int draggingWorldPos, RectCornerDirections dir)
        {
            Vector3Int centeredCellPos = draggingWorldPos;
            switch (dir)
            {
                case RectCornerDirections.LeftBottom:
                    centeredCellPos.x -= 1; centeredCellPos.y -= 1; break;
                case RectCornerDirections.LeftTop:
                    centeredCellPos.x -= 1; break;
                case RectCornerDirections.RightBottom:
                    centeredCellPos.y -= 1; break;
                case RectCornerDirections.RightTop:
                    break;

            }
            return centeredCellPos;
        }
    }
}
