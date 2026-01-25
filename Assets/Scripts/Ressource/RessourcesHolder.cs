using Unity.Netcode;
using UnityEngine;
using MMOJam.Manager;
using System.Collections.Generic;

namespace MMOJam
{
    public class RessourcesHolder : NetworkBehaviour
    {
        private Dictionary<short, long> _ressources = new();

        public void ChangeRessources(short id, long amount)
        {
            if (!ServerManager.Instance.IsAuthority) return;

            _ressources.TryGetValue(id, out long current);

            long total = current + amount;
            if (total < 0)
                total = 0;

            _ressources[id] = total;
            Debug.Log($"[RGT] [{string.Join(", ", _ressources.Values)}]");
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            RessourcesManager.Instance.RegisterHolder(this);
        }
    }
}
