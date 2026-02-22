using MMOJam.Manager;
using MMOJam.SO;
using MMOJam.Vehicle;
using MMOJam.Zone;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

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

        [SerializeField]
        private LineRenderer _lr;

        [SerializeField]
        private GameObject _model;

        private RuntimeVehicle _currentVehicleObject;
        public NetworkVariable<ulong> CurrentVehicle { private set; get; } = new(0);
        public NetworkVariable<SeatType> CurrentSeat { private set; get; } = new((SeatType)(-1));
        public NetworkVariable<NetworkObjectReference> CurrentZone { private set; get; } = new();
        public NetworkVariable<int> CurrentFaction { private set; get; } = new(0);

        protected CharacterController _controller;
        private PlayerInput _pInput;
        public RessourcesHolder _ressource_controller;
        private Camera _cam;
        private Collider _coll;
        private LivingEntity _livingEntity;
        private SkinnedMeshRenderer _mr;
        private Rigidbody _rb;

        protected Vector2 _mov;
        private float _verticalSpeed;

        private float _shootTimer = -1f;
        private bool _isAttacking;

        public bool IsAi { set; get; }

        private readonly List<AInteractible> _interactibles = new();

        public bool IsAlive => _livingEntity.IsAlive;

        protected virtual bool UseRelativeMov => true;

        private Animator _anim;

        // Server only

        protected virtual void Awake()
        {
            _livingEntity = GetComponent<LivingEntity>();
            _controller = GetComponent<CharacterController>();
            _ressource_controller = GetComponent<RessourcesHolder>();
            _pInput = GetComponentInChildren<PlayerInput>();
            _cam = Camera.main;
            _coll = GetComponent<Collider>();
            _mr = GetComponentInChildren<SkinnedMeshRenderer>();
            _rb = GetComponent<Rigidbody>();
            _anim = GetComponentInChildren<Animator>();

            _lr.gameObject.SetActive(false);

            CurrentVehicle.OnValueChanged += (oldValue, newValue) =>
            {
                _renderer.SetActive(newValue == 0);
                _coll.enabled = newValue == 0;
                _controller.enabled = newValue == 0;

                _currentVehicleObject = newValue == 0 ? null : ServerManager.Instance.GetVehicle(newValue);
                if (_currentVehicleObject != null && IsLocalPlayer)
                {
                    transform.position = _currentVehicleObject.transform.position;
                }
                transform.rotation = Quaternion.identity;
            };

            if (IsLocalHuman)
            {
                CurrentZone.OnValueChanged += (oldValue, newValue) =>
                {
                    if (newValue.TryGet(out var networkObject))
                    {
                        var zone = networkObject.GetComponent<ZoneController>();

                        Debug.Log("[ZNE] Now entering " + zone.name);
                    }
                    else
                    {
                        Debug.Log("[ZNE] Now exiting");
                    }
                };
            }

            CurrentFaction.OnValueChanged += (oldValue, newValue) =>
            {
                ShowFactionData(newValue);
            };
        }

        private void ShowFactionData(int newValue)
        {
            if (IsLocalPlayer) UIManager.Instance.ShowFactionName(CurrentFaction.Value);
            if (!IsAi)
            {
                var mats = _mr.materials;
                mats[0] = ServerManager.Instance.GetFaction(newValue).Material; // not properly sync
                _mr.materials = mats;
            }
        }

        public bool IsLocalHuman => IsOwner && !IsAi;

        public void SetupServer()
        {
            Debug.Log($"[PLY] SetupServer called for {NetworkObjectId}");

            var faction = ServerManager.Instance.GetNextFaction();

            CurrentFaction.Value = faction;

            MoveToSpawnpoint();
        }

        private void ClientMoveToSpawnPoint()
        {
            _controller.enabled = false;
            ZoneManager.Instance.SpawnAtFaction(CurrentFaction.Value, this);
            _controller.enabled = true;
        }

        public void MoveToSpawnpoint()
        {
            Debug.Log($"[PLY] Moving to spawn point");

            _controller.enabled = false;
            if (ZoneManager.Instance.SpawnAtFaction(CurrentFaction.Value, this))
            {
                Debug.Log("[PLY] Succesfully respawned");
                _anim.SetTrigger("Revive");
                //_rb.isKinematic = true;
                //transform.rotation = Quaternion.identity;
                _livingEntity.RestoreHealth();
            }
            _controller.enabled = true;
        }

        public void MoveTo(Vector3 pos)
        {
            Debug.Log($"[PLA] Player teleported to {pos}");
            transform.position = pos;
        }

        public void TryRespawn()
        {
            StartCoroutine(WaitAndRespawn());
        }

        private IEnumerator WaitAndRespawn()
        {
            yield return new WaitForSeconds(3f);
            if (!IsAlive) MoveToSpawnpoint();
        }

        [Rpc(SendTo.Owner)]
        public void MoveToSpawnPointRpc()
        {
            TryRespawn();
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void DieRpc()
        {
            _anim.SetTrigger("Die");
        }

        public void Die()
        {
            DieRpc();
            /*_rb.isKinematic = false;
            _rb.AddTorque(new Vector3(Random.value, Random.value, Random.value).normalized * 10f);*/

            if (CurrentVehicle.Value != 0) // TODO: Is this done on all clients?
            {
                SetVehicle(null, (SeatType)(-1));
                LeaveVehicleRpc();
            }
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsLocalHuman)
            {
                FindFirstObjectByType<CinemachineCamera>().Target.TrackingTarget = transform;
                UIManager.Instance.Player = this;
                gameObject.AddComponent<AudioListener>();
            }

            if (IsHost || IsServer || IsLocalHuman)
            {
                var tArea = GetComponentInChildren<TriggerArea>();
                tArea.OnTriggerEnterEvent.AddListener((c) =>
                {
                    UnregisterInteractible();
                    if (c.TryGetComponent<AInteractible>(out var cComp) && !_interactibles.Any(x => x.gameObject.GetEntityId() == c.gameObject.GetEntityId()))
                    {
                        _interactibles.Add(cComp);
                        if (IsLocalHuman)
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
                        if (_interactibles.Count == 0 && IsLocalHuman)
                        {
                            UIManager.Instance.ShowInteractionText(false);
                        }
                    }
                });
            }

            if (!IsAi)
            {
                ServerManager.Instance.RegisterPlayer(this);

                if (IsServer)
                {
                    SetupServer();
                    StartCoroutine(WaitAndKill());
                }
                else
                {
                    ShowFactionData(CurrentFaction.Value);
                    ClientMoveToSpawnPoint();
                }
            }
        }

        private IEnumerator WaitAndKill()
        {
            yield return new WaitForEndOfFrame();
            _livingEntity.TakeDamage(null, 999);
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            ServerManager.Instance.UnregisterPlayer(this);
        }

        public void UnregisterInteractible()
        {
            _interactibles.RemoveAll(x => x == null);
        }

        [Rpc(SendTo.Server)]
        public void SetVehicleServerRpc(ulong vehicle, SeatType seat)
        {
            CurrentVehicle.Value = vehicle;
            CurrentSeat.Value = seat;
        }

        public void SetVehicle(RuntimeVehicle vehicle, SeatType seat)
        {
            SetVehicleServerRpc(vehicle == null ? 0 : vehicle.Key, seat);

            transform.parent = vehicle == null ? null : vehicle.transform;
            if (vehicle != null)
            {
                transform.position = vehicle.transform.position;
            }
        }

        public void SetZone(ZoneController zone)
        {
            CurrentZone.Value = zone != null ? zone.gameObject : null;
        }

        [Rpc(SendTo.Server)]
        public void InteractPlayerRpc()
        {
            UnregisterInteractible();
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
            UnregisterInteractible();
        }

        [Rpc(SendTo.Server)]
        public void LeaveVehicleRpc()
        {
            CurrentVehicle.Value = 0;
            CurrentSeat.Value = (SeatType)(-1);
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (ServerManager.Instance.IsAuthority && other.CompareTag("Trap"))
            {
                _livingEntity.TakeDamage(ServerManager.Instance.GetFaction(CurrentFaction.Value), 999);
            }
        }

        private float _vehicleMoveTick = 0f;
        private Vector2 _lastVehiculeDir;
        protected virtual void Update()
        {
            if (!IsOwner)
                return;

            //Debug.Log($"[PLA] IsAlive? {IsAlive}");

            if (_shootTimer > 0f) _shootTimer -= Time.deltaTime;

            if (IsLocalHuman && IsAlive && _isAttacking && _shootTimer <= 0f)
            {
                var mousePos = Mouse.current.position.ReadValue();// CursorUtils.GetPosition(_pInput);
                if (mousePos != null)
                {
                    var ray = _cam.ScreenPointToRay(mousePos);
                    ShootServerRpc(ray.origin, ray.direction);
                }
                else Debug.LogWarning("Mouse position not found");
                _shootTimer = .2f;
            }

            if (CurrentVehicle.Value != 0 && CurrentSeat.Value == SeatType.Driver)
            {
                if (_vehicleMoveTick > 0f && _lastVehiculeDir == _mov) _vehicleMoveTick -= Time.deltaTime;
                else
                {
                    _currentVehicleObject.Move(_mov);
                    _vehicleMoveTick = .25f;
                    _lastVehiculeDir = _mov;
                }
            }
            else
            {
                var pos = _mov;
                Vector3 desiredMove = IsAlive ?
                    (UseRelativeMov ? (transform.forward * pos.y + transform.right * pos.x) : new Vector3(pos.x, 0f, pos.y))
                    : Vector3.zero;

                _anim.SetBool("IsWalking", desiredMove.magnitude > 0f);

                if (desiredMove.magnitude > 0f) _model.transform.rotation = Quaternion.LookRotation(desiredMove, Vector3.up);

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
            if (IsLocalHuman) _mov = value.ReadValue<Vector2>();
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void ShootClientVfxRpc(Vector3 dest)
        {
            var source = new Vector3(transform.position.x, 1f, transform.position.z);
            _lr.gameObject.SetActive(true);
            _lr.SetPositions(new Vector3[]
            {
                source,
                dest
            });
            StartCoroutine(ShootVfx());
        }

        private IEnumerator ShootVfx()
        {
            yield return new WaitForSeconds(.2f);
            _lr.gameObject.SetActive(false);
        }

        public void TakeDamage(int amount)
        {
            _livingEntity.TakeDamage(null, amount);
        }

        [Rpc(SendTo.Server)]
        public void ShootServerRpc(Vector3 rayStartPoint, Vector3 rayDir)
        {
            //if (!IsAlive) return; // Pain

            var ray = new Ray(rayStartPoint, rayDir);
            Debug.Log($"[PLA] {GetEntityId()} is trying to shoot");
            if (CurrentVehicle.Value == 0)
            {
                // Raycast against world to know where the mouse points
                if (Physics.Raycast(ray, out var hitInfo, 500f, LayerMask.GetMask("World")))
                {
                    Debug.Log($"[PLA] {GetEntityId()} is shooting toward {hitInfo.point}");
                    Vector3 spawnPos = transform.position + Vector3.up * 1f;

                    // Direction = from player to hit point
                    Vector3 direction = (hitInfo.point - spawnPos);
                    direction.y = 0f; // keep projectile horizontal
                    direction.Normalize();

                    ProjectileManager.Instance.SpawnProjectile(
                        projectileId: 0,
                        position: spawnPos + direction,
                        direction: direction,
                        factionId: CurrentFaction.Value
                    );
                }
            }
            else if (CurrentSeat.Value == SeatType.Shooter && _currentVehicleObject.ProjectileData != null) // We are in a turret that can shoot!
            {
                // Raycast against world to know where the mouse points
                if (Physics.Raycast(ray, out var hitInfo, 500f, LayerMask.GetMask("World")))
                {
                    Vector3 spawnPos = transform.position + Vector3.up * 1f;

                    // Direction = from player to hit point
                    Vector3 direction = (hitInfo.point - spawnPos);
                    direction.y = 0f; // keep projectile horizontal
                    direction.Normalize();

                    ProjectileManager.Instance.SpawnProjectile(
                        projectileId: 1,
                        position: spawnPos + direction * 5f,
                        direction: direction,
                        factionId: CurrentFaction.Value
                    );
                }
            }
        }

        public void OnShoot(InputAction.CallbackContext value)
        {
            if (value.phase == InputActionPhase.Started) _isAttacking = true;
            else if (value.phase == InputActionPhase.Canceled) _isAttacking = false;
        }

        public void OnInteract(InputAction.CallbackContext value)
        {
            if (IsLocalHuman && IsAlive && _interactibles.Count > 0 && value.phase == InputActionPhase.Started)
            {
                if (CurrentVehicle.Value != 0)
                {
                    var p = Random.insideUnitCircle.normalized * 2f;
                    transform.position = transform.position + new Vector3(p.x, 0f, p.y);
                    SetVehicle(null, (SeatType)(-1));
                    LeaveVehicleRpc();
                }
                else
                {
                    InteractPlayerRpc();
                    _vehicleMoveTick = 0f;
                }
            }
        }
    }
}
