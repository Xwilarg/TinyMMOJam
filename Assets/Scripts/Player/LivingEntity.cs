using Unity.Netcode;
using UnityEngine;

namespace MMOJam.Player
{
    public class LivingEntity : NetworkBehaviour, IShootable
    {
        private int _health = 10;

        public void TakeDamage(int amount)
        {
            _health -= amount;

            if (_health <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
