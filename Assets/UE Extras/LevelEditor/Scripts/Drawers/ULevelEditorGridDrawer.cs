using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.PlayerSettings;

namespace Ultra.LevelEditor
{
    [Serializable]
    public class GridDrawerData
    {
        public string LineSortingLayerName = "EditorGrids";

        public float CellSize = 1;

        public float LineWidth = 0.02f;
        public Color LineColor = Color.white;

        public float AxisWidth = 0.03f;
        public Color XAxisColor = Color.red;
        public Color YAxisColor = Color.green;
    }
    public class ULevelEditorGridDrawer
    {
        public int GridXStart { get => _gridXStart; }
        public int GridYStart { get => _gridYStart; }

        private string _lineSortingLayerName;
        private float _cellSize;
        private float _lineWidth;
        private float _axisWidth;

        //TODO: do not use AssetDataBase.Load, it utilizes unity editor so that you cannot build it
        private const string EidtorGridsPath = "Assets/UE Extras/LevelEditor/Prefabs/Drawer/EditorGrids.prefab";
        private const string LineMatPath = "Packages/com.unity.render-pipelines.universal/Runtime/Materials/Sprite-Lit-Default.mat";
        private const string DotPath = "Assets/UE Extras/LevelEditor/Prefabs/Drawer/Dot.prefab";

        private Vector2Int _levelSize;

        private int _gridXStart;
        private int _gridYStart;

        private int _gridXEnd;
        private int _gridYEnd;

        GameObject EditorGridsParent;
        Material lineMaterial;

        public ULevelEditorGridDrawer(ULevelEditor levelEditor)
        {
            _levelSize = levelEditor.LevelSize;

            _lineSortingLayerName = levelEditor.GridDrawerData.LineSortingLayerName;
            _cellSize = levelEditor.GridDrawerData.CellSize;
            _lineWidth = levelEditor.GridDrawerData.LineWidth;
            _axisWidth = levelEditor.GridDrawerData.AxisWidth;

            _gridXStart = levelEditor.LevelStartPos.x;
            _gridYStart = levelEditor.LevelStartPos.y;

            _gridXEnd = levelEditor.LevelEndPos.x;
            _gridYEnd = levelEditor.LevelEndPos.y;

            EditorGridsParent = new GameObject("Editor Grids");
            lineMaterial = AssetDatabase.LoadAssetAtPath<Material>(LineMatPath);

            //Now instead of line renderer, 2d grid material is used instead
            #region Obsolete
            //for (int x = _gridXStart; x < _gridXEnd + 1; x++)
            //{
            //    Vector3[] linePositions = { GetWorldPos(x, _gridYStart), GetWorldPos(x, _gridYEnd) };
            //    DrawLine(linePositions, _lineWidth, "Line_X_" + x, data.LineColor);
            //}
            //for (int y = _gridYStart; y < _gridYEnd + 1; y++)
            //{
            //    Vector3[] linePositions = { GetWorldPos(_gridXStart, y), GetWorldPos(_gridXEnd, y) };
            //    DrawLine(linePositions, _lineWidth, "Line_Y_" + y, data.LineColor);
            //}
            #endregion

            GameObject editorGrids = AssetDatabase.LoadAssetAtPath<GameObject>(EidtorGridsPath);
            GameObject.Instantiate(editorGrids, EditorGridsParent.transform);

            //Draw X Axis
            Vector3[] xAxisPositions = { new Vector3(levelEditor.LevelViewBound.min.x, 0), new Vector3(levelEditor.LevelViewBound.max.x, 0) };
            LineRenderer xAxis = DrawLine(xAxisPositions, _axisWidth, "XAxis", levelEditor.GridDrawerData.XAxisColor);
            xAxis.sortingOrder = 1;

            //Draw Y Axis
            Vector3[] yAxisPositions = { new Vector3(0, levelEditor.LevelViewBound.min.y), new Vector3(0, levelEditor.LevelViewBound.max.y) };
            LineRenderer yAxis = DrawLine(yAxisPositions, _axisWidth, "YAxis", levelEditor.GridDrawerData.YAxisColor);
            yAxis.sortingOrder = 1;

            GameObject dot = AssetDatabase.LoadAssetAtPath<GameObject>(DotPath);
            GameObject.Instantiate(dot, EditorGridsParent.transform);
        }
        protected virtual Vector3 GetWorldPos(int x, int y)
        {
            return new Vector3(x, y) * _cellSize;
        }
        protected virtual LineRenderer DrawLine(Vector3[] linePositions, float width, string name, Color color)
        {
            GameObject newGO = new GameObject(name);
            newGO.transform.SetParent(EditorGridsParent.transform);
            LineRenderer newLineRenderer = newGO.AddComponent<LineRenderer>();
            newLineRenderer.SetPositions(linePositions);
            newLineRenderer.startWidth = width;
            newLineRenderer.endWidth = width;
            newLineRenderer.startColor = color;
            newLineRenderer.endColor = color;
            newLineRenderer.material = lineMaterial;
            newLineRenderer.sortingLayerName = _lineSortingLayerName;
            return newLineRenderer;
        }
    }
}
