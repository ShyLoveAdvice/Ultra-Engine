using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Ultra.LevelEditor
{
    public class UScaleDraggerManager : MonoBehaviour
    {
        public Vector3Int CurrentCellPos;
        [HideInInspector] public ULevelEditorGUIManager GUIManager;
        [HideInInspector] public RectTransform RectTransform;
        [HideInInspector] public ULevelEditorInputManager InputManager;
        private USelection.ScaleDraggersData _currentScaleDraggerData;
        public enum ScaleDraggerDirections { LeftBottom, LeftTop, RightBottom, RightTop }
        private UScaleDragger[] _scaleDraggers;
        private bool _scaleDraggersTurnedOn;
        public void InitializeScaleDraggerManager(ULevelEditorGUIManager GUIManager)
        {
            this.GUIManager = GUIManager;
            RectTransform = GetComponent<RectTransform>();
            _scaleDraggers = GetComponentsInChildren<UScaleDragger>();

            for (int i = 0; i < _scaleDraggers.Length; i++)
            {
                _scaleDraggers[i].InitializeScaleDragger(this);
            }

            TurnOffScaleDraggers();
        }
        public void UpdateScaleDraggerManager()
        {
            if(_scaleDraggersTurnedOn)
            {
                for (int i = 0; i < _scaleDraggers.Length; i++)
                {
                    _scaleDraggers[i].UpdateScaleDragger(_currentScaleDraggerData, (ScaleDraggerDirections)i);
                }
            }
        }
        public void TurnOnScaleDraggers()
        {
            foreach (var scaleDragger in _scaleDraggers)
            {
                scaleDragger.gameObject.SetActive(true);
            }

            _scaleDraggersTurnedOn = true;
        }
        public void TurnOffScaleDraggers()
        {
            foreach (var scaleDragger in _scaleDraggers)
            {
                scaleDragger.gameObject.SetActive(false);
            }

            _scaleDraggersTurnedOn = false;
        }
        public void SetScaleDraggersWorldPositions(USelection.ScaleDraggersData scaleDraggersData)
        {
            _currentScaleDraggerData = scaleDraggersData;
            if (_scaleDraggersTurnedOn)
            {
                
            }
        }
    }
}
