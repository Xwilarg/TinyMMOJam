using MMOJam.Manager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace MMOJam.Menu
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField]
        private UIDocument _ui;

        private void Awake()
        {
            _ui.rootVisualElement.Q<Button>("dedicated-server").clicked += () =>
            {
                NetworkLink.IP = "51.159.6.4";
                NetworkLink.Port = 9761;
                SceneManager.LoadScene("Main");
            };
        }
    }
}
