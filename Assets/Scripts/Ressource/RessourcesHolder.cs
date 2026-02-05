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

        public void CheckRessources(short id, long amount)
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

        [ServerRpc]
        public void RequestRessourceServerRpc(short id, ServerRpcParams rpcParams = default)
        {
            // Runs only on the server
            _ressources.TryGetValue(id, out long value);

            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new[] { rpcParams.Receive.SenderClientId }
                }
            };

            // Send back ONLY to the requesting client
            SendRessourceClientRpc(id, value, clientRpcParams);
        }

        [ClientRpc]
        private void SendRessourceClientRpc(
            short id,
            long value,
            ClientRpcParams clientRpcParams = default)
        {
            _ressources[id] = value;
            // Runs on the client that requested it
            Debug.Log($"[RCN] Received resource {id}: {value}");

            // You can cache it locally, update UI, etc.
        }
    }
}
