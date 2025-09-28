using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kinnly
{
    public class CameraSystem : MonoBehaviour
    {
        public GameObject player;

        private Vector2 _offset;
        void Start()
        {
            _offset.x = 0.1f;
            _offset.y = 0.25f;
        }
        
        void Update()
        {
            if (player)
                transform.position = new Vector3(
                    player.transform.position.x + _offset.x, 
                    player.transform.position.y + _offset.y, -1);
        }
    }
}

