using MMOJam.SO;
using Unity.Netcode;
using UnityEngine;

namespace MMOJam.Zone
{
    public abstract class ABuilding : NetworkBehaviour, IShootable
    {
        [SerializeField]
        private int _health;

        public ZoneController AttachedZone { set; get; }

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
