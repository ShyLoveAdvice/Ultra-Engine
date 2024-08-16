using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ultra.LevelEditor
{
    public class UMagicWandTool: ULevelEditorTool
    {
        public UMagicWandTool(ULevelEditor levelEditor, ULevelEditorToolTypes toolType) : base(levelEditor, toolType)
        {
        }

        protected override void OnMouseLeftButtonDown()
        {
            if(!Selection.Contains(CurrentMouseCellPos))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    Selection.BuildSelected(LevelEditor.CurrentLayer.GetFloodFillPoses(CurrentMouseCellPos));
                }
                else
                {
                    Selection.PutDownSelected();
                    Selection.BuildSelectedClear(LevelEditor.CurrentLayer.GetFloodFillPoses(CurrentMouseCellPos));
                }
            }
            
        }
    }
}
