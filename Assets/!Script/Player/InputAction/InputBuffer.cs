using System;
using System.Collections.Generic;
using UnityEngine;

public class InputBuffer<T>
{
    private struct BufferedEntry
    {
        public float time;
        public T value;
    }

    private readonly float _bufferTime;
    private readonly Queue<BufferedEntry> _queue = new();

    public InputBuffer(float bufferTime = 0.15f)
    {
        _bufferTime = bufferTime;
    }

    public void Buffer(T value)
    {
        _queue.Enqueue(new BufferedEntry { time = Time.time, value = value });
    }

    public bool TryConsume(out T value)
    {
        while (_queue.Count > 0)
        {
            var e = _queue.Peek();
            if (Time.time - e.time <= _bufferTime)
            {
                value = e.value;
                _queue.Dequeue();
                return true;
            }
            else
            {
                // expired → drop it
                _queue.Dequeue();
            }
        }
        value = default;
        return false;
    }

    public void Clear() => _queue.Clear();
}