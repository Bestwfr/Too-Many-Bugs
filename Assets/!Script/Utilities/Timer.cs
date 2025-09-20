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
        
        private bool isActive;

        public Timer(float duration)
        {
            _duration = duration;
        }

        public void StartTimer()
        {
            _startTime = Time.time;
            _targetTime = _startTime + _duration;
            isActive = true;
        }

        public void StopTimer()
        {
            isActive = false;
        }

        public void Tick()
        {
            if (!isActive) return;
            
            if (Time.time > _targetTime)
            {
                OnTimerDone?.Invoke();
                StopTimer();
            }
        }
    }
}