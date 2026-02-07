using MMOJam.SO;
using UnityEngine;

namespace MMOJam.Zone
{
    internal class FactoryBuilding : ABuilding
    {
        public override void BuildingDestroyed(FactionInfo _)
        {
            Debug.Log("[BLD] Factory destroyed");
        }
    }
}
