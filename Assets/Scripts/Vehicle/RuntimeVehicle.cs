using MMOJam.Player;
using MMOJam.SO;
using UnityEngine;

namespace MMOJam.Vehicle
{
    public class RuntimeVehicle : AInteractible
    {
        [SerializeField]
        private VehicleInfo _info;

        public override void Interact(PlayerController player)
        {
            Debug.Log("Player interacted with me!");
        }
    }
}
