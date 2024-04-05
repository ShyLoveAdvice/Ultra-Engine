using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ultra.LevelEditor
{
    public class UEraserTool : ULevelEditorTool
    {
        private List<Vector3Int> _toBeErasedTiles = new List<Vector3Int>();
        public UEraserTool(ULevelEditor levelEditor, ULevelEditorToolTypes toolType) : base(levelEditor, toolType)
        {
        }
        protected override void OnMouseLeftButton()
        {
            if (MoveMoreThanOneCellAtFrame)
            {
                Vector3Int[] lineCellPoses = UShapeGetter.GetLine(LastCellPos, CurrentMouseCellPos);
                for (int i = 0; i < lineCellPoses.Length; i++)
                {
                    EraseTile(lineCellPoses[i]);
                }
            }
            else
            {
                EraseTile(CurrentMouseCellPos);
            }
        }
        protected override void OnMouseLeftButtonUp()
        {
            LevelEditor.CurrentLayer.EraseTiles(_toBeErasedTiles.ToArray());
            _toBeErasedTiles.Clear();
        }
        protected void EraseTile(Vector3Int tilePos)
        {
            if (!_toBeErasedTiles.Contains(tilePos))
            {
                if (Selection.NothingSelected || Selection.Contains(tilePos))
                {
                    _toBeErasedTiles.Add(tilePos);
                    LevelEditor.CurrentLayer.EraseTilePreview(tilePos);
                }
            }
        }
    }
}
