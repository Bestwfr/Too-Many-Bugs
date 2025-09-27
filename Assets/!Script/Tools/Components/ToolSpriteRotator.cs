using FlamingOrange.CoreSystem;
using UnityEngine;

namespace FlamingOrange.Tools.Components
{
    public class ToolSpriteRotator : ToolComponent
    {
        private GameObject _toolSprite;
        private float _angleDegree;
        
        private InputManager _inputManager;
        
        private void HandleAttackInput(Vector2 direction)
        {
            direction.Normalize();
            _angleDegree = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            
            if (direction.x < -0.01f)
                _angleDegree -= 180f;
        }

        private void HandleRotateSprite()
        {
            if (_toolSprite == null) return;
            
            _toolSprite.transform.rotation = Quaternion.Euler(0f, 0f, _angleDegree);
        }

        protected override void OnDirectionChanged(Vector2 dir)
        {
            if (_toolSprite == null)
                return;

            _angleDegree = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (dir.x < -0.01f)
                _angleDegree -= 180f;

            _toolSprite.transform.rotation = Quaternion.Euler(0f, 0f, _angleDegree);
        }

        protected override void Start()
        {
            base.Start();
            _inputManager = Core.Root.GetComponent<InputManager>();
            
            _toolSprite = tool.ToolSpriteGameObject;
            
            _inputManager.OnAttack += HandleAttackInput;

            eventHandler.OnRotateSprite += HandleRotateSprite;
        }
     
        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            _inputManager.OnAttack -= HandleAttackInput;
            
            eventHandler.OnRotateSprite -= HandleRotateSprite;
        }
    }
}