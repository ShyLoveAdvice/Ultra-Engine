using MoreMountains.CorgiEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ultra.UntitledNewGame
{
    public class UCharacterAbility: CharacterAbility
    {
        protected UInputManager _uInputManager;
        public override void SetInputManager(InputManager inputManager)
        {
            base.SetInputManager(inputManager);

            _uInputManager = inputManager as UInputManager;
        }
    }
}
