using MMOJam.Manager;
using MMOJam.Player;
using MMOJam.SO;
using UnityEngine;

namespace MMOJam.Zone
{
    internal class FactoryBuilding : ABuilding
    {
        public override void BuildingDestroyed(FactionInfo f)
        {
            base.BuildingDestroyed(f);
            Debug.Log("[BLD] Factory destroyed");
        }

        public override void BuildingRestored(FactionInfo f)
        {
            if (_isPlayerInside != null && _isPlayerInside.CurrentFaction.Value == f.Id) UIManager.Instance.ToggleCraftingList(true);
        }

        public override void OnZoneEnter(PlayerController player)
        {
            base.OnZoneEnter(player);
            if (player.IsLocalHuman)
            {
                if (player.CurrentFaction.Value == AttachedZone.Faction.Id)
                {
                    UIManager.Instance.ToggleCraftingList(true);
                }
            }
        }
        public override void OnZoneExit(PlayerController player)
        {
            base.OnZoneExit(player);
            if (player.IsLocalHuman)
            {
                UIManager.Instance.ToggleCraftingList(false);
            }
        }

        public override void SpawnPlayer(PlayerController pc)
        {
            if (pc.IsLocalPlayer) UIManager.Instance.ToggleCraftingList(IsAlive);
        }
    }
}
