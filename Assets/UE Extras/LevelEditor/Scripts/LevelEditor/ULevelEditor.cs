using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using UnityEngine.Tilemaps;
using System;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine.Serialization;

namespace Ultra.LevelEditor
{
    public enum ULevelEditorToolTypes
    {
        None, Brush, BoxSelect, Move, Eraser, Line, Circle, CircleSelect, Bucket, MagicWand, PrefabPlace
    }
    public class ULevelEditor : MMSingleton<ULevelEditor>
    {
        public UPreviewLayer PreviewLayer;
        public ULevelLayer CurrentLayer;

        [ReadOnly] public ULevelEditorToolTypes CurrentTool;
        public Action ToolPersistenceEvent;
        
        private ULevelEditorTool[] _tools;
        protected UBrushTool BrushTool { get; private set; }
        public UBoxSelectTool BoxSelectTool { get; private set; }
        public USelection Selection {  get; private set; }
        [Header("Level Data")]
        public string LevelName;
        [TextArea]
        public string LevelDescription;

        public Vector3Int LevelStartPos { get; private set; }
        public Vector3Int LevelEndPos { get; private set; }
        public Vector2Int LevelSize;
        public Vector2Int LevelViewSize;
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

        [FormerlySerializedAs("CurrentTileBase")] public TileBase CurrentTile;
        public UPrefabTile CurrentPrefabTile;
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

            CurrentTile = GUIManager.CurrentSelectedTileBase();
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
            LevelStartPos = DetermineLevelStartPos();
            LevelEndPos = DetermineLevelEndPos(LevelStartPos);

            Debug.Log($"LevelStartPos: {LevelStartPos}");
            Debug.Log($"LevelEndPos: {LevelEndPos}");

            InitializeGrids();

            InitializeLevelEditorCam();

            InitializeInputManager();

            InitializeLevelLayers();

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
        private void InitializeLevelLayers()
        {
            ULevelLayer[] levelLayers = FindObjectsOfType<ULevelLayer>();
            foreach (var levelLayer in levelLayers)
            {
                levelLayer.InitializeLevelLayer(this);
            }
        }
        private void InitializeTools()
        {
            _tools = GameObject.FindObjectsOfType<ULevelEditorTool>();
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
        private Vector3Int DetermineLevelStartPos()
        {
            Vector3Int levelStartPos = new Vector3Int();

            bool isXEven = LevelSize.x % 2 == 0;
            bool isYEven = LevelSize.y % 2 == 0;

            levelStartPos.x = isXEven ? -(LevelSize.x / 2) : -((LevelSize.x + 1) / 2);
            levelStartPos.y = isYEven ? -(LevelSize.y / 2) : -((LevelSize.y + 1) / 2);

            return levelStartPos;
        }
        private Vector3Int DetermineLevelEndPos(Vector3Int levelStartPos)
        {
            return new Vector3Int(levelStartPos.x + LevelSize.x, levelStartPos.y + LevelSize.y);
        }
        #endregion
        #region Update
        private void UpdateTools()
        {
            ToolPersistenceEvent?.Invoke();
            _tools.ForEach(t => t.HandleInput());
        }
        #endregion
        #region Events
        public void ToolSelected(ULevelEditorToolTypes toolType)
        {
            _tools.ForEach(t => t.Select(toolType));
        }
        public void ToolUnSelected(ULevelEditorToolTypes toolType)
        {
            _tools.ForEach(t => t.UnSelect(toolType));
        }
        #endregion
    }
}
