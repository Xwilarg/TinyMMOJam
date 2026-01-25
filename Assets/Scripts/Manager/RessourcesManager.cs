using Unity.Netcode;

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