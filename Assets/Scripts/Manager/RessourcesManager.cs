using Unity.Netcode;
using UnityEngine.Networking;
using UnityEngine;

namespace MMOJam.Manager
{
    public class RessourcesManager : NetworkBehaviour
    {
        public static ServerManager Instance { private set; get; }
        public void OnStartServer()
        {

        }
    }
}