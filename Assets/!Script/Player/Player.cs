using FlamingOrange.CoreSystem;
using FlamingOrange.Tools;
using UnityEngine;

    public class Player : MonoBehaviour
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
        
        private void OnEnable()
        {
            Input.OnAttack += HandleAttackEvent;
        }

        private void OnDisable()
        {
            Input.OnAttack -= HandleAttackEvent;
        }

        private void Start()
        {
            Anim = GetComponent<Animator>();
            
            StateMachine.Initialize(IdleState);
        }

        private void Update()
        {
            Core.LogicUpdate();
            StateMachine.CurrentState.LogicUpdate();
        }

        private void FixedUpdate()
        {
            Core.PhysicsUpdate();
            StateMachine.CurrentState.PhysicsUpdate();
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
        
        private void AnimationTrigger() => StateMachine.CurrentState.AnimationTrigger();
        private void AnimationFinishedTrigger() => StateMachine.CurrentState.AnimationFinishedTrigger();

        #endregion
    }