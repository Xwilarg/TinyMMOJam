using MMOJam.Manager;
using Unity.Netcode;

namespace MMOJam.Player
{
    public class LivingEntity : NetworkBehaviour, IShootable
    {
        private PlayerController _player;

        private void Awake()
        {
            _player = GetComponent<PlayerController>();

            _health.OnValueChanged += (oldValue, newValue) =>
            {
                if (_health.Value <= 0)
                {
                    if (_player.IsAi)
                    {
                        Destroy(gameObject);
                    }
                    else
                    {
                        if (IsOwner) _player.MoveToSpawnpoint();
                        if (ServerManager.Instance.IsAuthority) _health.Value = 10;
                    }
                }
            };
        }

        private NetworkVariable<int> _health = new(10);

        public void TakeDamage(int amount)
        {
            _health.Value -= amount;
        }
    }
}
