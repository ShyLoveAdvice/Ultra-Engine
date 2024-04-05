using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ultra.UntitledNewGame
{
    public enum UWorldManagerBrushType
    {
        None,
        WorldPiece,
        VoidPiece
    }
    [CustomEditor(typeof(UWorldManager), true)]
    [InitializeOnLoad]
    public class UWorldManagerEditor: Editor
    {
   //     protected const string _texturePath = "Assets/UE Extras/Untitled Platformer Puzzle Game/Textures/Icons/";

   //     public static UWorldManagerBrushType BrushType;
   //     protected UWorldManagerBrushType _lastBrushType = UWorldManagerBrushType.None;

   //     protected UWorldManager WorldManagerTarget;
   //     protected SerializedProperty _worldPiece;
   //     protected SerializedProperty _voidPiece;

   //     protected static Texture2D _worldPieceMouseCursor;
   //     protected static Texture2D _voidPieceMouseCursor;
   //     protected static CursorMode _cursorMode = CursorMode.Auto;
   //     protected static Vector2 _hotSpot = new Vector2(50,50);

   //     protected bool _cursorIsOut;

   //     protected Event _currentEvent;

   //     protected const string _undoText = "Modified World Manager";
   //     protected virtual void OnEnable()
   //     {
			//Initialization();
   //     }
   //     protected virtual void Initialization()
   //     {
   //         WorldManagerTarget = target as UWorldManager;
   //         _worldPiece = serializedObject.FindProperty("WorldPiece");
   //         _voidPiece = serializedObject.FindProperty("VoidPiece");
   //         _worldPiece.objectReferenceValue = AssetDatabase.LoadAssetAtPath
   //             <GameObject>("Assets/UE Extras/Untitled Platformer Puzzle Game/Prefabs/World/WorldPiece.prefab");
   //         _voidPiece.objectReferenceValue = AssetDatabase.LoadAssetAtPath
   //             <GameObject>("Assets/UE Extras/Untitled Platformer Puzzle Game/Prefabs/World/VoidPiece.prefab");
   //         serializedObject.ApplyModifiedProperties();
   //     }
   //     public override void OnInspectorGUI()
   //     {
   //         _currentEvent = Event.current;
   //         serializedObject.Update();
   //         Undo.RecordObject(target, _undoText);

   //         if(GUILayout.Button("Clear Pieces"))
   //         {
   //             WorldManagerTarget.ClearPieces();
   //         }

   //         base.OnInspectorGUI();

   //         serializedObject.ApplyModifiedProperties();
   //     }
   //     public static void SetBrush(UWorldManagerBrushType brushType)
   //     {
   //         BrushType = brushType;
   //         switch (BrushType)
   //         {
   //             case UWorldManagerBrushType.None:
   //                 break;
   //             case UWorldManagerBrushType.WorldPiece:
   //                 break;
   //             case UWorldManagerBrushType.VoidPiece:
   //                 break;
   //         }
   //     }
   //     private void OnSceneGUI()
   //     {
   //         _currentEvent = Event.current;

   //         switch (_currentEvent.type)
   //         {
   //             case EventType.MouseLeaveWindow:
   //                 _cursorIsOut = true;
   //                 break;

   //             case EventType.MouseEnterWindow:
   //                 _cursorIsOut = false;
   //                 break;
   //         }

   //         switch (BrushType)
   //         {
   //             case UWorldManagerBrushType.None:
   //                 break;
   //             case UWorldManagerBrushType.WorldPiece:
   //             case UWorldManagerBrushType.VoidPiece:
   //                 if (_currentEvent.type == EventType.Layout)
   //                 {
   //                     HandleUtility.AddDefaultControl(0);
   //                 }
   //                 break;
   //         }

   //         if (_currentEvent.type == EventType.Repaint && !_cursorIsOut)
   //         {
   //             //PlayerSettings.defaultCursor = cursor_normal;
   //             switch (BrushType)
   //             {
   //                 case UWorldManagerBrushType.None:
   //                     UnityEngine.Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
   //                     break;
   //                 case UWorldManagerBrushType.WorldPiece:
   //                     UnityEngine.Cursor.SetCursor(_worldPieceMouseCursor, _hotSpot, CursorMode.Auto);
   //                     break;
   //                 case UWorldManagerBrushType.VoidPiece:
   //                     UnityEngine.Cursor.SetCursor(_voidPieceMouseCursor, _hotSpot, CursorMode.Auto);
   //                     break;
   //             }
   //             EditorGUIUtility.AddCursorRect(new Rect(0, 0, 10000, 10000), MouseCursor.CustomCursor);
   //         }

   //         Vector3 mousePosition = Event.current.mousePosition;
   //         Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);

   //         if (_currentEvent.type == EventType.MouseDown)
   //         {
   //             var pos = ray.origin;
   //             pos.x = Mathf.FloorToInt(pos.x);
   //             pos.y = Mathf.FloorToInt(pos.y);
   //             switch (BrushType)
   //             {
   //                 case UWorldManagerBrushType.None:
   //                     UnityEngine.Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
   //                     break;
   //                 case UWorldManagerBrushType.WorldPiece:
   //                     WorldManagerTarget.DrawWorldPiece(pos);
   //                     break;
   //                 case UWorldManagerBrushType.VoidPiece:
   //                     WorldManagerTarget.DrawVoidPiece(pos);
   //                     break;
   //             }
   //             WorldManagerTarget.DrawWorldPiece(pos);
   //         }

   //         if (_currentEvent.type == EventType.Repaint && _cursorIsOut)
   //         {
   //             // PlayerSettings.defaultCursor = null;
   //             UnityEngine.Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
   //         }
   //     }
   //     [InitializeOnLoadMethod]
   //     private static void InitializeOnLoad()
   //     {
   //         _worldPieceMouseCursor = AssetDatabase.LoadAssetAtPath<Texture2D>(_texturePath + "Icon_PieceBrush_Cursor.png");
   //         _voidPieceMouseCursor = AssetDatabase.LoadAssetAtPath<Texture2D>(_texturePath + "Icon_VoidBrush_Cursor.png");

   //         //SceneView.duringSceneGui += sceneView =>
   //         //{
   //         //    Handles.BeginGUI();
   //         //    {
   //         //        GUIStyle boxStyle = new GUIStyle("Box");
   //         //        GUILayout.BeginArea(new Rect(10, 10, 200, 70), boxStyle);
   //         //        {
   //         //            if (GUILayout.Button("Create World"))
   //         //            {

   //         //            }
   //         //        }
   //         //        GUILayout.EndArea();
   //         //    }
   //         //    Handles.EndGUI();
   //         //};
   //     }
        //private void OnDestroy()
        //{
        //    SceneView.duringSceneGui -= sceneView => {
        //        Handles.BeginGUI();
        //        {
        //            GUIStyle boxStyle = new GUIStyle("Box");
        //            GUILayout.BeginArea(new Rect(10, 10, 200, 70), boxStyle);
        //            {
        //                if (GUILayout.Button("Create World"))
        //                {

        //                }
        //            }
        //            GUILayout.EndArea();
        //        }
        //        Handles.EndGUI();
        //    };
        //}
        //[DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
        //static void DrawWorldCreationButton(UWorldManager levelManager, GizmoType gizmoType)
        //{
        //    if(Event.current.type == EventType.Layout) 
        //    {
        //        Handles.BeginGUI();
        //        {
        //            GUIStyle boxStyle = new GUIStyle("Box");
        //            GUILayout.BeginArea(new Rect(10, 10, 200, 70), boxStyle);
        //            {
        //                if (GUILayout.Button("Create World"))
        //                {

        //                }
        //            }
        //            GUILayout.EndArea();
        //        }
        //        Handles.EndGUI();
        //    }
        //}
    }
}
