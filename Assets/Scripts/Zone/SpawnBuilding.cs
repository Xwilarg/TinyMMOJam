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

        public void DoSpawnPlayer(PlayerController player)
        {
            player.MoveTo(_spawnPoint.transform.position);
        }

        public override void OnZoneEnter(PlayerController player)
        {
            base.OnZoneEnter(player);
        }
        public override void OnZoneExit(PlayerController player)
        {
            base.OnZoneExit(player);
        }

        public override void SpawnPlayer(PlayerController _)
        {

        }
    }
}
