using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Ultra.LevelEditor
{
    public enum SelectStates
    {
        None,
        Move,
        New,
        Additive
    }
    public class UBoxSelectTool : ULevelEditorTool
    {
        public SelectStates BoxSelectState { get; private set; }

        protected Vector3Int _startCellPos;
        protected Vector3Int _lastCellPos;

        private bool CanMove
        {
            get
            {
                if (Selection.Contains(CurrentMouseCellPos))
                {
                    return Selection.Contains(CurrentMouseCellPos);
                }
                return false;
            }
        }
        private Vector3Int _movedDistance;
        private Vector3Int _lastMovedDistance;
        private Vector3Int[] _originalSelectedTilesPoses;
        private TileBase[] _originalTileBasesAtTilesPoses;
        private Vector3Int[] _movingTilePoses;
        public UBoxSelectTool(ULevelEditor levelEditor, ULevelEditorToolTypes toolType) : base(levelEditor, toolType)
        {
        }
        public override void InterruptTool()
        {
            base.InterruptTool();

            DeterminePreviewTiles(ref _originalSelectedTilesPoses, ref _originalTileBasesAtTilesPoses);
            if (_originalSelectedTilesPoses.Length == 0)
            {
                DetermineTiles(ref _originalSelectedTilesPoses, ref _originalTileBasesAtTilesPoses);
                LevelEditor.CurrentLayer.EraseTiles(_originalSelectedTilesPoses);
                LevelEditor.CurrentLayer.DrawTilesPreview(_originalSelectedTilesPoses, _originalTileBasesAtTilesPoses);
            }
            SaveAndDrawTiles(_movingTilePoses, _originalTileBasesAtTilesPoses);

        }
        protected override void UnSelected()
        {
            base.UnSelected();

            SaveAndDrawTiles(_movingTilePoses, _originalTileBasesAtTilesPoses);
        }
        protected override void BeforeMouseEvents()
        {
            if (InputManager.CurrentMouseInputState == LevelEditorMouseInputStates.None)
            {
                BoxSelectState = SelectStates.None;

                if (Input.GetKey(KeyCode.LeftShift))
                {
                    BoxSelectState = SelectStates.Additive;
                }
            }
        }
        protected override void OnMouseLeftButtonDown()
        {
            if (BoxSelectState == SelectStates.None)
            {
                if (Selection.NothingSelected || !Selection.Contains(CurrentMouseCellPos))
                {
                    BoxSelectState = SelectStates.New;
                }
                else if (CanMove)
                {
                    BoxSelectState = SelectStates.Move;
                    _movedDistance = Vector3Int.zero;
                    _lastMovedDistance = Vector3Int.zero;

                    DeterminePreviewTiles(ref _originalSelectedTilesPoses, ref _originalTileBasesAtTilesPoses);
                    if (_originalSelectedTilesPoses.Length == 0)
                    {
                        DetermineTiles(ref _originalSelectedTilesPoses, ref _originalTileBasesAtTilesPoses);
                        LevelEditor.CurrentLayer.EraseTiles(_originalSelectedTilesPoses);
                        LevelEditor.CurrentLayer.DrawTilesPreview(_originalSelectedTilesPoses, _originalTileBasesAtTilesPoses);
                    }

                    if (_originalSelectedTilesPoses.Length == 0)
                    {
                        LevelEditor.CurrentLayer.EraseTiles(_originalSelectedTilesPoses);
                    }

                    _movingTilePoses = new Vector3Int[_originalSelectedTilesPoses.Length];
                    for (int i = 0; i < _originalSelectedTilesPoses.Length; i++)
                    {
                        _movingTilePoses[i] = _originalSelectedTilesPoses[i];
                    }
                }
            }

            _startCellPos = CurrentMouseCellPos;
        }
        protected override void OnMouseLeftButton()
        {
            if (BoxSelectState == SelectStates.New || BoxSelectState == SelectStates.Additive)
            {
                if (CurrentMouseCellPos != LastCellPos || CurrentMouseCellPos == _startCellPos)
                {
                    Selection.BuildBoxSelectionActive(new Vector2Int(_startCellPos.x, _startCellPos.y), new Vector2Int(CurrentMouseCellPos.x, CurrentMouseCellPos.y));
                    Selection.DrawActive();
                }
            }
            if (BoxSelectState == SelectStates.Move)
            {
                _movedDistance = CurrentMouseCellPos - _startCellPos;
                if (_movedDistance != _lastMovedDistance)
                {
                    Selection.MoveSelectionPreview(_movedDistance);

                    LevelEditor.CurrentLayer.ClearAllPreviewTiles();
                    LevelEditor.CurrentLayer.DrawTilesInDict(_movingTilePoses);

                    for (int i = 0; i < _originalSelectedTilesPoses.Length; i++)
                    {
                        _movingTilePoses[i].x = _originalSelectedTilesPoses[i].x + _movedDistance.x;
                        _movingTilePoses[i].y = _originalSelectedTilesPoses[i].y + _movedDistance.y;
                    }

                    LevelEditor.CurrentLayer.DrawTilesPreview(_movingTilePoses, _originalTileBasesAtTilesPoses);

                    _lastMovedDistance = _movedDistance;
                }
            }

        }
        protected override void OnMouseLeftButtonUp()
        {
            if (BoxSelectState == SelectStates.New)
            {
                Selection.ClearDrawnSelected();

                if (CurrentMouseCellPos != _startCellPos)
                {
                    Selection.BuildBoxSelection(new Vector2Int(_startCellPos.x, _startCellPos.y), new Vector2Int(CurrentMouseCellPos.x, CurrentMouseCellPos.y));
                    Selection.ClearDrawnActive();
                    Selection.DrawSelected();
                }
                else
                {
                    Selection.ClearDrawnActive();
                    Selection.ClearSelectionData();
                }

                SaveAndDrawTiles(_movingTilePoses, _originalTileBasesAtTilesPoses);
            }
            if (BoxSelectState == SelectStates.Additive)
            {
                Selection.BuildBoxSelectionAdditive(new Vector2Int(_startCellPos.x, _startCellPos.y), new Vector2Int(CurrentMouseCellPos.x, CurrentMouseCellPos.y));
                Selection.ClearDrawnActive();
                Selection.DrawSelected();
            }
            if (BoxSelectState == SelectStates.Move)
            {
                Selection.BuildMovedSelectionData(CurrentMouseCellPos - _startCellPos);
                Selection.DrawSelected();
            }
        }
        protected override void PersistUpdate()
        {
            if (BoxSelectState == SelectStates.None)
            {
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    if (Input.GetKeyDown(KeyCode.D))
                    {
                        Selection.ClearSelectionData();
                        Selection.ClearDrawnSelected();
                    }
                }
            }
        }
        public void SelectTiles()
        {

        }

        public void DeterminePreviewTiles(ref Vector3Int[] tilePoses, ref TileBase[] tileBases)
        {
            tilePoses = Selection.GetSelectedPreviewTiles();
            tileBases = LevelEditor.CurrentLayer.GetPreviewTileBases(tilePoses);
        }
        public void DetermineTiles(ref Vector3Int[] tilePoses, ref TileBase[] tileBases)
        {
            tilePoses = Selection.GetSelectedTiles();
            tileBases = LevelEditor.CurrentLayer.GetTileBases(tilePoses);
        }

        public void SaveAndDrawTiles(Vector3Int[] tilePoses, TileBase[] tileBases)
        {
            if (_originalSelectedTilesPoses != null && _movingTilePoses != null)
            {
                LevelEditor.CurrentLayer.ClearAllPreviewTiles();
                LevelEditor.CurrentLayer.DrawTiles(tilePoses, tileBases);
            }
        }
    }
}

