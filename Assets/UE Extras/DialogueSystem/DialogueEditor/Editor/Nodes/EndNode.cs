using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ultra.DialogueSystem
{
    public class EndNode : BaseNode
    {
        private EndNodeTypes _endNodeType = EndNodeTypes.End;
        private EnumField _enumField;

        public EndNodeTypes EndNodeType { get => _endNodeType; set => _endNodeType = value; }
        public EndNode()
        {

        }
        public EndNode(Vector2 position, DialogueEditorWindow editorWindow, DialogueGraphView graphView)
        {
            _editorWindow = editorWindow;
            _graphView = graphView;

            title = "End";
            SetPosition(new Rect(position, _defaultNodeSize));
            _nodeGuid = Guid.NewGuid().ToString();

            AddInputPort("Input", Port.Capacity.Multi);

            _enumField = new EnumField()
            {
                value = _endNodeType
            };
            _enumField.Init(_endNodeType);
            _enumField.RegisterValueChangedCallback((value) =>
            {
                _endNodeType = (EndNodeTypes)value.newValue;
            });
            _enumField.SetValueWithoutNotify(_endNodeType);

            mainContainer.Add(_enumField);   

            RefreshExpandedState();
            RefreshPorts();
        }
        public override void LoadValueIntoField()
        {
            _enumField.SetValueWithoutNotify(_endNodeType);
        }
    }
}
