using MoreMountains.Tools;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ultra.LevelEditor
{
    public partial class USelection : SerializedMonoBehaviour
    {
        public struct ScaleDraggersData
        {
            public Vector3 leftBottom, leftTop, rightBottom, rightTop;
            public ScaleDraggersData(Vector3 leftBottom, Vector3 leftTop, Vector3 rightBottom, Vector3 rightTop)
            {
                this.leftBottom = leftBottom; this.leftTop = leftTop; this.rightBottom = rightBottom; this.rightTop = rightTop;
            }
        }

        [Header("Scaler")]
        public UScaleDraggerManager ScaleDraggerManager;
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
        public Rect DetermineBoundingBox()
        {
            float xMin, yMin, xMax, yMax;
            xMin = int.MaxValue; yMin = int.MaxValue; xMax = int.MinValue; yMax = int.MinValue;
            foreach (int y in SelectedLineDict.Keys)
            {
                if (y < yMin)
                {
                    yMin = y;
                }
                if (y > yMax)
                {
                    yMax = y;
                }
                for (int i = 0; i < SelectedLineDict[y].Length; i++)
                {
                    for (int ix = 0; ix < SelectedLineDict[y][i].Length; ix++)
                    {
                        if (SelectedLineDict[y][i][ix] < xMin)
                        {
                            xMin = SelectedLineDict[y][i][ix];
                        }
                        if (SelectedLineDict[y][i][ix] > xMax)
                        {
                            xMax = SelectedLineDict[y][i][ix];
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
        private void UpdateScaleDraggersWorldPositions(Rect selectionBoundingBox)
        {
            ScaleDraggerManager.SetScaleDraggersWorldPositions(new ScaleDraggersData(
                    SelectionBoundingBoxLeftBottom(selectionBoundingBox),
                    SelectionBoundingBoxLeftTop(selectionBoundingBox),
                    SelectionBoundingBoxRightBottom(selectionBoundingBox),
                    SelectionBoundingBoxRightTop(selectionBoundingBox)
                ));
        }
    }
}
