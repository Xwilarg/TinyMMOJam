using System.Collections;
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
        private Label _factionName, _factionSubtitle;
        private Label _ressource_amount;

        private void Awake()
        {
            Instance = this;

            _label = _ui.rootVisualElement.Q<Label>("interact_text");
            _label.visible = false;

            _factionName = _ui.rootVisualElement.Q<Label>("faction_text");
            _factionSubtitle = _ui.rootVisualElement.Q<Label>("subfaction_text");
            _factionName.visible = false;
            _factionSubtitle.visible = false;

            _ressource_amount = _ui.rootVisualElement.Q<Label>("ressource_1_amount");
        }

        public void ShowFactionName(int factionId)
        {
            var faction = ServerManager.Instance.GetFaction(factionId);

            _factionName.visible = true;
            _factionSubtitle.visible = true;

            _factionName.text = "You are in the " + faction.Name + " faction";
            _factionSubtitle.text = faction.Description;

            StartCoroutine(WaitAndHideFactionNameCoroutine());
        }

        private IEnumerator WaitAndHideFactionNameCoroutine()
        {
            yield return new WaitForSeconds(3f);
            _factionName.visible = false;
            _factionSubtitle.visible = false;
        }

        public void ShowInteractionText(bool value)
        {
            _label.visible = value;
        }

        public void UpdateRessources(long value)
        {
            _ressource_amount.text = "" + value;
        }
    }
}
