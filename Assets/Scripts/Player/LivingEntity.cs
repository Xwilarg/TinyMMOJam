using MMOJam.Manager;
using MMOJam.SO;
using Unity.Netcode;

namespace MMOJam.Player
{
    public class LivingEntity : NetworkBehaviour, IShootable
    {
        private PlayerController _player;

        private int _health = 10;

        public bool IsAlive => _health > 0;

        private void Awake()
        {
            _player = GetComponent<PlayerController>();
        }

        public void TakeDamage(FactionInfo info, int amount)
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

                    GameManager.Instance.CheckVictoryCondition();
                }
            }
        }
    }
}
