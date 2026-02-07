using MMOJam.Player;
using MMOJam.SO;
using UnityEngine;

namespace MMOJam.Zone
{
    internal class SpawnBuilding : ABuilding
    {
        public override void BuildingDestroyed(FactionInfo faction)
        {
            Debug.Log("[BLD] Spawn destroyed");

            AttachedZone.ConvertZoneTo(faction);
        }

        public void SpawnPlayer(PlayerController player)
        {
            player.transform.position = transform.position;
        }
    }
}
