using Unity.Netcode;
using UnityEngine;
using MMOJam.Manager;
using System.Collections.Generic;

namespace MMOJam
{
    public class ProjectileManager : MonoBehaviour
    {
        public static ProjectileManager Instance { get; private set; }

        [SerializeField] private List<ProjectileData> _database;

        private Dictionary<int, ProjectileData> _lookup = new();
        private readonly List<Projectile> _activeProjectiles = new();

        private void Awake()
        {
            Instance = this;

            foreach (var data in _database)
                _lookup[data.projectileId] = data;
        }

        // ------------------------------------------------
        // CALLED ONLY BY SERVER
        // ------------------------------------------------

        public Projectile SpawnProjectile(
            int projectileId,
            Vector3 position,
            Vector3 direction,
            int factionId)
        {
            if (!_lookup.TryGetValue(projectileId, out var data))
                return null;

            var go = Instantiate(data.prefab, position, Quaternion.identity);

            var netObj = go.GetComponent<NetworkObject>();
            netObj.Spawn(); // SERVER ONLY

            var proj = go.GetComponent<Projectile>();
            proj.Initialize(projectileId, data, factionId, position, direction, ServerManager.Instance.GetFaction(factionId));

            RegisterProjectile(proj);

            return proj;
        }

        public void RegisterProjectile(Projectile proj)
        {
            if (!_activeProjectiles.Contains(proj))
                _activeProjectiles.Add(proj);
        }

        public void UnregisterProjectile(Projectile proj)
        {
            _activeProjectiles.Remove(proj);
        }

        public void DespawnProjectile(Projectile proj)
        {
            if (proj == null) return;

            UnregisterProjectile(proj);

            if (proj.NetworkObject != null && proj.NetworkObject.IsSpawned)
            {
                proj.NetworkObject.Despawn(true);
            }
        }
    }
}
