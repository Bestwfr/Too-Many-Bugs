using System;
using FlamingOrange.CoreSystem;
using FlamingOrange.Tools.Components;
using FlamingOrange.Utilities;
using PurrNet;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FlamingOrange.Tools
{
    public class Tool : NetworkBehaviour
    {
        [SerializeField] private float attackCounterResetCooldown;

        public ToolData Data { get; private set; }
        
        public int CurrentAttackCounter
        {
            get => _currentAttackCounter;
            private set => _currentAttackCounter = value >= Data.NumberOfAttacks ? 0 : value;
        }

        public event Action OnEnter;
        public event Action OnExit;
        
        private Animator _animator;
        public GameObject BaseGameObject {  get; private set; }
        public GameObject ToolSpriteGameObject { get; private set; }
        
        public AnimationEventHandler EventHandler {  get; private set; }
        
        public Core Core { get; private set; }

        private int _currentAttackCounter;

        private Timer _attackCounterResetTimer;
        
        private SyncVar<AttackState> syncAttackState = new(ownerAuth: true);
        private struct AttackState
        {
            public bool Active;
            public int Counter;
            public Vector2 Direction;
        }
        
        protected override void OnSpawned()
        {
            base.OnSpawned();
            syncAttackState.onChanged += OnAttackStateChanged;
        }

        public void Enter()
        {
            _attackCounterResetTimer.StopTimer();
            
            _animator.SetBool("Active", true);
            _animator.SetInteger("Counter", _currentAttackCounter);
            
            if (isOwner)
            {
                var dir = Core.Root.GetComponent<InputManager>().AttackDirection;

                syncAttackState.value = new AttackState 
                { 
                    Active = true, 
                    Counter = _currentAttackCounter, 
                    Direction = dir 
                };
            }
            
            OnEnter?.Invoke();
        }

        public void SetCore(Core core)
        {
            Core = core;
        }

        public void SetData(ToolData data)
        {
            Data = data;
        }

        public void Exit()
        {
            _animator.SetBool("Active", false);
            CurrentAttackCounter++;
            _attackCounterResetTimer.StartTimer();
            
            if (isOwner)
            {
                syncAttackState.value = new AttackState 
                { 
                    Active = false, 
                    Counter = _currentAttackCounter, 
                    Direction = Vector2.zero 
                };
            }

            OnExit?.Invoke();
        }

        private void Awake()
        {
            BaseGameObject = transform.Find("Base").gameObject;
            ToolSpriteGameObject = transform.Find("ToolSprite").gameObject;
            
            _animator = BaseGameObject.GetComponent<Animator>();
            
            EventHandler = BaseGameObject.GetComponent<AnimationEventHandler>();

            _attackCounterResetTimer = new Timer(attackCounterResetCooldown);
        }

        private void Update()
        {
            _attackCounterResetTimer.Tick();
        }
        
        private void OnAttackStateChanged(AttackState newValue)
        {
            if (isOwner) return;
            
            _animator.SetBool("Active", newValue.Active);
            _animator.SetInteger("Counter", newValue.Counter);
            
            foreach (var comp in GetComponents<ToolComponent>())
            {
                comp.UpdateAttackState(newValue.Active, newValue.Counter, newValue.Direction);
            }
        }
        
        private void ResetAttackCounter()
        {
            CurrentAttackCounter = 0;

            var state = syncAttackState.value;
            state.Counter = 0;
            syncAttackState.value = state;
        }

        public void SetFacingDirection(Vector2 dir) => _animator.SetFloat("Y", dir.y);
        
        private void HandleAnimationFinish() { if (isOwner) Exit(); }

        private void OnEnable()
        {
            EventHandler.OnFinish += HandleAnimationFinish;
            _attackCounterResetTimer.OnTimerDone += ResetAttackCounter;
        }

        private void OnDisable()
        {
            EventHandler.OnFinish -= HandleAnimationFinish;
            _attackCounterResetTimer.OnTimerDone -= ResetAttackCounter;
        }
    }
}