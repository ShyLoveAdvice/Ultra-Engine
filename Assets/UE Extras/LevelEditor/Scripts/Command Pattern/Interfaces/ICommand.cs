using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ultra.LevelEditor
{
    public interface ICommand
    {
        void Execute();
        void Undo();
    }
}
