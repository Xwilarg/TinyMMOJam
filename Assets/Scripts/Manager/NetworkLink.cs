using Unity.Netcode;
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

            _uiDocument.rootVisualElement.Q<Button>("btn-start_host").clicked += () =>
            {
                GetComponent<NetworkManager>().StartHost();
                _uiDocument.rootVisualElement.Q<GroupBox>("network-container").visible = false;
            };
            _uiDocument.rootVisualElement.Q<Button>("btn-start_client").clicked += () =>
            {
                GetComponent<NetworkManager>().StartClient();
                _uiDocument.rootVisualElement.Q<GroupBox>("network-container").visible = false;
            };
        }
    }
}
