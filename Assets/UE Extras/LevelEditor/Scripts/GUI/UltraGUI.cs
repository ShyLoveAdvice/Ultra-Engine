using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Ultra.LevelEditor
{
    public class UltraGUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public bool BlockMouseEvent = true;
        protected ULevelEditorGUIManager GUIManager { get; private set; }
        public void InitializeGUI(ULevelEditorGUIManager guiManager)
        {
            GUIManager = guiManager;
            Initialize();
        }
        protected virtual void Initialize()
        {

        }
        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if(BlockMouseEvent)
            {
                GUIManager.UpdateMouseOverUI(true);
            }
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (BlockMouseEvent)
            {
                GUIManager.UpdateMouseOverUI(false);
            }
        }
    }
}
