using Unity.Netcode;
using UnityEngine;

namespace MMOJam.Zone
{
    public abstract class ABuilding : NetworkBehaviour, IShootable
    {
        [SerializeField]
        private int _health;

        public ZoneController AttachedZone { set; get; }

        public void TakeDamage(int amount)
        {
            _health -= amount;

            if (_health <= 0)
            {
                BuildingDestroyed();
            }
        }

        public abstract void BuildingDestroyed();
    }
}
