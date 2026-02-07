using MMOJam.Player;
using MMOJam.SO;
using UnityEngine;

namespace MMOJam.Zone
{
    internal class SpawnBuilding : ABuilding
    {
        public override void BuildingDestroyed(FactionInfo _)
        {
            Debug.Log("[BLD] Spawn destroyed");
        }

        public void SpawnPlayer(PlayerController player)
        {
            player.transform.position = transform.position;
        }
    }
}
