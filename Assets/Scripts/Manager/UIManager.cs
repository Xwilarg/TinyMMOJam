using UnityEngine;
using UnityEngine.UIElements;

namespace MMOJam.Manager
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { private set; get; }

        [SerializeField]
        private UIDocument _ui;

        private Label _label;

        private void Awake()
        {
            Instance = this;

            _label = _ui.rootVisualElement.Q<Label>("interact_text");
            _label.visible = false;
        }

        public void ShowInteractionText(bool value)
        {
            _label.visible = value;
        }
    }
}
