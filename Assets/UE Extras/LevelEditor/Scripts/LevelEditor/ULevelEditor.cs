using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using UnityEngine.Tilemaps;
using System;
using Sirenix.OdinInspector;

namespace Ultra.LevelEditor
{
    public enum ULevelEditorToolTypes
    {
        None, Brush, BoxSelect, Move, Eraser, Line, Circle, CircleSelect
    }
    public class ULevelEditor : MMSingleton<ULevelEditor>
    {
        public UPreviewLayer PreviewLayer;
        public ULevelLayer CurrentLayer;

        [ReadOnly] public ULevelEditorToolTypes CurrentTool;

        [Header("BoxSelectTool")]
        public GameObject ActiveBox;
        public GameObject SelectedBox;
        protected UToolPersistence ToolPersistence { get; private set; }
        protected UBrushTool BrushTool { get; private set; }
        public UBoxSelectTool BoxSelectTool { get; private set; }
        public USelection Selection {  get; private set; }
        protected UMoveTool MoveTool { get; private set; }
        protected UEraserTool EraserTool { get; private set;}
        protected ULineTool LineTool { get; private set; }
        protected UCircleTool CircleTool { get; private set; }
        protected UCircleSelectTool CircleSelectTool { get; private set; }
        [Header("Level Data")]
        public string LevelName;
        [TextArea]
        public string LevelDescription;
        public Vector2Int LevelSize;
        public Vector2Int LevelViewSize = new Vector2Int(200, 200);
        public Bounds LevelViewBound;

        [Header("Camera")]
        public ULevelEditorCameraData CameraData;
        protected ULevelEditorCamera EditorCamera { get; private set; }
        public ULevelEditorGUIManager GUIManager { get; private set; }

        [Header("Grids")]
        public GridDrawerData GridDrawerData;

        [Header("Fade")]
        public float FadeOutDuration;
        public MMTweenType FadeOutTweenType;

        public TileBase CurrentTileBase;
        public ULevelEditorInputManager InputManager {  get; private set; }
        public ULevelEditorGridDrawer GridDrawer { get; private set; }

        protected ULevelData _levelData;
        public Vector3Int[] PaintedTiles { get => BrushTool._paintedTiles; set => BrushTool._paintedTiles = value; }
        public Vector3Int[] PreviewTiles { get; set; }
        protected override void Awake()
        {
            base.Awake();

            Initialize();
        }
        //[Button]
        //public void TestSelectionBoundingBox()
        //{
        //    Selection.DetermineBoundingBox(Selection.SelectedSelectionLineDict);
        //}
        private void Update()
        {
            InputManager.ReadInput();

            CurrentTileBase = GUIManager.CurrentSelectedTileBase();
            CurrentTool = GUIManager.CurrentSelectedTool();

            GUIManager.UpdateGUIs();

            UpdateTools();

            if(Input.GetKeyDown(KeyCode.Z))
            {
                LevelEditorCommandInvoker.UndoCommand();
            }
            if(Input.GetKeyDown(KeyCode.Y))
            {
                LevelEditorCommandInvoker.RedoCommand();
            }
        }
        private void LateUpdate()
        {
            EditorCamera.CameraUpdate();
        }
        #region Initialization
        protected virtual void Initialize()
        {
            MMFadeOutEvent.Trigger(FadeOutDuration, FadeOutTweenType);

            _levelData = new ULevelData(LevelName, LevelDescription, LevelSize);

            InitializeGrids();

            InitializeLevelEditorCam();

            InitializeInputManager();

            InitializeTools();

            InitializeGUIs();

            InitializeSelection();
        }
        protected virtual void InitializeGrids()
        {
            LevelViewBound = new Bounds(new Vector2(0, 0), new Vector2(LevelViewSize.x, LevelViewSize.y));

            GridDrawer = new ULevelEditorGridDrawer(this);
        }
        private void InitializeLevelEditorCam()
        {
            EditorCamera = new ULevelEditorCamera(this);
        }
        private void InitializeInputManager()
        {
            InputManager = FindObjectOfType<ULevelEditorInputManager>();
            InputManager.InitializeInputManager(this);
        }
        private void InitializeTools()
        {
            ToolPersistence = new UToolPersistence(this);

            BrushTool = new UBrushTool(this, ULevelEditorToolTypes.Brush);
            BoxSelectTool = new UBoxSelectTool(this, ULevelEditorToolTypes.BoxSelect);
            MoveTool = new UMoveTool(this, ULevelEditorToolTypes.Move);
            EraserTool = new UEraserTool(this, ULevelEditorToolTypes.Eraser);
            LineTool = new ULineTool(this, ULevelEditorToolTypes.Line);
            CircleTool = new UCircleTool(this, ULevelEditorToolTypes.Circle);
            CircleSelectTool = new UCircleSelectTool(this, ULevelEditorToolTypes.CircleSelect);
        }
        private void InitializeGUIs()
        {
            GUIManager = FindObjectOfType<ULevelEditorGUIManager>();
            GUIManager.InitializeGUIManager(this);
        }
        private void InitializeSelection()
        {
            //Selection = new USelection(this);
            Selection = FindObjectOfType<USelection>();
            Selection.InitializeSelection(this);
        }
        #endregion
        #region Update
        private void UpdateTools()
        {
            ToolPersistence.HandleInput();

            BrushTool.HandleInput();
            BoxSelectTool.HandleInput();
            MoveTool.HandleInput();
            EraserTool.HandleInput();
            LineTool.HandleInput();
            CircleTool.HandleInput();
            CircleSelectTool.HandleInput();
        }
        #endregion
        #region Events
        public void ToolSelected(ULevelEditorToolTypes toolType)
        {
            BrushTool.Select(toolType);
            BoxSelectTool.Select(toolType);
            MoveTool.Select(toolType);
            EraserTool.Select(toolType);
            LineTool.Select(toolType);
            CircleTool.Select(toolType);
            CircleSelectTool.Select(toolType);
        }
        public void ToolUnSelected(ULevelEditorToolTypes toolType)
        {
            BrushTool.UnSelect(toolType);
            BoxSelectTool.UnSelect(toolType);
            MoveTool.UnSelect(toolType);
            EraserTool.UnSelect(toolType);
            LineTool.UnSelect(toolType);
            CircleTool.UnSelect(toolType);
            CircleSelectTool.UnSelect(toolType);
        }
        #endregion
    }
}
