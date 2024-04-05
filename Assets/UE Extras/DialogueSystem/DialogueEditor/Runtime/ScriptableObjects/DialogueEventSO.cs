using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ultra.DialogueSystem
{
    [CreateAssetMenu(menuName = ("Dialogue/New Dialogue Event")), System.Serializable]
    public class DialogueEventSO : ScriptableObject
    {
	    public virtual void RunEvent()
        {
            Debug.Log("Event was call");
        }
    }
}
