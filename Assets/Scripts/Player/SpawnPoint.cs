using UnityEngine;

namespace MMOJam.Player
{
    public enum SpawnType
    {
        Resource,
        Enemy
    }

    public class SpawnPoint : MonoBehaviour
    {
        [SerializeField]
        private float _radius;

        [SerializeField]
        private SpawnType _spawnType;
        public SpawnType SpawnType => _spawnType;

        private void OnDrawGizmos()
        {
            Gizmos.color = _spawnType switch
            {
                SpawnType.Resource => Color.blue,
                SpawnType.Enemy => Color.red,
                _ => Color.white
            };
            Gizmos.DrawWireSphere(transform.position, _radius);
        }

        public Vector3 GetRandomPos()
        {
            var rand = Random.insideUnitCircle * _radius;

            return new Vector3(
                transform.position.x + rand.x,
                transform.position.y,
                transform.position.z + rand.y
            );
        }
    }
}
