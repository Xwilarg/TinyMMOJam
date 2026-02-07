using MMOJam.Manager;
using Unity.Netcode;
using UnityEngine;

namespace MMOJam.Player
{
    public class LivingEntity : NetworkBehaviour, IShootable
    {
        private PlayerController _player;

        private int _health = 10;

        private void Awake()
        {
            _player = GetComponent<PlayerController>();
        }

        public void TakeDamage(int amount)
        {
            _health -= amount;

            if (_health <= 0)
            {
                if (_player.IsAi)
                {
                    Destroy(gameObject);
                }
                else
                {
                    _player.MoveToSpawnPointRpc();
                    _health = 10;
                }
            }
        }
    }
}
