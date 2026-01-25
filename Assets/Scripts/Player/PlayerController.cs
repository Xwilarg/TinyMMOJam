using MMOJam.Manager;
using Sketch.Common;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using Unity.Netcode;
using Unity.VisualScripting.Dependencies.NCalc;
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

        protected CharacterController _controller;
        private PlayerInput _pInput;
        private Camera _cam;

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
        }

        [Rpc(SendTo.Server)]
        public void InteractPlayerRpc()
        {
            if (_interactibles.Count > 0)
            {
                InteractWithRpc(_interactibles[0].Key);
            }
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void InteractWithRpc(ulong key)
        {
            ServerManager.Instance.InteractWith(key, this);
        }

        protected virtual void Update()
        {
            if (!_controller.enabled || !IsOwner)
                return;

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

        public void OnMovement(InputAction.CallbackContext value)
        {
            if (IsOwner && !IsAi) _mov = value.ReadValue<Vector2>();
        }

        public void OnShoot(InputAction.CallbackContext value)
        {
            if (IsOwner && !IsAi && value.phase == InputActionPhase.Started)
            {
                var mousePos = CursorUtils.GetPosition(_pInput);
                var ray = _cam.ScreenPointToRay(mousePos.Value);
                Debug.Log(mousePos);
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
                InteractPlayerRpc();
            }
        }
    }
}
