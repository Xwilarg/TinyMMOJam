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
    }
}
