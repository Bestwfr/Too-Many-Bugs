using System;
using FlamingOrange.CoreSystem;
using FlamingOrange.Tools;
using Kinnly;
using PurrNet;
using UnityEngine;

    public class Player : NetworkBehaviour
    {
        #region State Variables

        public PlayerStateMachine StateMachine { get; private set; }
        
        public PlayerIdleState IdleState { get; private set; }
        public PlayerMoveState MoveState { get; private set; }
        public PlayerStopState StopState { get; private set; }
        public PlayerDashState DashState { get; private set; }
        public PlayerAttackState AttackState { get; private set; }
        
        [SerializeField] private PlayerData playerData;
        #endregion

        #region Components

        public Core Core { get; private set; }
        
        public Animator Anim { get; private set;}
        public InputManager Input { get; private set; }

        #endregion

        #region Other Variables

        [SerializeField] private Tool tool;
        
        private InputBuffer<Vector2> _attackBuffer;

        #endregion

        #region Unity Callbacks Functions

        private void Awake()
        {
            Core = GetComponentInChildren<Core>();

            tool.SetCore(Core);
            
            Input = GetComponent<InputManager>();
            
            _attackBuffer = new InputBuffer<Vector2>(0.2f);
            
            StateMachine = new PlayerStateMachine();
            
            IdleState = new PlayerIdleState(this, StateMachine, playerData, "Idle");
            MoveState = new PlayerMoveState(this, StateMachine, playerData, "Move");
            StopState = new PlayerStopState(this, StateMachine, playerData, "Stop");
            DashState = new PlayerDashState(this, StateMachine, playerData, "Dash");
            AttackState = new PlayerAttackState(this, StateMachine, playerData, "Attack", tool);
        }

        private void Start()
        {
            Anim = GetComponent<Animator>();
            
            StateMachine.Initialize(IdleState);
            
            Debug.Log("Owner: " + isOwner);
            if (isOwner) Input.OnAttack += HandleAttackEvent;
        }

        private void Update()
        {
            if (!isOwner) return;
            
            Core.LogicUpdate();
            StateMachine.CurrentState.LogicUpdate();
        }

        private void FixedUpdate()
        {
            if (!isOwner) return;
            
            Core.PhysicsUpdate();
            StateMachine.CurrentState.PhysicsUpdate();
        }

        private void OnDisable()
        {
            if(isOwner) Input.OnAttack -= HandleAttackEvent;
        }

        #endregion

        #region Network Functions

        protected override void OnSpawned()
        {
            base.OnSpawned();

            if (!isOwner) return;
            
            if (Camera.main)
            {
                Camera.main.GetComponent<CameraSystem>().player = gameObject;
            }
        }

        #endregion

        #region Event Functions
        
        private void HandleAttackEvent(Vector2 dir)
        {
            if (StateMachine.CurrentState == null || AttackState == null) return;

            var canChangeToAttackState = StateMachine.CurrentState == IdleState ||
                                         StateMachine.CurrentState == StopState ||
                                         StateMachine.CurrentState == MoveState;
            if (canChangeToAttackState)
            {
                AttackState.RequestDirection(dir);
                StateMachine.ChangeState(AttackState);
                return;
            }

            _attackBuffer.Buffer(dir);
        }

        #endregion

        #region Other Functions
        
        public bool TryConsumeAttack(out Vector2 dir)
        {
            return _attackBuffer.TryConsume(out dir);
        }
        
        private void AnimationTrigger() { if (isOwner) StateMachine.CurrentState.AnimationTrigger(); }
        private void AnimationFinishedTrigger() { if (isOwner) StateMachine.CurrentState.AnimationFinishedTrigger(); }

        #endregion
    }