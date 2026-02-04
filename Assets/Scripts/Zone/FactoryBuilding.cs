using UnityEngine;

namespace MMOJam.Zone
{
    internal class FactoryBuilding : ABuilding
    {
        public override void BuildingDestroyed()
        {
            Debug.Log("[BLD] Factory destroyed");
        }
    }
}
