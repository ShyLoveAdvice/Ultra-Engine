using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using UnityEngine;

namespace Ultra.LevelEditor
{
    public class PaintCommand : ICommand
    {
        private UBrushTool _brushTool;
        private Vector3Int[] _previousDrawnTiles;
        private Vector3Int[] _toBeDrawTiles;
        public PaintCommand(UBrushTool brushTool, Vector3Int[] drawnTiles, Vector3Int[] toBeDrawTiles)
        {
            _brushTool = brushTool;
            _previousDrawnTiles = drawnTiles;
            _toBeDrawTiles = drawnTiles.Concat(toBeDrawTiles).ToArray();
        }
        public void Execute()
        {
            //_brushTool.DrawTiles(_toBeDrawTiles);
        }

        public void Undo()
        {
            //_brushTool.DrawTiles(_previousDrawnTiles);
        }
    }
}
