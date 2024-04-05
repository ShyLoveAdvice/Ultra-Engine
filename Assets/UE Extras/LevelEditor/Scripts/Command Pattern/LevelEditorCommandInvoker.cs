using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ultra.LevelEditor
{ 
    public class LevelEditorCommandInvoker
    {
	    private static Stack<ICommand> _undoSlack = new Stack<ICommand>();
        private static Stack<ICommand> _redoSlack = new Stack<ICommand>();

        public static void ExecuteCommand(ICommand command)
        {
            command.Execute();
            _undoSlack.Push(command);
            _redoSlack.Clear();
        }
        public static void UndoCommand()
        {
            Debug.Log(_undoSlack.Count);
            for (int i = 0; i < _undoSlack.Count; i++)
            {
                Debug.Log(_undoSlack.ToArray()[i].GetType());
            }
            if( _undoSlack.Count > 0 )
            {
                ICommand activeCommand = _undoSlack.Pop();
                _redoSlack.Push(activeCommand);
                activeCommand.Undo();
            }
        }
        public static void RedoCommand()
        {
            if (_redoSlack.Count > 0)
            {
                ICommand activeCommand = _redoSlack.Pop();
                _undoSlack.Push(activeCommand);
                activeCommand.Execute();
            }
        }
    }
}
