using UnityEngine;

namespace FlamingOrange.Tools.Components
{
    public class Movement : ToolComponent<MovementData, AttackMovement>
    {
        private CoreSystem.Movement _coreMovement;
        
        private CoreSystem.Movement CoreMovement => 
            _coreMovement ? _coreMovement : Core.GetCoreComponent(ref _coreMovement);
        
        private Vector2 _inputDirection;
        
        private void HandleStartMovement()
        {
            CoreMovement.AddForce(currentAttackData.MoveForce * _inputDirection);
        }
        
        private void HandleStopMovement()
        {
            CoreMovement.SetVelocity(Vector2.zero);
        }

        private void HandleAttackDirection(Vector2 direction)
        {
            _inputDirection = direction;
        }

        protected override void Start()
        {
            base.Start();
            
            eventHandler.OnStartMovement += HandleStartMovement;
            eventHandler.OnStopMovement += HandleStopMovement;

            InputManager.Instance.OnAttack += HandleAttackDirection;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            eventHandler.OnStartMovement -= HandleStartMovement;
            eventHandler.OnStopMovement -= HandleStopMovement;
            
            InputManager.Instance.OnAttack -= HandleAttackDirection;
        }
    }
}