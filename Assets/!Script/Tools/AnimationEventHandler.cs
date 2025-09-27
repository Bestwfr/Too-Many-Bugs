using System;
using PurrNet;
using UnityEngine;

namespace FlamingOrange.Tools
{
    public class AnimationEventHandler : NetworkBehaviour
    {
        public event Action OnFinish;
        public event Action OnStartMovement;
        public event Action OnStopMovement;
        public event Action OnAttackAction;
        public event Action OnRotateSprite;
        
        
        private void AnimationFinishedTrigger() { if (isOwner) OnFinish?.Invoke(); }
        private void StartMovementTrigger() { if (isOwner) OnStartMovement?.Invoke(); }
        private void StopMovementTrigger() { if (isOwner) OnStopMovement?.Invoke(); }
        private void AttackActionTrigger() { if (isOwner) OnAttackAction?.Invoke(); }
        private void RotateSpriteTrigger() { if (isOwner) OnRotateSprite?.Invoke(); }
    }
}
