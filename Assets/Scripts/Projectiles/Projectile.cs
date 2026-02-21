using MMOJam.Manager;
using MMOJam.SO;
using Unity.Netcode;
using UnityEngine;

namespace MMOJam
{
    public class Projectile : NetworkBehaviour
    {
        private int _projectileId;
        private int _factionId;
        private NetworkVariable<float> _speed = new();
        private int _damage;
        private float _lifetime;

        private NetworkVariable<Vector3> _direction = new();
        private float _timer;

        private FactionInfo _faction;

        public void Initialize(int id, ProjectileData data, int factionId, Vector3 position, Vector3 direction, FactionInfo faction)
        {
            _projectileId = id;
            _speed.Value = data.speed;
            _damage = data.damage;
            _lifetime = data.lifetime;
            _factionId = factionId;
            _direction.Value = direction.normalized;
            _faction = faction;

            SyncPosRpc(position, factionId);
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void SyncPosRpc(Vector3 position, int factionId)
        {
            transform.position = position;
            var mr = GetComponentInChildren<MeshRenderer>();
            var mats = mr.materials;
            mats[0] = ServerManager.Instance.GetFaction(factionId).Material;
            mr.materials = mats;
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
            transform.position += _direction.Value * _speed.Value * Time.deltaTime;

            // Rotate to face movement direction
            if (_direction.Value != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(_direction.Value);

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
            if (!ServerManager.Instance.IsAuthority)
                return;

            if (other.TryGetComponent<IShootable>(out var living))
            {
                living.TakeDamage(_faction, _damage);
            }
            else if (other.TryGetComponent<Projectile>(out var p))
            {
                ProjectileManager.Instance.DespawnProjectile(p);
            }

            ProjectileManager.Instance.DespawnProjectile(this);
        }
    }
}
