using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ultra.LevelEditor
{
    public class UCircleTool : ULevelEditorTool
    {
        protected Vector3Int _startCellPos;
        protected Vector3Int _ellipseCenter;
        protected Vector3Int[] _circleCellPoses = new Vector3Int[0];
        protected List<Vector3Int> _toBeDrawnCellPoses = new List<Vector3Int>();
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
                _circleCellPoses = UShapeGetter.GetEllipseByDiagonalLine(_startCellPos, CurrentMouseCellPos);
                Debug.Log(_circleCellPoses.Length);
                for (int i = 0; i < _circleCellPoses.Length; i++)
                {
                    if (CanDrawTile(_circleCellPoses[i]))
                    {
                        LevelEditor.PreviewLayer.DrawPreviewTile(_circleCellPoses[i], LevelEditor.CurrentTile);
                    }
                }
            }
        }
        protected override void OnMouseLeftButtonUp()
        {
            DetermineFinalToBeDrawnTiles();
            LevelEditor.PreviewLayer.ClearPreviewTiles();
            LevelEditor.CurrentLayer.DrawTiles(_toBeDrawnCellPoses.ToArray(), LevelEditor.CurrentTile);
        }
        protected void DetermineFinalToBeDrawnTiles()
        {
            for (int i = 0; i < _circleCellPoses.Length; i++)
            {
                if (CanDrawTile(_circleCellPoses[i]))
                {
                    _toBeDrawnCellPoses.Add(_circleCellPoses[i]);
                }
            }
        }
        protected bool CanDrawTile(Vector3Int tilePos)
        {
            return Selection.NothingSelected || Selection.Contains(tilePos);
        }
    }
}
