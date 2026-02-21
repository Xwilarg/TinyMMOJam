using MMOJam.Manager;
using MMOJam.SO;
using Unity.Netcode;

namespace MMOJam.Player
{
    public class LivingEntity : NetworkBehaviour, IShootable
    {
        private PlayerController _player;

        private NetworkVariable<int> _health = new(10);

        //public bool IsAlive => _health.Value > 0;

        public bool IsAlive { set; get; }

        [Rpc(SendTo.Server)]
        public void RestoreHealthRpc()
        {
            _health.Value = 10;
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void ForceBoardcastIsAliveRpc(bool value) // Networkvariable isn't sync idk why
        {
            IsAlive = value;
        }

        public void RestoreHealth()
        {
            RestoreHealthRpc();
            ForceBoardcastIsAliveRpc(true);
        }

        private void Awake()
        {
            _player = GetComponent<PlayerController>();
        }

        public void TakeDamage(FactionInfo info, int amount)
        {
            if (_health.Value < 0) return;

            _health.Value -= amount;
            GameManager.Instance.TakeDamageAtRpc(transform.position, amount);

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
                    ForceBoardcastIsAliveRpc(false);

                    GameManager.Instance.CheckVictoryCondition();
                }
            }
        }
    }
}
