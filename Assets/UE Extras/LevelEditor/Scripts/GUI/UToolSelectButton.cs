using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;

namespace Ultra.LevelEditor
{
    public class UToolSelectButton : USelectButton
    {
        [Header("ULevelEditorButton")]
        public ULevelEditorToolTypes ButtonToolType;
        public override void Select()
        {
            base.Select();

            GUIManager.UpdateCurrentSelectedTool(ButtonToolType);
        }
    }
}
