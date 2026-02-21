using UnityEngine;

namespace MMOJam.Projectiles
{
    public class DamageDisplay : MonoBehaviour
    {
        private void Awake()
        {
            Destroy(gameObject, 3f);
        }

        private void Update()
        {
            transform.Translate(Vector3.up * Time.deltaTime);
        }
    }
}
