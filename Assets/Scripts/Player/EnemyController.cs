using MMOJam.Manager;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace MMOJam.Player
{
    public class EnemyController : PlayerController
    {
        private List<PlayerController> _targets = new();
        private float _timerRefreshPath = 0f;

        private NavMeshPath _path;
        private int _pathIndex;

        protected override bool UseRelativeMov => false;

        protected override void Awake()
        {
            base.Awake();

            _path = new();
        }

        protected override void OnTriggerEnter(Collider other)
        {
            if (!ServerManager.Instance.IsAuthority) return;

            if (other.CompareTag("Player")) _targets.Add(other.GetComponent<PlayerController>());
        }

        private void OnTriggerExit(Collider other)
        {
            if (!ServerManager.Instance.IsAuthority) return;

            if (other.CompareTag("Player"))
            {
                var player = other.GetComponent<PlayerController>();
                _targets.RemoveAll(x => x.GetEntityId() == player.GetEntityId());
            }
        }

        protected override void Update()
        {
            base.Update();

            if (!ServerManager.Instance.IsAuthority) return;

            if (_targets.Count == 0)
            {
                _mov = Vector2.zero;
                return;
            }

            if (_pathIndex < _path.corners.Length)
            {
                if (Vector3.Distance(transform.position, _path.corners[_pathIndex]) < .2f)
                {
                    _pathIndex++;
                }
                if (_pathIndex < _path.corners.Length)
                {
                    var dir = (_path.corners[_pathIndex] - transform.position).normalized;
                    _mov = new Vector2(dir.x, dir.z);
                }
            }

            if (_timerRefreshPath > 0f)
            {
                _timerRefreshPath -= Time.deltaTime;
            }
            else
            {
                NavMesh.CalculatePath(transform.position, _targets[0].transform.position, NavMesh.AllAreas, _path);
                _pathIndex = 0;

                _timerRefreshPath = 1f;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            if (_path != null)
            {
                for (int i = 1; i < _path.corners.Length; i++)
                {
                    Gizmos.DrawLine(_path.corners[i - 1], _path.corners[i]);
                }
            }
        }
    }
}
