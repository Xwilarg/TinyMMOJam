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

        public override void BuildingRestored(FactionInfo _)
        { }

        public override void OnZoneEnter(PlayerController player)
        { }
        public override void OnZoneExit(PlayerController player)
        { }

        public override void SpawnPlayer()
        {

        }
    }
}
