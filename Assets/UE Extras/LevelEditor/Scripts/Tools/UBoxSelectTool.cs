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
        public UBoxSelectTool(ULevelEditor levelEditor, ULevelEditorToolTypes toolType) : base(levelEditor, toolType)
        {
        }
        public override void InterruptTool()
        {
            base.InterruptTool();
        }
        protected override void OnSelected()
        {
            base.OnSelected();

            if(Selection != null)
            {
                Selection.RebuildSelected();
            }
        }
        protected override void UnSelected()
        {
            base.UnSelected();

            if(Selection != null)
            {
                Selection.ClearAndSetSelectedTiles();
            }
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
                    Debug.Log("NothingSelected: " + Selection.NothingSelected);
                }
                else if (CanMove)
                {
                    BoxSelectState = SelectStates.Move;
                    _movedDistance = Vector3Int.zero;
                    _lastMovedDistance = Vector3Int.zero;
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
                    Selection.BuildActive(new Vector3Int(_startCellPos.x, _startCellPos.y), new Vector3Int(CurrentMouseCellPos.x, CurrentMouseCellPos.y));
                }
            }
            if (BoxSelectState == SelectStates.Move)
            {
                _movedDistance = CurrentMouseCellPos - _startCellPos;
                if (_movedDistance != _lastMovedDistance)
                {
                    Selection.MoveSelected(_movedDistance);
                    _lastMovedDistance = _movedDistance;
                }
            }
        }
        protected override void OnMouseLeftButtonUp()
        {
            if (BoxSelectState == SelectStates.New)
            {
                Selection.ClearSelected();
                Selection.ClearActive();

                if (CurrentMouseCellPos != _startCellPos)
                {
                    Selection.BuildSelected(new Vector3Int(_startCellPos.x, _startCellPos.y), new Vector3Int(CurrentMouseCellPos.x, CurrentMouseCellPos.y));
                }
            }
            if (BoxSelectState == SelectStates.Additive)
            {
                Selection.ClearActive();
                Selection.BuildSelectedAdditive(new Vector3Int(_startCellPos.x, _startCellPos.y), new Vector3Int(CurrentMouseCellPos.x, CurrentMouseCellPos.y));
            }
            if (BoxSelectState == SelectStates.Move)
            {
                Selection.MoveSelected(CurrentMouseCellPos - _startCellPos);
                Selection.MoveSelectedMouseUp(CurrentMouseCellPos - _startCellPos);
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
                        Selection.ClearSelected();
                    }
                }
            }
        }
    }
}

