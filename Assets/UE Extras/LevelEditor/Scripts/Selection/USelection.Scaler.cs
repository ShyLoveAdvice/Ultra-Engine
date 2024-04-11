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

namespace Ultra.LevelEditor
{
    public partial class USelection : SerializedMonoBehaviour
    {
        public enum RectCornerDirections { LeftBottom, LeftTop, RightBottom, RightTop }
        public struct ScaleData
        {
            public bool isEmpty;
            public ScaleData(bool isEmpty)
            {
                this.isEmpty = isEmpty;
            }
        }

        [Header("Scaler")]
        public UScaleDragger LeftBottomDragger;
        public UScaleDragger LeftTopDragger;
        public UScaleDragger RightBottomDragger;
        public UScaleDragger RightTopDragger;
        public RectTransform DraggersParent;

        private bool _begunDragging;
        private Rect _currentScaleDraggerRect;
        private RectCornerDirections _scalingDir;
        private RectCornerDirections _scaleOriginDir;
        private Vector3Int _initialScalingPos;
        private Vector3Int _scalingPos;
        private Vector3Int _scaleOriginWorldPos;
        private Vector3Int _lastPositionShift;
        private Dictionary<RectCornerDirections, UScaleDragger> _cornerScaleDraggerDict = new Dictionary<RectCornerDirections, UScaleDragger> ();
        private Dictionary<RectCornerDirections, Vector3> _cornerWorldPosDict = new Dictionary<RectCornerDirections, Vector3>();
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
        public Vector3Int[] ScaleSelectedCells(Vector3Int scalingPos, Vector3Int scaleOrigin)
        {
            switch(_scalingDir)
            {
                case RectCornerDirections.RightTop: break;
                
            }

            Dictionary<Vector3Int, ScaleData>  ScaledCellPosesScaleDataDict = new Dictionary<Vector3Int, ScaleData>();

            Vector3Int scaleOriginCellPos = RectCornerToCellPos(scaleOrigin, _scaleOriginDir);
            Vector3Int[] distanceVectorsToOrigin = GetVectorsToPoint(GetAllCellsInSelectedRect(_currentScaleDraggerRect), scaleOriginCellPos);

            Vector3Int initialDistanceVector = _initialScalingPos - scaleOrigin;
            Vector3Int currentDistanceVector = scalingPos - scaleOrigin;

            float xScale = (float)currentDistanceVector.x / (float)initialDistanceVector.x;
            float yScale = (float)currentDistanceVector.y / (float)initialDistanceVector.y;

            int xDisMult = initialDistanceVector.x >= 0 ? 1 : -1; xDisMult *= (int)Mathf.Sign(xScale);
            int yDisMult = initialDistanceVector.y >= 0 ? 1 : -1; yDisMult *= (int)Mathf.Sign(yScale);

            int xStartMinus = xScale >= 0 ? 0 : 1;
            int yStartMinus = yScale >= 0 ? 0 : 1;

            xScale = MathF.Abs(xScale); yScale = MathF.Abs(yScale);

            for (int i = 0; i < distanceVectorsToOrigin.Length; i++)
            {
                int unscaledXDis = distanceVectorsToOrigin[i].x;
                int unscaledYDis = distanceVectorsToOrigin[i].y;

                int scaledXDis = Mathf.RoundToInt(unscaledXDis * xScale);
                int scaledYDis = Mathf.RoundToInt(unscaledYDis * yScale);

                Vector3Int distanceVector = distanceVectorsToOrigin[i];
                if (initialDistanceVector.x < 0) distanceVector.x *= -1;
                if(initialDistanceVector.y < 0) distanceVector.y *= -1;

                ScaleData currentScaleData = new ScaleData(!Contains(distanceVector + scaleOriginCellPos));

                for (int y = 1; y <= Mathf.Abs(scaledYDis); y++)
                {
                    for (int x = 1; x <= Mathf.Abs(scaledXDis); x++)
                    {
                        Vector3Int scaledCellPos = new Vector3Int((x - xStartMinus) * xDisMult + scaleOriginCellPos.x, (y - yStartMinus) * yDisMult + scaleOriginCellPos.y);
                        if (!ScaledCellPosesScaleDataDict.ContainsKey(scaledCellPos))
                        {
                            ScaledCellPosesScaleDataDict[scaledCellPos] = currentScaleData;
                        }
                    }
                }
            }

            List<Vector3Int> scaledCells = new List<Vector3Int>();
            foreach (KeyValuePair<Vector3Int, ScaleData> cellPosScaleData in ScaledCellPosesScaleDataDict)
            {
                if (!cellPosScaleData.Value.isEmpty)
                {
                    scaledCells.Add(cellPosScaleData.Key);
                }
            }

            return scaledCells.ToArray();
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
        private ScaleData[] GetScaleDatasInSelectedRect(Rect selectedRect)
        {
            List<ScaleData> allCells = new List<ScaleData>();

            int index = 0;
            for (int x = 0; x < selectedRect.width; x++)
            {
                for (int y = 0; y < selectedRect.height; y++)
                {
                    Vector3Int cellPos = new Vector3Int(x + (int)selectedRect.min.x, y + (int)selectedRect.min.y);
                    bool isEmpty = Contains(cellPos);
                    ScaleData scaleData = new ScaleData(isEmpty);
                    allCells.Add(scaleData);
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
                if(d1.x != d2.x)
                {
                    return d1.x.CompareTo(d2.x);
                }
                else if(d1.y != d2.y)
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
        public Rect DetermineBoundingBox(Vector3[] previewPoints)
        {
            float xMin, yMin, xMax, yMax;
            xMin = int.MaxValue; yMin = int.MaxValue; xMax = int.MinValue; yMax = int.MinValue;
            for (int i = 0; i < previewPoints.Length; i++)
            {
                if (previewPoints[i].x > xMax)
                {
                    xMax = previewPoints[i].x;
                }
                if (previewPoints[i].x < xMin)
                {
                    xMin = previewPoints[i].x;
                }
                if (previewPoints[i].y > yMax)
                {
                    yMax = previewPoints[i].y;
                }
                if (previewPoints[i].y < yMin)
                {
                    yMin = previewPoints[i].y;
                }
            }
            Rect selectedBoundingBox = new Rect();
            selectedBoundingBox.xMin = xMin;
            selectedBoundingBox.yMin = yMin;
            selectedBoundingBox.xMax = xMax;
            selectedBoundingBox.yMax = yMax;

            return selectedBoundingBox;
        }
        public Rect DetermineBoundingBox(Vector3Int[] previewPoints)
        {
            float xMin, yMin, xMax, yMax;
            xMin = int.MaxValue; yMin = int.MaxValue; xMax = int.MinValue; yMax = int.MinValue;
            for (int i = 0; i < previewPoints.Length; i++)
            {
                if (previewPoints[i].x > xMax)
                {
                    xMax = previewPoints[i].x;
                }
                if (previewPoints[i].x < xMin)
                {
                    xMin = previewPoints[i].x;
                }
                if (previewPoints[i].y > yMax)
                {
                    yMax = previewPoints[i].y;
                }
                if (previewPoints[i].y < yMin)
                {
                    yMin = previewPoints[i].y;
                }
            }
            Rect selectedBoundingBox = new Rect();
            selectedBoundingBox.xMin = xMin;
            selectedBoundingBox.yMin = yMin;
            selectedBoundingBox.xMax = xMax;
            selectedBoundingBox.yMax = yMax;

            return selectedBoundingBox;
        }
        private void SetScaleDraggersWorldPos(Rect selectionBoundingBox)
        {
            Vector3 leftBottom = SelectionBoundingBoxLeftBottom(selectionBoundingBox);
            Vector3 leftTop = SelectionBoundingBoxLeftTop(selectionBoundingBox);
            Vector3 rightBottom = SelectionBoundingBoxRightBottom(selectionBoundingBox);
            Vector3 rightTop = SelectionBoundingBoxRightTop(selectionBoundingBox);

            LeftBottomDragger.SetWorldPos(leftBottom);
            LeftTopDragger.SetWorldPos(leftTop);
            RightBottomDragger.SetWorldPos(rightBottom);
            RightTopDragger.SetWorldPos(rightTop);

            _cornerWorldPosDict.Clear();
            _cornerWorldPosDict.Add(RectCornerDirections.LeftBottom, leftBottom);
            _cornerWorldPosDict.Add(RectCornerDirections.LeftTop, leftTop);
            _cornerWorldPosDict.Add(RectCornerDirections.RightBottom, rightBottom);
            _cornerWorldPosDict.Add(RectCornerDirections.RightTop, rightTop);

            _cornerScaleDraggerDict.Clear();
            _cornerScaleDraggerDict.Add(RectCornerDirections.LeftBottom, LeftBottomDragger);
            _cornerScaleDraggerDict.Add(RectCornerDirections.LeftTop, LeftTopDragger);
            _cornerScaleDraggerDict.Add(RectCornerDirections.RightBottom, RightBottomDragger);
            _cornerScaleDraggerDict.Add(RectCornerDirections.RightTop, RightTopDragger);

            _currentScaleDraggerRect = selectionBoundingBox;
        }
        private void TurnOnScaleDraggers()
        {
            LeftBottomDragger.TurnOnScaleDragger();
            LeftTopDragger.TurnOnScaleDragger();
            RightBottomDragger.TurnOnScaleDragger();
            RightTopDragger.TurnOnScaleDragger();
        }
        private void TurnOffScaleDraggers()
        {
            LeftBottomDragger.TurnOffScaleDragger();
            LeftTopDragger.TurnOffScaleDragger();
            RightBottomDragger.TurnOffScaleDragger();
            RightTopDragger.TurnOffScaleDragger();
        }
        public void BeginDrag(UScaleDragger dragging)
        {
            _begunDragging = true;

            if (dragging == LeftBottomDragger)
            {
                _scalingDir = RectCornerDirections.LeftBottom; _scaleOriginDir = RectCornerDirections.RightTop;
            }
            if (dragging == LeftTopDragger)
            {
                _scalingDir = RectCornerDirections.LeftTop; _scaleOriginDir = RectCornerDirections.RightBottom;
            }
            if (dragging == RightBottomDragger)
            {
                _scalingDir = RectCornerDirections.RightBottom; _scaleOriginDir = RectCornerDirections.LeftTop;
            }
            if (dragging == RightTopDragger)
            {
                _scalingDir = RectCornerDirections.RightTop; _scaleOriginDir = RectCornerDirections.LeftBottom;
            }

            _scalingPos = _cornerWorldPosDict[_scalingDir].MMVector3Int();
            _initialScalingPos = _scalingPos;
            _scaleOriginWorldPos = _cornerWorldPosDict[_scaleOriginDir].MMVector3Int();
        }
        public void Drag(Vector3Int positionShift)
        {
            if (_lastPositionShift != positionShift)
            {
                _scalingPos = _initialScalingPos + positionShift;

                Vector3Int[] scaledCells = ScaleSelectedCells(_scalingPos, _scaleOriginWorldPos);
                BuildActiveSelectionData(scaledCells);
                DrawActive();

                Rect _currentRect = DetermineBoundingBox(scaledCells);

                
            }

            _lastPositionShift = positionShift;
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
