using MoreMountains.CorgiEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShootGame
{
    public class CharacterFlip : CharacterAbility
    {
        private Vector3 _mouseWorldPos;

        public override void ProcessAbility()
        {
            base.ProcessAbility();

            _mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if ((_mouseWorldPos.x < _controller.ColliderCenterPosition.x && _character.IsFacingRight) ||
               (_mouseWorldPos.x > _controller.ColliderCenterPosition.x && !_character.IsFacingRight))
            {
                _character.Flip(true);
            }
        }
    }
}
