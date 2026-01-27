using MMOJam.Manager;
using MMOJam.Player;
using MMOJam.SO;
using System.Collections.Generic;
using System.Linq;
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
            var players = ServerManager.Instance.GetPlayersInVehicle(Key).ToList();

            for (int i = 0; i < _info.Seats.Length; i++)
            {
                var match = players.FirstOrDefault(x => x.CurrentSeat.Value == _info.Seats[i]);
                if (match != null)
                {
                    players.Remove(match);
                }
                else
                {
                    Debug.Log($"[VHC] Entered {_info.Name} as {_info.Seats[i]}");
                    player.SetVehicle(this, _info.Seats[i]);
                    break;
                }
            }
        }

        public IEnumerable<PlayerController> GetPlayersInVehicle()
        {
            return ServerManager.Instance.GetPlayersInVehicle(Key);
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
