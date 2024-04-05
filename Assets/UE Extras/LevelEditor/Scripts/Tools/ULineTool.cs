using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ultra.LevelEditor
{
    public class ULineTool : ULevelEditorTool
    {
        protected Vector3Int _startCellPos;
        protected Vector3Int[] _lineCellPoses = new Vector3Int[0];
        protected List<Vector3Int> _toBeDrawnCellPoses = new List<Vector3Int>();
        public ULineTool(ULevelEditor levelEditor, ULevelEditorToolTypes toolType) : base(levelEditor, toolType)
        {
        }
        protected override void OnMouseLeftButtonDown()
        {
            _toBeDrawnCellPoses.Clear();
            _startCellPos = CurrentMouseCellPos;
        }
        protected override void OnMouseLeftButton()
        {
            if (CurrentMouseCellPos != LastCellPos)
            {
                LevelEditor.PreviewLayer.ClearPreviewTiles();
                _lineCellPoses = UShapeGetter.GetLine(_startCellPos, CurrentMouseCellPos);
                for (int i = 0; i < _lineCellPoses.Length; i++)
                {
                    if(CanDrawTile(_lineCellPoses[i]))
                    {
                        LevelEditor.PreviewLayer.DrawPreviewTile(_lineCellPoses[i], LevelEditor.CurrentTileBase);
                    }
                }
            }
        }
        protected override void OnMouseLeftButtonUp()
        {
            DetermineFinalToBeDrawnTiles();
            LevelEditor.PreviewLayer.ClearPreviewTiles();
            LevelEditor.CurrentLayer.DrawTiles(_toBeDrawnCellPoses.ToArray(), LevelEditor.CurrentTileBase);
        }
        protected void DetermineFinalToBeDrawnTiles()
        {
            for (int i = 0; i < _lineCellPoses.Length; i++)
            {
                if (CanDrawTile(_lineCellPoses[i]))
                {
                    _toBeDrawnCellPoses.Add(_lineCellPoses[i]);
                }
            }
        }
        protected bool CanDrawTile(Vector3Int tilePos)
        {
            return Selection.NothingSelected || Selection.Contains(tilePos);
        }
    }
}
