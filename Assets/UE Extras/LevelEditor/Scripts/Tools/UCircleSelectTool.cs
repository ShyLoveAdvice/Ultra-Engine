using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ultra.LevelEditor
{
    public class UCircleSelectTool : ULevelEditorTool
    {
        public UCircleSelectTool(ULevelEditor levelEditor, ULevelEditorToolTypes toolType) : base(levelEditor, toolType)
        {
        }
        public SelectStates CircleSelectState { get; private set; }

        public GameObject _activeBox;
        public GameObject _selectedBox;
        protected Vector3Int _startCellPos;
        protected Vector3Int _lastCellPos;
        protected override void BeforeMouseEvents()
        {
            //if (Input.GetKey(KeyCode.LeftShift))
            //{
            //    CircleSelectState = SelectStates.Additive;
            //}
            //else
            //{
            //    CircleSelectState = SelectStates.New;
            //}
        }
        protected override void OnMouseLeftButtonDown()
        {
            if (CircleSelectState == SelectStates.New)
            {
                //Selection.ClearDrawnSelected();
            }
            else
            {

            }
            _startCellPos = CurrentMouseCellPos;
        }
        protected override void OnMouseLeftButton()
        {
            if (CurrentMouseCellPos != LastCellPos)
            {
                //Selection.BuildBoxSelectionActive(new Vector2Int(_startCellPos.x, _startCellPos.y), new Vector2Int(CurrentCellPos.x, CurrentCellPos.y));
                //Selection.DrawActive(_activeBox);
            }
        }
        protected override void OnMouseLeftButtonUp()
        {
            if (CircleSelectState == SelectStates.New)
            {
                //Selection.BuildBoxSelection(new Vector2Int(_startCellPos.x, _startCellPos.y), new Vector2Int(CurrentCellPos.x, CurrentCellPos.y));
                //Selection.ClearDrawnActive();
                //Selection.DrawSelected(_selectedBox);
            }
            else
            {
                //Selection.BuildBoxSelectionAdditive(new Vector2Int(_startCellPos.x, _startCellPos.y), new Vector2Int(CurrentCellPos.x, CurrentCellPos.y));
                //Selection.ClearDrawnActive();
                //Selection.DrawSelected(_selectedBox);
            }
        }
    }
}
