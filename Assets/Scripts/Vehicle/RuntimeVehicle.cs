using MMOJam.Player;
using MMOJam.SO;
using UnityEngine;

namespace MMOJam.Vehicle
{
    public class RuntimeVehicle : AInteractible
    {
        [SerializeField]
        private VehicleInfo _info;

        public override void InteractClient(PlayerController player)
        {
        }

        public override void InteractServer(PlayerController player)
        {
            player.InVehicle.Value = true;
        }
    }
}
