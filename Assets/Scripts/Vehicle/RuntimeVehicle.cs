using MMOJam.SO;
using Unity.Netcode;
using UnityEngine;

namespace MMOJam.Vehicle
{
    public class RuntimeVehicle : NetworkBehaviour
    {
        [SerializeField]
        private VehicleInfo _info;
    }
}
