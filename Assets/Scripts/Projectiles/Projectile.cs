using Unity.Netcode;
using UnityEngine;
using MMOJam.Player;
using MMOJam.Manager;

namespace MMOJam
{
    public class Projectile : NetworkBehaviour
    {
        private int _projectileId;
        private int _factionId;
        private float _speed;
        private int _damage;
        private float _lifetime;

        private Vector3 _direction;
        private float _timer;

        public void Initialize(int id, ProjectileData data, int factionId, Vector3 direction)
        {
            _projectileId = id;
            _speed = data.speed;
            _damage = data.damage;
            _lifetime = data.lifetime;
            _factionId = factionId;
            _direction = direction.normalized;
        }

        public override void OnNetworkSpawn()
        {
            ProjectileManager.Instance.RegisterProjectile(this);
        }

        public override void OnNetworkDespawn()
        {
            ProjectileManager.Instance.UnregisterProjectile(this);
        }

        private void Update()
        {
            // Move
            transform.position += _direction * _speed * Time.deltaTime;

            // Rotate to face movement direction
            if (_direction != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(_direction);

            if (!IsServer)
                return;

            _timer += Time.deltaTime;

            if (_timer >= _lifetime)
            {
                ProjectileManager.Instance.DespawnProjectile(this);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsServer)
                return;

            if (other.TryGetComponent(out LivingEntity entity))
            {
                if (other.TryGetComponent(out PlayerController player))
                {
                    if (player.CurrentFaction.Value == _factionId)
                        return;
                }

                entity.TakeDamage(
                    ServerManager.Instance.GetFaction(_factionId),
                    _damage
                );

                ProjectileManager.Instance.DespawnProjectile(this);
            }
        }
    }
}
