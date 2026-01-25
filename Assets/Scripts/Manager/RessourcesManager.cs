using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

namespace MMOJam.Manager
{
    public class RessourcesManager : NetworkBehaviour
    {
        public static RessourcesManager Instance { private set; get; }
        private Dictionary<ulong, RessourcesHolder> _ressources_holders = new();
        public void RegisterHolder(RessourcesHolder elem)
        {
            _ressources_holders.Add(elem.GetComponent<NetworkObject>().NetworkObjectId, elem);
        }
    }
}