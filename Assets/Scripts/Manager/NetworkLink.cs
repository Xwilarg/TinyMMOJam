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

        private void Awake()
        {
            Instance = this;

/*#if !UNITY_EDITOR
            _uiDocument.rootVisualElement.Q<Button>("btn-start_host").visible = false;
            _uiDocument.rootVisualElement.Q<Button>("btn-start_client").visible = false;
#endif*/
        }

        private void Start()
        {
            var nm = GetComponent<NetworkManager>();
            var transport = GetComponent<UnityTransport>();
#if UNITY_SERVER
            transport.ConnectionData.Port = 9761;
            nm.StartServer();
#else

            _uiDocument.rootVisualElement.Q<Button>("btn-start_host").clicked += () =>
            {
                transport.ConnectionData.Address = _uiDocument.rootVisualElement.Q<TextField>("field-ip").value;
                transport.ConnectionData.Port = (ushort)_uiDocument.rootVisualElement.Q<IntegerField>("field-port").value;
                nm.StartHost();
                _uiDocument.rootVisualElement.Q<GroupBox>("network-container").visible = false;
            _uiDocument.rootVisualElement.Q<VisualElement>("lang_menu").visible = false;
            };
            _uiDocument.rootVisualElement.Q<Button>("btn-start_client").clicked += () =>
            {
                transport.ConnectionData.Address = _uiDocument.rootVisualElement.Q<TextField>("field-ip").value;
                transport.ConnectionData.Port = (ushort)_uiDocument.rootVisualElement.Q<IntegerField>("field-port").value;
                nm.StartClient();
                _uiDocument.rootVisualElement.Q<GroupBox>("network-container").visible = false;
            _uiDocument.rootVisualElement.Q<VisualElement>("lang_menu").visible = false;
            };

            _uiDocument.rootVisualElement.Q<Button>("btn-join_dedicated").clicked += () =>
            {
#if UNITY_WEBGL
                transport.ConnectionData.Address = "gamejam.zirk.eu";
                transport.ConnectionData.Port = 443;
                transport.UseEncryption = true;
                transport.SetClientSecrets("gamejam.zirk.eu");
#else
                transport.ConnectionData.Address = "51.159.6.4";
                transport.ConnectionData.Port = 9761;
#endif
                nm.StartClient();
                _uiDocument.rootVisualElement.Q<GroupBox>("network-container").visible = false;
            _uiDocument.rootVisualElement.Q<VisualElement>("lang_menu").visible = false;
            };
#endif
        }
    }
}
