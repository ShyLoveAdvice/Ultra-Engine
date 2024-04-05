using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Ultra.LevelEditor
{
    public class UMoveTool : ULevelEditorTool
    {
        private bool CanMove
        {
            get
            {
                if(Selection.SomethingSelected)
                {
                    return Selection.Contains(CurrentMouseCellPos);
                }
                return false;
            }
        }
        private bool _moveStarted = false;
        private Vector3Int _startMovingCellPos;
        private Vector3Int _movedDistance;
        private Vector3Int _lastMovedDistance;
        private Vector3Int[] _originalSelectedTilesPoses;
        private Vector3Int[] _movingTilePoses;
        private Vector3Int[] _lastMovedTilePoses;

        public UMoveTool(ULevelEditor levelEditor, ULevelEditorToolTypes toolType) : base(levelEditor, toolType)
        {
            
        }
        protected override void OnMouseLeftButtonDown()
        {
            if(CanMove)
            {
                _startMovingCellPos = CurrentMouseCellPos;
                _lastMovedDistance = Vector3Int.zero;
                _originalSelectedTilesPoses = Selection.GetSelectedTiles();
                _lastMovedTilePoses = _originalSelectedTilesPoses;
                _moveStarted = true;
            }
        }
        protected override void OnMouseLeftButton()
        {
            if (_moveStarted)
            {
                _movedDistance = CurrentMouseCellPos - _startMovingCellPos;
                if (_movedDistance != _lastMovedDistance)
                {
                    Selection.MoveSelectionPreview(_movedDistance);

                    _movingTilePoses = new Vector3Int[_originalSelectedTilesPoses.Length];
                    for (int i = 0; i < _originalSelectedTilesPoses.Length; i++)
                    {
                        _movingTilePoses[i].x = _originalSelectedTilesPoses[i].x + _movedDistance.x;
                        _movingTilePoses[i].y = _originalSelectedTilesPoses[i].y + _movedDistance.y;
                    }

                    LevelEditor.CurrentLayer.ErasePreviewTilesNotInTileDataDict(_lastMovedTilePoses);
                    LevelEditor.CurrentLayer.DrawTilesPreview(_movingTilePoses, LevelEditor.CurrentTileBase);

                    _lastMovedDistance = _movedDistance;
                    _lastMovedTilePoses = _movingTilePoses;
                }
            }
        }
        protected override void OnMouseLeftButtonUp()
        {
            if(_moveStarted)
            {
                Selection.BuildMovedSelectionData(_movedDistance);
                Selection.DrawSelected();

                TileBase[] tileBasesInOriginalTilesPoses = LevelEditor.CurrentLayer.GetTileBases(_originalSelectedTilesPoses);
                LevelEditor.CurrentLayer.EraseTiles(_originalSelectedTilesPoses);
                LevelEditor.CurrentLayer.DrawTiles(_movingTilePoses, tileBasesInOriginalTilesPoses);

                _moveStarted = false;
            }
        }

    }
}
