using MMOJam.Manager;
using MMOJam.SO;
using Unity.Netcode;

namespace MMOJam.Player
{
    public class LivingEntity : NetworkBehaviour, IShootable
    {
        private PlayerController _player;

        private NetworkVariable<int> _health = new(10);

        public bool IsAlive => _health.Value > 0;

        [Rpc(SendTo.Server)]
        public void RestoreHealthRpc()
        {
            _health.Value = 10;
        }

        public void RestoreHealth()
        {
            RestoreHealthRpc();
        }

        private void Awake()
        {
            _player = GetComponent<PlayerController>();
        }

        public void TakeDamage(FactionInfo info, int amount)
        {
            if (_health.Value < 0) return;

            _health.Value -= amount;

            if (_health.Value <= 0)
            {
                if (_player.IsAi)
                {
                    Destroy(gameObject);
                }
                else
                {
                    _player.Die();
                    _player.MoveToSpawnPointRpc();

                    GameManager.Instance.CheckVictoryCondition();
                }
            }
        }
    }
}
