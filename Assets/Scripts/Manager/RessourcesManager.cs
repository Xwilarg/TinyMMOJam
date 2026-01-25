using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

namespace MMOJam.Manager
{
    public class RessourcesManager : NetworkBehaviour
    {
        public static RessourcesManager Instance { private set; get; }
        private Dictionary<ulong, RessourcesHolder> _ressources_holders = new();

        private void Awake()
        {
            Instance = this;
        }
        public void RegisterHolder(RessourcesHolder elem)
        {
            _ressources_holders.Add(elem.GetComponent<NetworkObject>().NetworkObjectId, elem);
            Debug.Log($"[RHL] added {elem.GetComponent<NetworkObject>().NetworkObjectId} total [{string.Join(", ", _ressources_holders.Keys)}]");
        }

        public void DeleteHolder(ulong id)
        {
            if (_ressources_holders.Remove(id))
            {
                Debug.Log($"[RHL] deleted {id} total [{string.Join(", ", _ressources_holders.Keys)}]");
            }
            else
            {
                Debug.LogWarning($"[RHL] tried to delete {id} but it was not found");
            }
        }
    }
}