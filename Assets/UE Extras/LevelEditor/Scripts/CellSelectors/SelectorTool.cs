using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ultra.LevelEditor
{
    public abstract class SelectorTool
    {
        protected abstract void SelectorToolInitialize(CellSelector cellSelector);
        protected abstract void SelectorToolUpdate();
    }
}
