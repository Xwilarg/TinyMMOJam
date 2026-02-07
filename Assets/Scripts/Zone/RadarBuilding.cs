using UnityEngine;

namespace MMOJam.Zone
{
    internal class RadarBuilding : ABuilding
    {
        public override void BuildingDestroyed()
        {
            Debug.Log("[BLD] Radar destoyed");
        }
    }
}
