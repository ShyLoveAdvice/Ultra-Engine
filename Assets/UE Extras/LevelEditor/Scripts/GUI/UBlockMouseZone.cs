using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Ultra.LevelEditor
{
    public class UBlockMouseZone : UltraGUI, IPointerEnterHandler, IPointerExitHandler
    {
        public bool BlockMouseEvent = true;
        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (BlockMouseEvent)
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
