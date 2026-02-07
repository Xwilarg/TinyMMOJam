using MMOJam.SO;
using Unity.Netcode;
using UnityEngine;

namespace MMOJam.Zone
{
    public abstract class ABuilding : NetworkBehaviour, IShootable
    {
        [SerializeField]
        private int _health;

        private int _maxHealth;

        public ZoneController AttachedZone { set; get; }

        private MeshRenderer _renderer;

        public bool IsAlive => _health > 0;

        private void Awake()
        {
            _renderer = GetComponent<MeshRenderer>();
            _maxHealth = _health;
        }

        public void Restore()
        {
            _health = _maxHealth;
        }

        public void UpdateFactionData()
        {
            _renderer.material = AttachedZone.Faction.Material;
        }

        public void TakeDamage(FactionInfo faction, int amount)
        {
            if (faction.Id == AttachedZone.Faction.Id) return;

            _health -= amount;

            if (_health <= 0)
            {
                BuildingDestroyed(faction);
            }
        }

        public abstract void BuildingDestroyed(FactionInfo faction);
    }
}
