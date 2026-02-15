using MMOJam.Player;
using MMOJam.SO;
using Unity.Netcode;
using UnityEngine;

namespace MMOJam.Zone
{
    public abstract class ABuilding : NetworkBehaviour, IShootable
    {
        [SerializeField]
        private int _health;

        [SerializeField]
        private MeshRenderer _flag;

        [SerializeField]
        private Material _factionNeutral;

        private int _maxHealth;

        public ZoneController AttachedZone { set; get; }

        public bool IsAlive => _health > 0;
        private int _brokenCounter = 0;

        private void Awake()
        {
            _maxHealth = _health;
        }

        public void Restore()
        {
            _health = _maxHealth;
        }

        public void UpdateFactionData()
        {
            _flag.material = AttachedZone.Faction.Material;
        }

        public void TakeDamage(FactionInfo faction, int amount)
        {
            if (faction.Id == AttachedZone.Faction.Id)
            {
                if (_brokenCounter > 0)
                {
                    _brokenCounter--;
                    if (_brokenCounter == 0)
                    {
                        BuildingRestored(faction);
                    }
                }
            }
            else
            {
                _health -= amount;

                if (_health <= 0)
                {
                    BuildingDestroyed(faction);
                    _brokenCounter = 10;
                }
            }
        }

        public abstract void SpawnPlayer();

        public virtual void BuildingDestroyed(FactionInfo faction)
        {
            _flag.material = _factionNeutral;
        }
        public abstract void BuildingRestored(FactionInfo faction);
        public abstract void OnZoneEnter(PlayerController player);
        public abstract void OnZoneExit(PlayerController player);
    }
}
