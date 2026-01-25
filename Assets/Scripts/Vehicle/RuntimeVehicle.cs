using MMOJam.Player;
using MMOJam.SO;
using UnityEngine;
using static UnityEditor.PlayerSettings;

namespace MMOJam.Vehicle
{
    public class RuntimeVehicle : AInteractible
    {
        [SerializeField]
        private VehicleInfo _info;

        private Rigidbody _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        public override void InteractClient(PlayerController player)
        {
            player.CurrentVehicle = new()
            {
                Vehicle = this,
                Seat = SeatType.Driver
            };
        }

        public override void InteractServer(PlayerController player)
        {
            player.InVehicle.Value = true;
        }

        public void Move(Vector2 mov)
        {
            Vector3 desiredMove = transform.forward * mov.y;// + transform.right * mov.x;
            _rb.AddForce(desiredMove);
            _rb.linearVelocity = _rb.linearVelocity.normalized * Mathf.Clamp(_rb.linearVelocity.magnitude, 0f, 20f);
        }
    }
}
