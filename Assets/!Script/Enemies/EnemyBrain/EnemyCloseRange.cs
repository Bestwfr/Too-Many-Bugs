using FlamingOrange.Enemies.StateMachine;

namespace FlamingOrange.Enemies
{
    public class EnemyCloseRange : Enemy<CloseRangeEnemyData>
    {
        public ChaseState<EnemyCloseRange, CloseRangeEnemyData> ChaseState { get; private set; }
        public override State AttackState { get; protected set; }

        public override void Awake()
        {
            base.Awake();
            ChaseState = new ChaseState<EnemyCloseRange, CloseRangeEnemyData> (this, StateMachine, "Chase", this);
            AttackState = new CloseAttackState(this, StateMachine, "Attack", this);
        }

        protected override void OnSpawned()
        {
            base.OnSpawned();
            if (isServer) StateMachine.Initialize(ChaseState);
        }
    }
}