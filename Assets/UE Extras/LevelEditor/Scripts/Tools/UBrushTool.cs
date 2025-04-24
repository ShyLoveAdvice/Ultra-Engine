using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Ultra.LevelEditor
{
    public class UBrushTool : ULevelEditorTool
    {
        private List<Vector3Int> _toBePaintedTiles = new List<Vector3Int>();
        public Vector3Int[] _paintedTiles = new Vector3Int[0];
        public Vector3Int[] _drawnTiles;
        protected Vector3 _mouseWorldPos;
        protected override void PersistUpdate()
        {

        }
        protected override void OnMouseLeftButtonDown()
        {
        }
        protected override void OnMouseLeftButton()
        {
            if(MoveMoreThanOneCellAtFrame)
            {
                Vector3Int[] lineCellPoses = UShapeGetter.GetLine(LastCellPos, CurrentMouseCellPos);
                for(int i = 0; i < lineCellPoses.Length; i++)
                {
                    DrawTile(lineCellPoses[i]);
                }
            }
            else
            {
                DrawTile(CurrentMouseCellPos);
            }
            
        }
        protected override void OnMouseLeftButtonUp()
        {
            if(_toBePaintedTiles.Count > 0)
            {
                LevelEditor.PreviewLayer.ClearPreviewTiles();
                LevelEditor.CurrentLayer.DrawTiles(_toBePaintedTiles.ToArray(), LevelEditor.CurrentTile);
                _toBePaintedTiles.Clear();
            }
        }
        protected void DrawTile(Vector3Int tilePos)
        {
            if (!_toBePaintedTiles.Contains(tilePos))
            {
                if (Selection.NothingSelected || Selection.Contains(tilePos))
                {
                    _toBePaintedTiles.Add(tilePos);
                    LevelEditor.PreviewLayer.DrawPreviewTile(tilePos, LevelEditor.CurrentTile);
                }
            }
        }
    }
}
