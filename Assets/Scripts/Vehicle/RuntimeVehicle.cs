using MMOJam.Player;
using MMOJam.SO;
using Unity.Netcode;
using UnityEngine;

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
        }

        public override void InteractServer(PlayerController player)
        {
            player.SetVehicle(this, SeatType.Driver);
        }

        public void Move(Vector2 mov)
        {
            MoveServerRpc(mov);
        }

        [Rpc(SendTo.Server)]
        public void MoveServerRpc(Vector2 mov)
        {
            if (mov.y < 0f)
            {
                mov.x = -mov.x;
            }
            Vector3 desiredMove = transform.forward * mov.y * Time.deltaTime * 5_000f;// + transform.right * mov.x;
            _rb.AddForce(desiredMove);
            _rb.linearVelocity = _rb.linearVelocity.normalized * Mathf.Clamp(_rb.linearVelocity.magnitude, 0f, 10f);

            transform.Rotate(Vector3.up, mov.x * _rb.linearVelocity.magnitude * Time.deltaTime * 10f);
        }
    }
}
