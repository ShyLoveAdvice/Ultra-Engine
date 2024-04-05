using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ultra.DialogueSystem
{
    public class DialogueEditorWindow: EditorWindow
    {
        private DialogueContainerSO _currentDialogueContainer;
        private DialogueGraphView _graphView;

        private LanguageTypes _languageType = LanguageTypes.English;
        private ToolbarMenu _toolbarMenu;
        private Label _nameOfDialogueContainer;

        public LanguageTypes LanguageType { get => _languageType; set => _languageType = value; }

        [OnOpenAsset(1)]
        public static bool ShowWindow(int instanceID, int line)
        {
            UnityEngine.Object item = EditorUtility.InstanceIDToObject(instanceID);

            if (item is DialogueContainerSO)
            {
                DialogueEditorWindow window = (DialogueEditorWindow) GetWindow(typeof(DialogueEditorWindow));
                window.titleContent = new GUIContent("Dialogue Editor");
                window._currentDialogueContainer = item as DialogueContainerSO;
                window.minSize = new Vector2(500, 250);
                window.Load();
            }

            return false;
        }
        private void OnEnable()
        {
            ConstructGraphView();
            GenerateToolbar();
            Load();
        }
        private void OnDisable()
        {
            rootVisualElement.Remove(_graphView);
        }
        private void ConstructGraphView()
        {
            _graphView = new DialogueGraphView(this);
            _graphView.StretchToParentSize();
            rootVisualElement.Add(_graphView);
        }
        private void GenerateToolbar()
        {
            StyleSheet styleSheet = Resources.Load<StyleSheet>("GraphViewStyleSheet");
            rootVisualElement.styleSheets.Add(styleSheet);

            Toolbar toolbar = new Toolbar();

            // save button
            Button saveBtn = new Button()
            {
                text = "Save",
            };
            saveBtn.clicked += () => Save();
            toolbar.Add(saveBtn);

            // load button
            Button loadBtn = new Button()
            {
                text = "Load",
            };
            loadBtn.clicked += () => Load();
            toolbar.Add(loadBtn);

            // dropdown menu for languages
            _toolbarMenu = new ToolbarMenu();
            foreach (LanguageTypes languageType in (LanguageTypes[])Enum.GetValues(typeof(LanguageTypes)))
            {
                _toolbarMenu.menu.AppendAction(languageType.ToString(), new Action<DropdownMenuAction>(x => Language(languageType, _toolbarMenu)));
            }
            toolbar.Add(_toolbarMenu);

            // name of current DialogueContainer you have open
            _nameOfDialogueContainer = new Label("");
            toolbar.Add(_nameOfDialogueContainer);
            _nameOfDialogueContainer.AddToClassList("nameOfDialogueContainer");

            rootVisualElement.Add(toolbar);
        }
        private void Load()
        {
            Debug.Log("Load");
            if(_currentDialogueContainer != null)
            {
                Language(LanguageTypes.English, _toolbarMenu);
                _nameOfDialogueContainer.text = "Name:   " + _currentDialogueContainer.name;
            }
        }
        private void Save()
        {
            Debug.Log("Save");
        }
        private void Language(LanguageTypes languageType, ToolbarMenu toolbarMenu)
        {
            toolbarMenu.text = "Language:   " + languageType.ToString();
            this._languageType = languageType;
            _graphView.ReloadLanguage();
        }
    }
}
