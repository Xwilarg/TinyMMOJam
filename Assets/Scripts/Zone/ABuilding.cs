using MMOJam.Manager;
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

        protected PlayerController _isPlayerInside;

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
            /*
            Debug.Log($"[FAC] Null check: flag ({_flag == null})");
            Debug.Log($"[FAC] Null check: zone ({AttachedZone == null})");
            Debug.Log($"[FAC] Null check: faction ({AttachedZone.Faction == null})");
            */
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
                ServerManager.Instance.TakeDamageAt(transform.position, amount);

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
        public virtual void OnZoneEnter(PlayerController player)
        {
            if (player.IsLocalHuman)
            {
                _isPlayerInside = player;
            }
        }
        public virtual void OnZoneExit(PlayerController player)
        {
            if (player.IsLocalHuman)
            {
                _isPlayerInside = null;
            }
        }
    }
}
