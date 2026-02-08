using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UIElements;

namespace MMOJam.Manager
{
    public class NetworkLink : MonoBehaviour
    {
        public static NetworkLink Instance { private set; get; }

        [SerializeField]
        private UIDocument _uiDocument;

        public static string IP { set; get; } = null;
        public static ushort? Port { set; get; } = null;

        private void Awake()
        {
            Instance = this;

            var nm = GetComponent<NetworkManager>();

#if UNITY_EDITOR
            IP ??= "localhost";
            Port ??= 7777;
#endif

            var transport = GetComponent<UnityTransport>();
            transport.ConnectionData.Address = IP;
            transport.ConnectionData.Port = Port.Value;

#if UNITY_EDITOR
            _uiDocument.rootVisualElement.Q<Button>("btn-start_host").clicked += () =>
            {
                nm.StartHost();
                _uiDocument.rootVisualElement.Q<GroupBox>("network-container").visible = false;
            };
            _uiDocument.rootVisualElement.Q<Button>("btn-start_client").clicked += () =>
            {
                nm.StartClient();
                _uiDocument.rootVisualElement.Q<GroupBox>("network-container").visible = false;
            };
#else
             _uiDocument.rootVisualElement.Q<GroupBox>("network-container").visible = false;
#endif
        }
    }
}
