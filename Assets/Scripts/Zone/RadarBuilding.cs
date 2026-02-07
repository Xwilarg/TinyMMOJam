using MMOJam.SO;
using UnityEngine;

namespace MMOJam.Zone
{
    internal class RadarBuilding : ABuilding
    {
        public override void BuildingDestroyed(FactionInfo _)
        {
            Debug.Log("[BLD] Radar destoyed");
        }
    }
}
