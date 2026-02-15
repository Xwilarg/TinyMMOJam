using MMOJam.Manager;
using MMOJam.SO;
using MMOJam.Vehicle;
using MMOJam.Zone;
using Sketch.Common;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MMOJam.Player
{
    public class EnemyController : PlayerController
    {
        private List<PlayerController> _targets = new();

        protected override void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player")) _targets.Add(other.GetComponent<PlayerController>());
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                var player = other.GetComponent<PlayerController>();
                _targets.RemoveAll(x => x.GetEntityId() == player.GetEntityId());
            }
        }
    }
}
