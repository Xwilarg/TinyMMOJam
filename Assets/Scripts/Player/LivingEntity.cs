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

        public void RestoreHealth()
        {
            _health.Value = 10;
        }

        private void Awake()
        {
            _player = GetComponent<PlayerController>();
        }

        public void TakeDamage(FactionInfo info, int amount)
        {
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
