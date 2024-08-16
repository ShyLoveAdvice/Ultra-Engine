using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Ultra.LevelEditor
{
    public enum LevelEditorMouseInputStates
    {
        None,
        OverUI,
        MouseLeftButtonDown,
        MouseLeftButton,
        MouseLeftButtonUp,
        MouseMiddleButtonDown,
        MouseMiddleButton,
        MouseMiddelButtonUp
    }
    public enum LevelEditorInputs
    {
        None,
        CtrlC,
        CtrlV,
        CtrlD,
        Delete
    }
    public class ULevelEditorInputManager : MMSingleton<ULevelEditorInputManager>
    {
        public Vector3Int CurrentMouseCellPos { get => _currentCellPos; }
        public Vector3 CurrentMouseCenteredCellPos { get => _currentCenteredCellPos; }
        public Vector3 CurrentMouseWorldPos { get => _mouseWorldPos; }
        public Vector2 CurrentMouseScrollData { get => _mouseScrollDelta; }
        public bool IsMouseOverUI { get => _isMosueOverUI; }
        public LevelEditorMouseInputStates CurrentMouseInputState { get => _currentMouseInputState; }
        public LevelEditorInputs CurrentInput { get => _currentInput; }

        protected Vector3Int _currentCellPos;
        private Vector3 _currentCenteredCellPos;
        protected Vector2 _mouseScrollDelta;
        protected Vector3 _mouseWorldPos;
        protected bool _isMosueOverUI;
        protected LevelEditorMouseInputStates _currentMouseInputState;
        protected LevelEditorInputs _currentInput;

        protected ULevelEditor LevelEditor { get; set; }

        public void InitializeInputManager(ULevelEditor levelEditor)
        {
            LevelEditor = levelEditor;
        }
        public void ReadInput()
        {
            _currentCellPos = MouseCellPos();
            _currentCenteredCellPos = new Vector3(_currentCellPos.x + 0.5f, _currentCellPos.y + 0.5f);
            _mouseScrollDelta = Input.mouseScrollDelta;

            _currentMouseInputState = LevelEditorMouseInputStates.None;
            _currentInput = LevelEditorInputs.None;

            if (Input.GetMouseButton(2))
            {
                _currentMouseInputState = LevelEditorMouseInputStates.MouseMiddleButton;
            }
            if (Input.GetMouseButtonDown(2))
            {
                if (ULevelEditorUtilities.IsPointerOverUIObject())
                {
                    _isMosueOverUI = true;
                }
                _currentMouseInputState = LevelEditorMouseInputStates.MouseMiddleButtonDown;
            }
            if (Input.GetMouseButtonUp(2))
            {
                _isMosueOverUI = false;
                _currentMouseInputState = LevelEditorMouseInputStates.MouseMiddelButtonUp;
            }

            if (Input.GetMouseButton(0))
            {
                if (!LevelEditor.GUIManager.IsMouseOverUI)
                {
                    _isMosueOverUI = false;
                }
                _currentMouseInputState = LevelEditorMouseInputStates.MouseLeftButton;
            }
            if (Input.GetMouseButtonDown(0))
            {
                if (ULevelEditorUtilities.IsPointerOverUIObject())
                {
                    _isMosueOverUI = true;
                }
                _currentMouseInputState = LevelEditorMouseInputStates.MouseLeftButtonDown;
            }
            if (Input.GetMouseButtonUp(0))
            {
                _isMosueOverUI = false;
                _currentMouseInputState = LevelEditorMouseInputStates.MouseLeftButtonUp;
            }

            if (Input.GetKey(KeyCode.LeftControl))
            {
                if (Input.GetKeyDown(KeyCode.C))
                {
                    _currentInput = LevelEditorInputs.CtrlC;
                }
                else if (Input.GetKeyDown(KeyCode.V))
                {
                    _currentInput = LevelEditorInputs.CtrlV;
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    _currentInput = LevelEditorInputs.CtrlD;
                }
            }

            if(Input.GetKeyDown (KeyCode.Delete))
            {
                _currentInput = LevelEditorInputs.Delete;
            }

            if (_isMosueOverUI)
            {
                _currentMouseInputState = LevelEditorMouseInputStates.OverUI;
            }
        }
            private Vector3Int MouseCellPos()
            {
                _mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                Vector3 screenMin = Camera.main.ScreenToWorldPoint(Vector2.zero);
                Vector3 screenMax = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));

                _mouseWorldPos.x = Mathf.Clamp(_mouseWorldPos.x, screenMin.x, screenMax.x);
                _mouseWorldPos.y = Mathf.Clamp(_mouseWorldPos.y, screenMin.y, screenMax.y);
                _mouseWorldPos.z = 0f;

                return new Vector3Int(Mathf.FloorToInt(_mouseWorldPos.x), Mathf.FloorToInt(_mouseWorldPos.y));
            }
        }
    }
