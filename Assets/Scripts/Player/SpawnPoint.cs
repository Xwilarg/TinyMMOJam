using UnityEngine;

namespace MMOJam.Player
{
    public class SpawnPoint : MonoBehaviour
    {
        [SerializeField]
        private float _radius;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
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
