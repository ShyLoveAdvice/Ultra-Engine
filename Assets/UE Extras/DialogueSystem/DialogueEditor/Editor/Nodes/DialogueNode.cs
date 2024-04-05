using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Video;

namespace Ultra.DialogueSystem
{
    public class DialogueNode : BaseNode
    {
        private List<LanguageGeneric<string>> _texts = new List<LanguageGeneric<string>>();
        private List<LanguageGeneric<AudioClip>> _audioClips = new List<LanguageGeneric<AudioClip>>();
        private string _name = "";
        private Sprite _faceImage;
        private DialogueImageFaceTypes _imageFaceType;

        private List<DialogueNodePort> _dialogueNodePorts = new List<DialogueNodePort>();

        public List<LanguageGeneric<string>> Texts { get => _texts; set => _texts = value; }
        public List<LanguageGeneric<AudioClip>> AudioClips { get => _audioClips; set => _audioClips = value; }
        public string Name { get => _name; set => _name = value; }
        public Sprite FaceImage { get => _faceImage; set => _faceImage = value; }
        public DialogueImageFaceTypes ImageFaceType { get => _imageFaceType; set => _imageFaceType = value; }

        private TextField _texts_Field;
        private ObjectField _audioClips_Field;
        private ObjectField _faceImage_Field;
        private TextField _name_Field;
        private EnumField _imageFaceType_Field;

        public DialogueNode()
        {

        }
        public DialogueNode(Vector2 position, DialogueEditorWindow editorWindow, DialogueGraphView graphView)
        {
            _editorWindow = editorWindow;
            _graphView = graphView;

            title = "Dialogue";
            SetPosition(new Rect(position, _defaultNodeSize));
            _nodeGuid = Guid.NewGuid().ToString();

            AddInputPort("Input", Port.Capacity.Multi);

            foreach (LanguageTypes language in (LanguageTypes[])(Enum.GetValues(typeof(LanguageTypes))))
            {
                _texts.Add(new LanguageGeneric<string>
                {
                    LanguageType = language,
                    LanguageGenericType = ""
                });

                _audioClips.Add(new LanguageGeneric<AudioClip>
                {
                    LanguageType = language,
                    LanguageGenericType = null
                });
            }

            // Face Image
            _faceImage_Field = new ObjectField()
            {
                objectType = typeof(Sprite),
                allowSceneObjects = false,
                value = _faceImage
            };
            _faceImage_Field.RegisterValueChangedCallback(value =>
            {
                _faceImage = value.newValue as Sprite;
            });
            mainContainer.Add(_faceImage_Field);

            // Face Image Enum
            _imageFaceType_Field = new EnumField()
            {
                value = _imageFaceType
            };
            _imageFaceType_Field.Init(_imageFaceType);
            _imageFaceType_Field.RegisterValueChangedCallback(value =>
            {
                _imageFaceType = (DialogueImageFaceTypes)value.newValue;
            });
            mainContainer.Add(_faceImage_Field);

            // Audio Clip
            _audioClips_Field = new ObjectField()
            {
                objectType = typeof(AudioClip),
                allowSceneObjects = false,
                value = _audioClips.Find(audioClip => audioClip.LanguageType == _editorWindow.LanguageType).LanguageGenericType
            };
            _audioClips_Field.RegisterValueChangedCallback(value =>
            {
                _audioClips.Find(audioClip => audioClip.LanguageType == _editorWindow.LanguageType).LanguageGenericType = value.newValue as AudioClip;
            });
            _audioClips_Field.SetValueWithoutNotify(_audioClips.Find(audioClip => audioClip.LanguageType == _editorWindow.LanguageType).LanguageGenericType);
            mainContainer.Add(_audioClips_Field);

            // Text Name
            Label label_name = new Label("Name");
            label_name.AddToClassList("label_name");
            label_name.AddToClassList("Label");
            mainContainer.Add(label_name);

            _name_Field = new TextField("Name");
            _texts_Field.RegisterValueChangedCallback(value =>
            {
                name = value.newValue;
            });
            _texts_Field.SetValueWithoutNotify(name);
            _texts_Field.AddToClassList("TextName");
            mainContainer.Add(_texts_Field);

            // Text Box
            Label label_text = new Label("Text Box");
            label_text.AddToClassList("label_text");
            label_text.AddToClassList("Label");
            mainContainer.Add(label_text);

            _texts_Field = new TextField("");
            _texts_Field.RegisterValueChangedCallback(value =>
            {
                _texts.Find(text => text.LanguageType == editorWindow.LanguageType).LanguageGenericType = value.newValue;
            });
            _texts_Field.SetValueWithoutNotify(_texts.Find(text => text.LanguageType == editorWindow.LanguageType).LanguageGenericType);
            _texts_Field.multiline = true;

            _texts_Field.AddToClassList("TextBox");
            mainContainer.Add(_texts_Field);

            Button button = new Button()
            {
                text = "Add Choice"
            };
            button.clicked += () =>
            {
                //TODO: add a new Choice Output Port
            };

            titleButtonContainer.Add(button);
        }
        public void ReloadLanguage()
        {
            _texts_Field.RegisterValueChangedCallback(value =>
            {
                _texts.Find(text => text.LanguageType == _editorWindow.LanguageType).LanguageGenericType = value.newValue;
            });
            _texts_Field.SetValueWithoutNotify(_texts.Find(text => text.LanguageType == _editorWindow.LanguageType).LanguageGenericType);

            _audioClips_Field = new ObjectField()
            {
                objectType = typeof(AudioClip),
                allowSceneObjects = false,
                value = _audioClips.Find(audioClip => audioClip.LanguageType == _editorWindow.LanguageType).LanguageGenericType
            };
            _audioClips_Field.SetValueWithoutNotify(_audioClips.Find(audioClip => audioClip.LanguageType == _editorWindow.LanguageType).LanguageGenericType);

            foreach (DialogueNodePort nodePort in _dialogueNodePorts)
            {
                nodePort.TextField.RegisterValueChangedCallback(value =>
                {
                    nodePort.TextLanguages.Find(language => language.LanguageType == _editorWindow.LanguageType).LanguageGenericType = value.newValue;
                });
                nodePort.TextField.SetValueWithoutNotify(nodePort.TextLanguages.Find(language => language.LanguageType == _editorWindow.LanguageType).LanguageGenericType);
            }
        }
        public override void LoadValueIntoField()
        {
            _texts_Field.SetValueWithoutNotify(_texts.Find(language => language.LanguageType == _editorWindow.LanguageType).LanguageGenericType);
            _audioClips_Field.SetValueWithoutNotify(_audioClips.Find(audioClip => audioClip.LanguageType == _editorWindow.LanguageType).LanguageGenericType);
            _faceImage_Field.SetValueWithoutNotify(_faceImage);
            _imageFaceType_Field.SetValueWithoutNotify(_imageFaceType);
            _name_Field.SetValueWithoutNotify(_name);
        }
        public Port AddChoicePort(BaseNode baseNode, DialogueNodePort dialogueNodePort = null)
        {
            Port port = GetPortInstance(Direction.Output);

            int outputPortCount = baseNode.outputContainer.Query("connector").ToList().Count;
            string outputPortName = $"Choice {outputPortCount + 1}";

            DialogueNodePort dnPort = new DialogueNodePort();

            foreach (LanguageTypes language in (LanguageTypes[])(Enum.GetValues(typeof(LanguageTypes))))
            {
                dnPort.TextLanguages.Add(new LanguageGeneric<string>()
                {
                    LanguageType = language,
                    LanguageGenericType = outputPortName
                });

            }
            if (dialogueNodePort != null)
            {
                dnPort.InputGuid = dialogueNodePort.InputGuid;
                dnPort.OutputGuid = dialogueNodePort.OutputGuid;

                foreach (LanguageGeneric<string> languageGeneric in dialogueNodePort.TextLanguages)
                {
                    dnPort.TextLanguages.Find(language => language.LanguageType == languageGeneric.LanguageType).LanguageGenericType = languageGeneric.LanguageGenericType;
                }
            }

            // Text for the port
            dnPort.TextField = new TextField();
            dnPort.TextField.RegisterValueChangedCallback(value =>
            {
                dnPort.TextLanguages.Find(language => language.LanguageType == _editorWindow.LanguageType).LanguageGenericType = value.newValue;
            });
            dnPort.TextField.SetValueWithoutNotify(dialogueNodePort.TextLanguages.Find(language => language.LanguageType == _editorWindow.LanguageType).LanguageGenericType);
            port.contentContainer.Add(dnPort.TextField);

            // Delete Button
            Button deleteButton = new Button(() => DeletePort(baseNode, port))
            {
                text = "X"
            };

            port.contentContainer.Add(deleteButton);

            dnPort.MyPort = port;
            port.portName = "";

            _dialogueNodePorts.Add(dnPort);

            baseNode.outputContainer.Add(port);

            baseNode.RefreshPorts();
            baseNode.RefreshExpandedState();

            return port;
        }
        private void DeletePort(BaseNode baseNode, Port port)
        {
            DialogueNodePort tmp = _dialogueNodePorts.Find(cport => cport.MyPort == port);
            _dialogueNodePorts.Remove(tmp);

            IEnumerable<Edge> portEdges = _graphView.edges.ToList().Where(edge => edge.output == port);

            if (portEdges.Any())
            {
                Edge edge = portEdges.First();
                edge.input.Disconnect(edge);
                edge.output.Disconnect(edge);
                _graphView.RemoveElement(edge);
            }

            baseNode.outputContainer.Remove(port);

            baseNode.RefreshPorts();
            baseNode.RefreshExpandedState();
        }
    }
}
