using MMOJam.Manager;
using MMOJam.SO;
using MMOJam.Vehicle;
using MMOJam.Zone;
using Sketch.Common;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MMOJam.Player
{
    public class PlayerController : NetworkBehaviour
    {
        [Header("Configuration")]
        [SerializeField]
        private float _mouvementSpeed = 5f;

        [SerializeField]
        private float _gravityMultiplier = .75f;

        [SerializeField]
        private GameObject _renderer;

        private RuntimeVehicle _currentVehicleObject;
        public NetworkVariable<ulong> CurrentVehicle { private set; get; } = new(0);
        public NetworkVariable<SeatType> CurrentSeat { private set; get; } = new((SeatType)(-1));
        public ZoneController CurrentZone { get; private set; }

        protected CharacterController _controller;
        private PlayerInput _pInput;
        private Camera _cam;
        private Collider _coll;

        private Vector2 _mov;
        private float _verticalSpeed;

        public bool IsAi { set; get; }

        private readonly List<AInteractible> _interactibles = new();

        // Server only

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _pInput = GetComponentInChildren<PlayerInput>();
            _cam = Camera.main;
            _coll = GetComponent<Collider>();

            CurrentVehicle.OnValueChanged += (oldValue, newValue) =>
            {
                _renderer.SetActive(newValue == 0);
                _coll.enabled = newValue == 0;
                _controller.enabled = newValue == 0;

                _currentVehicleObject = newValue == 0 ? null : ServerManager.Instance.GetVehicle(newValue);
                transform.rotation = Quaternion.identity;
            };
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsOwner && !IsAi)
            {
                FindFirstObjectByType<CinemachineCamera>().Target.TrackingTarget = transform;
            }

            if (IsHost || IsServer || (IsOwner && !IsAi))
            {
                var tArea = GetComponentInChildren<TriggerArea>();
                tArea.OnTriggerEnterEvent.AddListener((c) =>
                {
                    if (c.TryGetComponent<AInteractible>(out var cComp) && !_interactibles.Any(x => x.gameObject.GetEntityId() == c.gameObject.GetEntityId()))
                    {
                        _interactibles.Add(cComp);
                        if (IsOwner && !IsAi)
                        {
                            UIManager.Instance.ShowInteractionText(true);
                        }
                    }
                });
                tArea.OnTriggerExitEvent.AddListener((c) =>
                {
                    if (c.TryGetComponent<AInteractible>(out var cComp))
                    {
                        _interactibles.RemoveAll(x => x.gameObject.GetEntityId() == c.gameObject.GetEntityId());
                        if (_interactibles.Count == 0 && IsOwner && !IsAi)
                        {
                            UIManager.Instance.ShowInteractionText(false);
                        }
                    }
                });
            }

            ServerManager.Instance.RegisterPlayer(this);
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            ServerManager.Instance.UnregisterPlayer(this);
        }

        public void SetVehicle(RuntimeVehicle vehicle, SeatType seat)
        {
            CurrentVehicle.Value = vehicle == null ? 0 : vehicle.Key;
            CurrentSeat.Value = seat;

            transform.parent = vehicle == null ? null : vehicle.transform;
            if (vehicle != null)
            {
                transform.position = vehicle.transform.position;
            }
        }

        public void SetZone(ZoneController zone)
        {
            CurrentZone = zone;

            Debug.Log("Now entering " + zone.Name);
        }

        [Rpc(SendTo.Server)]
        public void InteractPlayerRpc()
        {
            if (_interactibles.Count > 0)
            {
                _interactibles[0].InteractServer(this);
                InteractWithRpc(_interactibles[0].Key);
            }
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void InteractWithRpc(ulong key)
        {
            ServerManager.Instance.InteractWith(key, this);
        }

        [Rpc(SendTo.Server)]
        public void LeaveVehicleRpc()
        {
            CurrentVehicle.Value = 0;
            CurrentSeat.Value = (SeatType)(-1);
        }

        protected virtual void Update()
        {
            if (!IsOwner)
                return;

            if (CurrentVehicle.Value != 0 && CurrentSeat.Value == SeatType.Driver)
            {
                _currentVehicleObject.Move(_mov);
            }
            else
            {
                var pos = _mov;
                Vector3 desiredMove = transform.forward * pos.y + transform.right * pos.x;

                // Get a normal for the surface that is being touched to move along it
                Physics.SphereCast(transform.position, _controller.radius, Vector3.down, out RaycastHit hitInfo,
                                   _controller.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
                desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

                Vector3 moveDir = Vector3.zero;
                moveDir.x = desiredMove.x * _mouvementSpeed;
                moveDir.z = desiredMove.z * _mouvementSpeed;

                if (_controller.isGrounded && _verticalSpeed < 0f) // We are on the ground and not jumping
                {
                    moveDir.y = -.1f; // Stick to the ground
                    _verticalSpeed = -_gravityMultiplier;
                }
                else
                {
                    // We are currently jumping, reduce our jump velocity by gravity and apply it
                    _verticalSpeed += Physics.gravity.y * _gravityMultiplier * Time.deltaTime;
                    moveDir.y += _verticalSpeed;
                }

                _controller.Move(moveDir * Time.deltaTime);
            }
        }

        public void OnMovement(InputAction.CallbackContext value)
        {
            if (IsOwner && !IsAi) _mov = value.ReadValue<Vector2>();
        }

        public void OnShoot(InputAction.CallbackContext value)
        {
            if (IsOwner && !IsAi && value.phase == InputActionPhase.Started && CurrentVehicle.Value == 0)
            {
                var mousePos = CursorUtils.GetPosition(_pInput);
                var ray = _cam.ScreenPointToRay(mousePos.Value);
                if (Physics.Raycast(ray, out var hitInfo, float.MaxValue, LayerMask.GetMask("World")))
                {
                    var dir = (hitInfo.point - transform.position).normalized;
                    dir.y = 0f;
                    Debug.DrawLine(transform.position, transform.position + (dir * 5f), Color.red, 2f);
                }
            }
        }

        public void OnInteract(InputAction.CallbackContext value)
        {
            if (IsOwner && !IsAi && _interactibles.Count > 0 && value.phase == InputActionPhase.Started)
            {
                if (CurrentVehicle.Value != 0)
                {
                    SetVehicle(null, (SeatType)(-1));
                    LeaveVehicleRpc();
                }
                else
                {
                    InteractPlayerRpc();
                }
            }
        }
    }
}
