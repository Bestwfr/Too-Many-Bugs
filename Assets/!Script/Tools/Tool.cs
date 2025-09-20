using System;
using FlamingOrange.CoreSystem;
using FlamingOrange.Utilities;
using UnityEngine;

namespace FlamingOrange.Tools
{
    public class Tool : MonoBehaviour
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
        
        public void Enter()
        {
            _attackCounterResetTimer.StopTimer();
            
            _animator.SetBool("Active", true);
            _animator.SetInteger("Counter", _currentAttackCounter);
            
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

        private void ResetAttackCounter() => CurrentAttackCounter = 0;

        public void SetFacingDirection(Vector2 dir) => _animator.SetFloat("Y", dir.y);

        private void OnEnable()
        {
            EventHandler.OnFinish += Exit;
            _attackCounterResetTimer.OnTimerDone += ResetAttackCounter;
        }

        private void OnDisable()
        {
            EventHandler.OnFinish -= Exit;
            _attackCounterResetTimer.OnTimerDone -= ResetAttackCounter;
        }
    }
}