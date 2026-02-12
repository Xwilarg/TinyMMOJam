using MMOJam.SO;
using UnityEngine;

namespace MMOJam.Zone
{
    internal class RadarBuilding : ABuilding
    {
        public override void BuildingDestroyed(FactionInfo f)
        {
            base.BuildingDestroyed(f);
            Debug.Log("[BLD] Radar destoyed");
        }

        public override void BuildingRestored(FactionInfo _)
        { }
    }
}
