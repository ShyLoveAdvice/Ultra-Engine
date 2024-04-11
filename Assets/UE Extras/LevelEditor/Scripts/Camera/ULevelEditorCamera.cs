using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Ultra.LevelEditor
{
    [System.Serializable]
    public class ULevelEditorCameraData
    {
        public float ZoomSpeed = 50f;
        public float MinZoomSize = 3f;
        public float MaxZoomSize = 15f;
    }
    public class ULevelEditorCamera
    {
        protected ULevelEditorCameraData Data;

        protected ULevelEditor LevelEditor { get; private set; }
        protected ULevelEditorInputManager InputManager { get => LevelEditor.InputManager; }
        protected Vector3 _mouseWorldPos;
        protected Vector3 _dragOrigin;
        protected Vector3 _newPostition;

        protected Camera _currentCamera => Camera.main;
        public virtual void CameraUpdate()
        {
                _mouseWorldPos = _currentCamera.ScreenToWorldPoint(Input.mousePosition);
                _mouseWorldPos.z = 0f;

                _newPostition = _currentCamera.transform.position;

                PanCamera();
                ZoomCamera();

                _currentCamera.transform.position = ClampCamera(_newPostition);
        }
        public ULevelEditorCamera(ULevelEditor levelEditor)
        {
            LevelEditor = levelEditor;
            Data = LevelEditor.CameraData;
        }
        protected virtual void PanCamera()
        {
            {
                if (InputManager.CurrentMouseInputState == LevelEditorMouseInputStates.MouseMiddleButtonDown)
                {
                    _dragOrigin = _mouseWorldPos;
                }

                if (InputManager.CurrentMouseInputState == LevelEditorMouseInputStates.MouseMiddleButton)
                {
                    Vector3 difference = _dragOrigin - _mouseWorldPos;
                    _newPostition += difference;
                }
            }
        }
        protected virtual void ZoomCamera()
        {
            if(!ULevelEditorGUIManager.Instance.IsMouseOverUI)
            {
                if (InputManager.CurrentMouseScrollData.y != 0)
                {
                    float zoomValue = (-Input.mouseScrollDelta.y) * Data.ZoomSpeed * Time.deltaTime;
                    _currentCamera.orthographicSize = Mathf.Clamp(_currentCamera.orthographicSize + zoomValue, Data.MinZoomSize, Data.MaxZoomSize);
                }
            }
        }
        protected virtual Vector3 ClampCamera(Vector3 targetPosition)
        {
            float camHeight = _currentCamera.orthographicSize;
            float camWidth = _currentCamera.orthographicSize * _currentCamera.aspect;

            float levelViewMinX = LevelEditor.LevelViewBound.min.x;
            float levelViewMinY = LevelEditor.LevelViewBound.min.y;
            float levelViewMaxX = LevelEditor.LevelViewBound.max.x;
            float levelViewMaxY = LevelEditor.LevelViewBound.max.y;

            float minX = levelViewMinX + camWidth;
            float minY = levelViewMinY + camHeight;
            float maxX = levelViewMaxX - camWidth;
            float maxY = levelViewMaxY - camHeight;

            float newX = Mathf.Clamp(targetPosition.x, minX, maxX);
            float newY = Mathf.Clamp(targetPosition.y, minY, maxY);

            return new Vector3(newX, newY, targetPosition.z);

        }
    }
}
