using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine.UIElements;
using UnityEditor.Toolbars;

namespace Ultra.UntitledNewGame
{
    //[Overlay(typeof(SceneView), id: ID_OVERLAY, displayName: "World Manager")]
    //[Icon(Path + "Icon_WorldManager.png")]
    public class UWorldManagerOverlay : ToolbarOverlay
    {
        //    protected bool VoidBrushToggled = false;

        //    private const string Path = "Assets/UE Extras/Untitled Platformer Puzzle Game/Textures/Icons/";
        //    private const string ID_OVERLAY = "World Manager Tools";
        //    public UWorldManagerOverlay() : base(AddWorldPieceBrush.ID, AddVoidPieceBrush.ID)
        //    {

        //    }
        //    public override VisualElement CreatePanelContent()
        //    {
        //        var root = new VisualElement();

        //        var titleLabel = new Label(text: "World Manager Tools");

        //        return root;
        //    }
        //    [EditorToolbarElement(ID, typeof(SceneView))]
        //    class AddWorldPieceBrush: EditorToolbarToggle
        //    {
        //        private UWorldManagerBrushType _brushType = UWorldManagerBrushType.WorldPiece;
        //        public const string ID = "WorldManagerTools/Piece Brush";
        //        public AddWorldPieceBrush()
        //        {
        //            this.text = "Piece Brush";
        //            this.icon = AssetDatabase.LoadAssetAtPath<Texture2D>(Path + "Icon_PieceBrush.png");
        //            this.tooltip = "Use this to paint world pieces into the scene";
        //            this.RegisterToggle();
        //            this.RegisterValueChangedCallback(OnValueChanged);
        //        }
        //        private void OnValueChanged(ChangeEvent<bool> changedEvent)
        //        {
        //            if(changedEvent.newValue == true)
        //            {
        //                UWorldManagerEditor.SetBrush(_brushType);
        //                ToggleManager.SelectToggle(this);
        //            }
        //            else
        //            {
        //                if(UWorldManagerEditor.BrushType == _brushType)
        //                {
        //                    UWorldManagerEditor.SetBrush(UWorldManagerBrushType.None);
        //                }
        //            }
        //        }
        //    }
        //    [EditorToolbarElement(ID, typeof(SceneView))]
        //    class AddVoidPieceBrush : EditorToolbarToggle
        //    {
        //        private UWorldManagerBrushType _brushType = UWorldManagerBrushType.VoidPiece;
        //        public const string ID = "WorldManagerTools/Void Brush";
        //        public AddVoidPieceBrush()
        //        {
        //            this.text = "Void Brush";
        //            this.icon = AssetDatabase.LoadAssetAtPath<Texture2D>(Path + "Icon_VoidBrush.png");
        //            this.tooltip = "Use this to paint void pieces into the scene";
        //            this.RegisterToggle();
        //            this.RegisterValueChangedCallback(OnValueChanged);
        //        }
        //        private void OnValueChanged(ChangeEvent<bool> changedEvent)
        //        {
        //            if (changedEvent.newValue == true)
        //            {
        //                UWorldManagerEditor.SetBrush(_brushType);
        //                ToggleManager.SelectToggle(this);
        //            }
        //            else
        //            {
        //                if (UWorldManagerEditor.BrushType == _brushType)
        //                {
        //                    UWorldManagerEditor.SetBrush(UWorldManagerBrushType.None);
        //                }
        //            }
        //        }
        //    }

        //}
        //public static class ToggleManager
        //{
        //    public static List<Toggle> AllToggle = new List<Toggle>();
        //    private static Toggle _lastSelectedToggle;
        //    public static void RegisterToggle(this Toggle toggle)
        //    {
        //        AllToggle.Add(toggle);
        //    }
        //    public static void SelectToggle(Toggle toggle)
        //    {
        //        AllToggle.ForEach(t =>
        //        {
        //            if(t != toggle)
        //            {
        //                t.value = false;
        //            }
        //        });
        //    }
        //}
    }
}
