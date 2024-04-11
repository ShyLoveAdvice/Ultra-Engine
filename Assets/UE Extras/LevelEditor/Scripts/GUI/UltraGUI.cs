using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Ultra.LevelEditor
{
    public class UltraGUI : MonoBehaviour
    {
        protected ULevelEditorGUIManager GUIManager { get; private set; }
        public void InitializeGUI(ULevelEditorGUIManager guiManager)
        {
            GUIManager = guiManager;
            Initialize();
        }
        protected virtual void Initialize()
        {

        }
        public virtual void UpdateGUI()
        {

        }
    }
}
