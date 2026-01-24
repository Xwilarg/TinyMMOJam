using UnityEngine;

namespace MMOJam.Manager
{
    public class NetworkManager : MonoBehaviour
    {
        public static NetworkManager Instance { private set; get; }

        private void Awake()
        {
            Instance = this;
        }
    }
}
