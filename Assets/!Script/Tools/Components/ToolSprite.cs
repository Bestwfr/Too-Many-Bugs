using System;
using UnityEngine;

namespace FlamingOrange.Tools.Components
{
    public class ToolSprite : ToolComponent<ToolSpriteData, AttackSprites>
    {
        private SpriteRenderer _baseSpriteRenderer;
        private SpriteRenderer _toolSpriteRenderer;

        private int _currentToolSpriteIndex;

        protected override void HandleEnter()
        {
            base.HandleEnter();
            _currentToolSpriteIndex = 0;
        }
        
        protected override void HandleExit()
        {
            base.HandleExit();
            _toolSpriteRenderer.sprite = null;
        }

        private void HandleBaseSpriteChange(SpriteRenderer sr)
        {
            if (!isAttackActive)
            {
                _toolSpriteRenderer.sprite = null;
                Debug.Log("baseSprite now null");
                return;
            }
            
            var currentAttackSprites = currentAttackData.Sprites;

            if (_currentToolSpriteIndex >= currentAttackSprites.Length)
            {
                Debug.LogWarning($"{tool.name} tool sprite length mismatch");
                return;
            }
            
            _toolSpriteRenderer.sprite = currentAttackSprites[_currentToolSpriteIndex];
            
            _currentToolSpriteIndex++;
        }

        protected override void Start()
        {
            base.Start();
            
            _baseSpriteRenderer = tool.BaseGameObject.GetComponent<SpriteRenderer>();
            _toolSpriteRenderer = tool.ToolSpriteGameObject.GetComponent<SpriteRenderer>();
            
            _baseSpriteRenderer.RegisterSpriteChangeCallback(HandleBaseSpriteChange);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            _baseSpriteRenderer.UnregisterSpriteChangeCallback(HandleBaseSpriteChange);
        }
    }
}