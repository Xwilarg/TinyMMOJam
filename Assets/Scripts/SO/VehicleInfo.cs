using UnityEngine;

namespace MMOJam.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/VehicleInfo", fileName = "VehicleInfo")]
    public class VehicleInfo : ScriptableObject
    {
        public string Name;
        public SeatType[] Seats;
    }

    public enum SeatType
    {
        Driver,
        Passenger,
        Miner,
        Shooter
    }
}