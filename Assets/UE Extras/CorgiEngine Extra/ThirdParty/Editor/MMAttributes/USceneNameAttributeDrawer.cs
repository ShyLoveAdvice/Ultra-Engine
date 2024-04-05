#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Experimental.GraphView;
#endif
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Codice.Utils;

namespace MoreMountains.Tools
{
    [CustomPropertyDrawer(typeof(USceneNameAttribute))]
    public class USceneNameAttributeDrawer : PropertyDrawer
    {
        int _sceneIndex = -1;
        GUIContent[] _sceneNamesGUI;
        string[] _sceneNames;
        string[] _sceneNamesWithPath;

        readonly string[] _scenePathSplit = { "/", ".unity" };
#if UNITY_EDITOR
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (EditorBuildSettings.scenes.Length == 0) return;

            if (_sceneIndex == -1)
            {
                GetSceneNameArray(property);
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"{label.text}", GUILayout.ExpandWidth(false), GUILayout.Width(200));

            ///UnityEngine.UIElements.Button sceneNameButton = new UnityEngine.UIElements.Button()

            //    text = property.stringValue
            //};

            if (GUILayout.Button($"{property.stringValue}", EditorStyles.popup))
            {
                Rect buttonScreenPos = GUIUtility.GUIToScreenRect(GUILayoutUtility.GetLastRect());
                SearchWindow.Open(new SearchWindowContext(EditorGUIUtility.GUIToScreenPoint(Event.current.mousePosition)),/*Event.current.mousePosition)),*/
                    new StringListSearchProvider(_sceneNamesWithPath, (selectedScene) =>
                    {
                        _sceneIndex = Array.FindIndex(_sceneNames, (name) => name == selectedScene);
                        property.stringValue = selectedScene;
                    },
                        "Scenes"));

                var searchWindow = new SearchWindow();
            }

            EditorGUILayout.EndHorizontal();

            property.stringValue = _sceneNames[_sceneIndex];
        }
#endif
        private void GetSceneNameArray(SerializedProperty property)
        {
            var scenes = EditorBuildSettings.scenes;

            _sceneNamesGUI = new GUIContent[scenes.Length];
            _sceneNames = new string[_sceneNamesGUI.Length];

            for (int i = 0; i < _sceneNamesGUI.Length; i++)
            {
                string path = scenes[i].path;
                string[] splitPath = path.Split(_scenePathSplit, System.StringSplitOptions.RemoveEmptyEntries);

                string sceneName = "";

                if (splitPath.Length > 0)
                {
                    sceneName = splitPath[splitPath.Length - 1];
                    _sceneNames[i] = sceneName;
                }
                else
                {
                    sceneName = "(Deleted Scene)";
                }

                bool isCorgiEngineScenes = false;

                if (splitPath.Length > 1)
                {
                    splitPath.ForEach(filename => 
                    { 
                        if (filename == "CorgiEngine")
                        {
                            isCorgiEngineScenes = true;
                        }
                    });

                    sceneName = isCorgiEngineScenes? "CorgiEngine/" + splitPath[splitPath.Length - 2] + "/" + splitPath[splitPath.Length - 1] : splitPath[splitPath.Length - 2] + "/" + splitPath[splitPath.Length - 1];
                }

                _sceneNamesGUI[i] = new GUIContent(sceneName);
            }

            if (_sceneNamesGUI.Length == 0)
            {
                _sceneNamesGUI = new[] { new GUIContent("Check Your Build Settings") };
            }

            if (!string.IsNullOrEmpty(property.stringValue))
            {
                bool nameFound = false;

                for (int i = 0; i < _sceneNames.Length; i++)
                {
                    if (_sceneNames[i] == property.stringValue)
                    {
                        _sceneIndex = i;
                        nameFound = true;
                        break;
                    }
                }
            }

            _sceneNamesWithPath = new string[_sceneNamesGUI.Length];
            for (int i = 0; i < _sceneNamesWithPath.Length; i++)
            {
                _sceneNamesWithPath[i] = _sceneNamesGUI[i].text;
            }

            if(_sceneIndex == -1)
            {
                _sceneIndex = 0;
            }

            property.stringValue = _sceneNames[_sceneIndex];
        }
    }
}