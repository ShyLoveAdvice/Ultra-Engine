using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ultra.LevelEditor
{
    public class ULevelEditorTool: MonoBehaviour
    {
        [SerializeField] private ULevelEditorToolTypes ToolType;
        protected ULevelEditor LevelEditor { get => ULevelEditor.Instance; }
        protected ULevelEditorInputManager InputManager { get => LevelEditor.InputManager; }
        protected Vector3Int CurrentMouseCellPos { get; private set; }
        protected Vector3Int LastCellPos { get; private set; }
        protected bool MoveMoreThanOneCellAtFrame {  get; private set; }    
        protected USelection Selection
        {
            get
            {
                if (LevelEditor != null)
                {
                    return LevelEditor.Selection;
                }
                return null;
            }
        }
        private bool ToolStarted;
        protected bool ToolSelected;
        public virtual void InterruptTool()
        {
            if (ToolStarted)
            {
                ToolStarted = false;
                OnMouseLeftButtonUp();
            }
        }
        public void Select(ULevelEditorToolTypes toolType)
        {
            if(toolType == ToolType)
            {
                OnSelected();
                ToolSelected = true;
            }
        }
        protected virtual void OnSelected() { }
        public void UnSelect(ULevelEditorToolTypes toolType)
        {
            if(toolType == ToolType)
            {
                UnSelected();
                ToolSelected = false;
            }
        }

        protected virtual void UnSelected() { }
        public void HandleInput()
        {
            CurrentMouseCellPos = InputManager.CurrentMouseCellPos;

            if(Mathf.Abs(CurrentMouseCellPos.x - LastCellPos.x) > 1 || Mathf.Abs(CurrentMouseCellPos.y - LastCellPos.y) > 1)
            {
                MoveMoreThanOneCellAtFrame = true;
            }
            else
            {
                MoveMoreThanOneCellAtFrame = false;
            }

            PersistUpdate();

            if(LevelEditor.CurrentTool != ToolType)
            {
                return;
            }

            if(InputManager.CurrentMouseInputState == LevelEditorMouseInputStates.OverUI)
            {
                return;
            }

            BeforeMouseEvents();

            switch (InputManager.CurrentMouseInputState)
            {
                case LevelEditorMouseInputStates.None:
                    break;
                case LevelEditorMouseInputStates.OverUI: 
                    break;
                case LevelEditorMouseInputStates.MouseLeftButtonDown:
                    OnMouseLeftButtonDown();
                    ToolStarted = true;
                    break;
                case LevelEditorMouseInputStates.MouseLeftButton:
                    if(ToolStarted)
                    {
                        OnMouseLeftButton();
                    }
                    break;
                case LevelEditorMouseInputStates.MouseLeftButtonUp:
                    if (ToolStarted)
                    {
                        OnMouseLeftButtonUp();
                        ToolStarted = false;
                    }
                    break;
            }

            AfterMouseEvents();

            LastCellPos = CurrentMouseCellPos;
        }
        /// <summary>
        /// PersistUpdate() happens every frame regardless of the tool currently selected
        /// </summary>
        protected virtual void PersistUpdate()
        {

        }
        /// <summary>
        /// Event happens every frame before performing mouse input events
        /// </summary>
        protected virtual void BeforeMouseEvents()
        {
        }
        /// <summary>
        /// Event happens every frame after performing mouse input events
        /// </summary>
        protected virtual void AfterMouseEvents()
        {

        }
        protected virtual void OnMouseLeftButtonDown()
        {

        }
        protected virtual void OnMouseLeftButton()
        {

        }
        protected virtual void OnMouseLeftButtonUp()
        {

        }
    }
}
