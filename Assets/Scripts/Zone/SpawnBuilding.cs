using MMOJam.Player;
using MMOJam.SO;
using UnityEngine;

namespace MMOJam.Zone
{
    internal class SpawnBuilding : ABuilding
    {
        [SerializeField]
        private Transform _spawnPoint;

        public override void BuildingDestroyed(FactionInfo faction)
        {
            base.BuildingDestroyed(faction);
            Debug.Log("[BLD] Spawn destroyed");

            AttachedZone.ConvertZoneTo(faction);
        }

        public override void BuildingRestored(FactionInfo _)
        { }

        public void SpawnPlayer(PlayerController player)
        {
            player.transform.position = _spawnPoint.transform.position;
        }

        public override void OnZoneEnter(PlayerController player)
        { }
        public override void OnZoneExit(PlayerController player)
        { }

        public override void SpawnPlayer()
        {

        }
    }
}
