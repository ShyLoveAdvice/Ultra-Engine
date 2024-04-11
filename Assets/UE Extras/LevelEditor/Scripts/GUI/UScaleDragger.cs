using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Ultra.LevelEditor
{

    public class UScaleDragger : UltraGUI, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        protected USelection Selection { get => ULevelEditor.Instance.Selection; }
        private Vector3 _draggerWorldPos;
        private Vector3 _currentMouseCenteredCellPos;
        private Vector3Int _positionShift;
        private RectTransform _rectTransform;
        protected override void Initialize()
        {
            base.Initialize();

            _rectTransform = GetComponent<RectTransform>();
        }
        public override void UpdateGUI()
        {
            if(gameObject.activeSelf)
            {
                Vector2 uiPos = (MMECoordinateSystemConverter.WorldPointToUILocalPoint(_draggerWorldPos + _positionShift, GUIManager.UICamera, Selection.DraggersParent));
                _rectTransform.anchoredPosition = uiPos;
            }
        }
        public void TurnOnScaleDragger()
        {
            gameObject.SetActive(true);
            _positionShift = Vector3Int.zero;
        }
        public void TurnOffScaleDragger()
        {
            gameObject.SetActive(false);
            _positionShift = Vector3Int.zero;
        }
        public void SetWorldPos(Vector3 worldPos)
        {
            _draggerWorldPos = worldPos;
        }
        public void OnBeginDrag(PointerEventData eventData)
        {
            Selection.BeginDrag(this);
        }
        public void OnDrag(PointerEventData eventData)
        {
            UpdateUIDimentionPosition();
            Selection.Drag(_positionShift);
        }
        public void OnEndDrag(PointerEventData eventData)
        {
            Debug.Log("EndDrag");
        }
        private void UpdateUIDimentionPosition()
        {
            if (!GUIManager.IsMouseOverUI)
            {
                _currentMouseCenteredCellPos = ULevelEditorInputManager.Instance.CurrentMouseCenteredCellPos;

                Vector3 vectorToMouse = _currentMouseCenteredCellPos - _draggerWorldPos;
                int positionShiftX = 0; int positionShiftY = 0;

                if (vectorToMouse.x > 0)
                {
                    positionShiftX = Mathf.FloorToInt(vectorToMouse.x);
                }
                else
                {
                    positionShiftX = Mathf.CeilToInt(vectorToMouse.x);
                }

                if (vectorToMouse.y > 0)
                {
                    positionShiftY = Mathf.FloorToInt(vectorToMouse.y);
                }
                else
                {
                    positionShiftY = Mathf.CeilToInt(vectorToMouse.y);
                }

                _positionShift.x = positionShiftX;
                _positionShift.y = positionShiftY;
            }
        }
    }
}
