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
            UIManager.Instance.UpdateCraftingList();
            Debug.Log($"[RGT] [{string.Join(", ", _ressources.Values)}]");
            SendRessourceClientRpc(id, total);
        }

        public bool CheckRessources(short id, long amount)
        {
            _ressources.TryGetValue(id, out long current);
            if (current < amount) return false;
            return true;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            RessourcesManager.Instance.RegisterHolder(this);
        }

        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
        public void RequestRessourceServerRpc(short id)
        {
            // Server-only
            _ressources.TryGetValue(id, out long value);

            // Send ONLY to the owning client
            SendRessourceClientRpc(id, value);
        }



        [Rpc(SendTo.Owner)]
        private void SendRessourceClientRpc(short id, long value)
        {
            _ressources[id] = value;
            Debug.Log($"[RCN] Received resource {id}: {value}");
            UIManager.Instance.UpdateRessources(value);
            UIManager.Instance.UpdateCraftingList();
        }
    }
}
