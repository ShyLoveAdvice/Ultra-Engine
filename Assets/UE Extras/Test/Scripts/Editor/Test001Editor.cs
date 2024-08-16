using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SearchService;
using UnityEngine;

namespace Test
{
    [CustomEditor(typeof(Test001), true)]
    public class Test001Editor : Editor
    {
        static Test001Editor()
        {
            EditorApplication.playModeStateChanged += ModeChanged;
        }
        
        private static void ModeChanged(PlayModeStateChange playModeState)
        {
            switch (playModeState)
            {
                case PlayModeStateChange.ExitingPlayMode:
                    Debug.Log("ExistingPlayMode");
                    GameObject.Instantiate(new GameObject());
                    break;
                case PlayModeStateChange.EnteredEditMode:
                    break;
            }
        }
    }
}
