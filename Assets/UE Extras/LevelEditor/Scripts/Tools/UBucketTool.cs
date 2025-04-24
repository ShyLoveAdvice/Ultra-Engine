using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ultra.LevelEditor
{
    public class UBucketTool : ULevelEditorTool
    {
        protected override void OnMouseLeftButtonDown()
        {
            if (Selection.SomethingSelected)
            {
                if (Selection.Contains(CurrentMouseCellPos))
                {
                    LevelEditor.CurrentLayer.FloodFill(CurrentMouseCellPos, new UTileData(LevelEditor.CurrentTile), Selection);
                }
            }
            else
            {
                LevelEditor.CurrentLayer.FloodFill(CurrentMouseCellPos, new UTileData(LevelEditor.CurrentTile));
            }
        }
    }
}
