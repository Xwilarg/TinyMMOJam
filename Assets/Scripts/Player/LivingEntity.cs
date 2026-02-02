using Unity.Netcode;

namespace MMOJam.Player
{
    public class LivingEntity : NetworkBehaviour, IShootable
    {
        private PlayerController _player;

        private void Awake()
        {
            _player = GetComponent<PlayerController>();
        }

        private int _health = 10;

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
                    _player.MoveToSpawnpoint();
                }
            }
        }
    }
}
