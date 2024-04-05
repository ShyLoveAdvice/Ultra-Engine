using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Ultra.LevelEditor
{
    
    public class UScaleDragger : UltraGUI, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        private Vector3Int _draggerCellPos;
        private Vector3 _draggerWorldPos;
        private Vector3Int _positionShift;
        private UScaleDraggerManager _scaleDraggerManager;
        public RectTransform RectTransform { get => _rectTransform; }
        private RectTransform _rectTransform;
        public Camera UICamera;
        protected override void Initialize()
        {
            base.Initialize();

            BlockMouseEvent = false;
            _rectTransform = GetComponent<RectTransform>();
        }
        public void InitializeScaleDragger(UScaleDraggerManager scaleDraggerManager)
        {
            _scaleDraggerManager = scaleDraggerManager;
        }
        public void UpdateScaleDragger(USelection.ScaleDraggersData scaleDraggersData, UScaleDraggerManager.ScaleDraggerDirections dir)
        {
            Vector3 worldPos = Vector3.zero;
            switch(dir)
            {
                case UScaleDraggerManager.ScaleDraggerDirections.LeftBottom:
                    worldPos = scaleDraggersData.leftBottom; break;
                case UScaleDraggerManager.ScaleDraggerDirections.LeftTop:
                    worldPos = scaleDraggersData.leftTop; break;
                case UScaleDraggerManager.ScaleDraggerDirections.RightBottom:
                    worldPos = scaleDraggersData.rightBottom; break;
                case UScaleDraggerManager.ScaleDraggerDirections.RightTop:
                    worldPos = scaleDraggersData.rightTop; break;
            }

            _draggerWorldPos = worldPos;
            _draggerCellPos = worldPos.ToCellPos();

            Vector2 newPos = (MMECoordinateSystemConverter.WorldPointToUILocalPoint(worldPos + _positionShift, _scaleDraggerManager.GUIManager.UICamera, _scaleDraggerManager.RectTransform));
            _rectTransform.anchoredPosition = newPos;
        }
        public void OnBeginDrag(PointerEventData eventData)
        {
            Debug.Log("BeginDrag");
        }
        public void OnDrag(PointerEventData eventData)
        {
            Vector3 currentMouseCenteredCellPos = ULevelEditorInputManager.Instance.CurrentMouseCenteredCellPos;

            Vector3 vectorToMouse = currentMouseCenteredCellPos - _draggerWorldPos;

            Debug.Log(vectorToMouse);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Debug.Log("EndDrag");
        }
    }
}
