using System;
using UnityEngine;

namespace FlamingOrange.Utilities
{
    public class Timer
    {
        public event Action OnTimerDone;
        
        private float _startTime;
        private float _duration;
        private float _targetTime;
        
        public bool IsActive { get; private set; }
        
        public float RemainingTime
        {
            get
            {
                if (!IsActive) return 0f;
                return Mathf.Max(0f, _targetTime - Time.time);
            }
        }

        public Timer(float duration)
        {
            _duration = duration;
        }

        public void StartTimer()
        {
            _startTime = Time.time;
            _targetTime = _startTime + _duration;
            IsActive = true;
        }

        public void StopTimer()
        {
            IsActive = false;
        }

        public void Tick()
        {
            if (!IsActive) return;
            
            if (Time.time > _targetTime)
            {
                OnTimerDone?.Invoke();
            }
        }
    }
}