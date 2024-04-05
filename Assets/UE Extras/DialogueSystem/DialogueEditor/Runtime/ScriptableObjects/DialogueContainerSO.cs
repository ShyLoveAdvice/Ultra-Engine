using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ultra.DialogueSystem
{
    [CreateAssetMenu(menuName = ("Dialogue/New Dialogue"), fileName = ("new DialogueContainer")), System.Serializable]
    public class DialogueContainerSO : ScriptableObject
    {
	    
    }
    [System.Serializable]
    public class LanguageGeneric<T>
    {
        public LanguageTypes LanguageType;
        public T LanguageGenericType;
    }
    [System.Serializable]
    public class DialogueNodePort
    {
        public string InputGuid;
        public string OutputGuid;
        public Port MyPort;
        public TextField TextField;
        public List<LanguageGeneric<string>> TextLanguages = new List<LanguageGeneric<string>>();
    }
}
