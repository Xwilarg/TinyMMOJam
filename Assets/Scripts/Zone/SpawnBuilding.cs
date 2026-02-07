using MMOJam.Player;
using UnityEngine;

namespace MMOJam.Zone
{
    internal class SpawnBuilding : ABuilding
    {
        public override void BuildingDestroyed()
        {
            Debug.Log("[BLD] Spawn destroyed");
        }

        public void SpawnPlayer(PlayerController player)
        {
            player.transform.position = transform.position;
        }
    }
}
