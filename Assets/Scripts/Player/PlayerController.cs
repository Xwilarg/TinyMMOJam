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

        private CharacterController _controller;
        private Vector2 _mov;
        private float _verticalSpeed;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
        }

        private void Start()
        {
            if (IsOwner)
            {
                FindFirstObjectByType<CinemachineCamera>().Target.TrackingTarget = transform;
            }
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
            if (IsOwner) _mov = value.ReadValue<Vector2>();
        }
    }
}
