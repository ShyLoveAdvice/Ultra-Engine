using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.CorgiEngine
{
    public class DamageOnTouchStun : DamageOnTouch
    {
        public enum StunStyles { NoStun, EqualToInvincibleTime, CustomStunTime }
        [Header("Stun")]
        public StunStyles DamagedCausedStunStyle = StunStyles.EqualToInvincibleTime;
        public float StunDuration = 0f;
        protected override void OnCollideWithDamageable(Health health)
        {
            if (health.CanTakeDamageThisFrame())
            {
                ApplyDamageCausedStun(health);
            }

            base.OnCollideWithDamageable(health);
        }

        protected virtual void ApplyDamageCausedStun(Health health)
        {
            if (!ShouldApplyCausedStun(health))
            {
                return;
            }

            CharacterStun characterStun = health.GetComponent<CharacterStun>();

            if (characterStun == null)
            {
                return;
            }

            switch (DamagedCausedStunStyle)
            {
                case StunStyles.NoStun: break;
                case StunStyles.EqualToInvincibleTime:
                    characterStun.StunFor(InvincibilityDuration); break;
                case StunStyles.CustomStunTime:
                    characterStun.StunFor(StunDuration); break;
            }
        }
        protected virtual bool ShouldApplyCausedStun(Health health)
        {
            return (health.AssociatedController != null)
                   && (DamagedCausedStunStyle != StunStyles.NoStun)
                   && !_colliderHealth.Invulnerable
                   && !_colliderHealth.PostDamageInvulnerable;
        }
    }
}
