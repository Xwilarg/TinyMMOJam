using UnityEngine;

namespace MMOJam
{
    [CreateAssetMenu(menuName = "ScriptableObject/Projectile")]
    public class ProjectileData : ScriptableObject
    {
        public int projectileId;

        public float speed;
        public int damage;
        public float lifetime;

        public float deviation;

        public GameObject prefab;
    }
}