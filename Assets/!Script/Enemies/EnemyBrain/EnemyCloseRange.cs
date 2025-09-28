using FlamingOrange.Combat.Damage;
using FlamingOrange.Combat.KnockBack;
using FlamingOrange.Enemies.StateMachine;
using PurrNet;

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

        [ObserversRpc]
        public void Attack()
        {
            var direction = Target.value.transform.position - Core.Root.transform.position;
            
            var damageable = Target.value.GetComponent<IDamageable>();
            
            damageable?.Damage(new DamageData(Data.AttackDamage, Core.Root));
            
            var knockBackable = Target.value.GetComponent<IKnockBackable>();
            knockBackable?.KnockBack(new KnockBackData(direction.normalized, Data.KnockBack, Core.Root));
        }
    }
}