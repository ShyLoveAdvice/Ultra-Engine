using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ultra.DialogueSystem
{
    public class DialogueGraphView: GraphView
    {
        private string _styleSheetsName = "GraphViewStyleSheet";
        private DialogueEditorWindow _editorWindow;
        public DialogueGraphView(DialogueEditorWindow editorWindow)
        {
            _editorWindow = editorWindow;

            StyleSheet tmpStyleSheet = Resources.Load<StyleSheet>(_styleSheetsName);
            styleSheets.Add(tmpStyleSheet);

            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new FreehandSelector());

            GridBackground grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

        }
        public void ReloadLanguage()
        {
            List<DialogueNode> dialogueNodes = nodes.ToList().Where(node => node is DialogueNode).Cast<DialogueNode>().ToList();
            foreach (DialogueNode dialogueNode in dialogueNodes)
            {
                dialogueNode.ReloadLanguage();
            }
        }
    }
}
