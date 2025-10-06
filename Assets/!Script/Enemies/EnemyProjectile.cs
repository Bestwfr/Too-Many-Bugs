using System;
using FlamingOrange.Utilities;
using FlamingOrange.Combat.Damage;
using FlamingOrange.Combat.KnockBack;
using PurrNet;
using UnityEngine;

namespace FlamingOrange.Enemies
{
    public class EnemyProjectile : NetworkBehaviour
    {
        private float _damage;
        private float _lifetime;
        private float _knockback;
        private float _speed;
        
        private GameObject _source;
        
        private Vector2 _direction;
       
        private Timer _destroyTimer;

        private void Start()
        {
            _destroyTimer ??= new Timer(_lifetime); 
            _destroyTimer.StartTimer();
        }

        protected override void OnSpawned()
        {
            base.OnSpawned();
            
            if (_destroyTimer != null) _destroyTimer.OnTimerDone += Destroy;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            if (_destroyTimer != null) _destroyTimer.OnTimerDone -= Destroy;
        }

        private void Update()
        {
            _destroyTimer.Tick();
        }

        private void FixedUpdate()
        {
            if (!isServer) return;
            if (TryGetComponent<Rigidbody2D>(out var rb))
                rb.linearVelocity = _direction * _speed;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!(other.CompareTag("Player") || other.CompareTag("Base"))) return;
            
            if (isServer)
            {
                var damageable = other.GetComponent<IDamageable>();
                damageable?.Damage(new DamageData(_damage, _source));
            }
            
            var direction = other.transform.position - transform.position;
            
            var knockBackable = other.GetComponent<IKnockBackable>();
            knockBackable?.KnockBack(new KnockBackData(direction.normalized, _knockback, _source));
            
            Destroy(gameObject);
        }

        public void Initialize(float damage,float lifetime, float knockback,GameObject source, Vector2 direction, float speed)
        {
            _damage = damage;
            _lifetime = lifetime;
            _knockback = knockback;
            _source = source;
            _direction = direction;
            _speed = speed;
        }

        private void Destroy()
        {
            Destroy(gameObject);
        }
    }
}