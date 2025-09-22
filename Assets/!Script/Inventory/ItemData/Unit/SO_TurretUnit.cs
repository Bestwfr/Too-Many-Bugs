using UnityEngine;

namespace FlamingOrange
{
    [CreateAssetMenu(fileName = "TurretUnit", menuName = "Item/SO_TurretUnit")]
    public class SO_TurretUnit : SO_Unit
    {
        public float range = 5f;
        public float fireInterval = 0.5f;
        public float bulletSpeed = 8f;
        public float bulletLifetime = 3f;
        public Sprite bulletSprite;
        public GameObject bulletPrefab;
        public Vector2 bulletScale = Vector2.one;
        public float rotationOffset = 0f;
    }
}
