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

        private void Awake()
        {
            Instance = this;

            _label = _ui.rootVisualElement.Q<Label>("interact_text");
            _label.visible = false;

            _factionName = _ui.rootVisualElement.Q<Label>("faction_text");
            _factionSubtitle = _ui.rootVisualElement.Q<Label>("subfaction_text");
            _factionName.visible = false;
            _factionSubtitle.visible = false;
        }

        public void ShowFactionName(int factionId)
        {
            _factionName.visible = true;
            _factionSubtitle.visible = true;

            _factionName.text = factionId == 1 ? "You are in the MARC faction" : "You are in the Überwurst faction";
            _factionSubtitle.text = factionId == 1 ? "Massively Anarchist Revolutionary Congregate, basically the bad guys" : "Please Indra this is a placeholder name, think of THE USER, replace this";

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
    }
}
