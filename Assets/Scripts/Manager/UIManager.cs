using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace MMOJam.Manager
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { private set; get; }

        [SerializeField]
        private UIDocument _ui;

        private ListView _craftingList;
        private Label _label;
        private Label _factionName, _factionSubtitle;
        private Label _ressource_amount;
        private List<short> _craftRecipeIds = new();


        private void Awake()
        {
            Instance = this;

            _label = _ui.rootVisualElement.Q<Label>("interact_text");
            _label.visible = false;

            _factionName = _ui.rootVisualElement.Q<Label>("faction_text");
            _factionSubtitle = _ui.rootVisualElement.Q<Label>("subfaction_text");
            _factionName.visible = false;
            _factionSubtitle.visible = false;

            _craftingList = _ui.rootVisualElement.Q<ListView>("crafting_list_ui");
            _ressource_amount = _ui.rootVisualElement.Q<Label>("ressource_1_amount");
        }

        private void Start()
        {
            _craftingList.itemsSource = _craftRecipeIds;

            _craftingList.makeItem = () =>
            {
                var button = new Button();
                button.AddToClassList("craft-button");
                return button;
            };

            _craftingList.bindItem = (element, index) =>
            {
                var button = (Button)element;
                short recipeId = _craftRecipeIds[index];

                button.text = $"Craft recipe {recipeId}";
                button.clicked += () =>
                {
                    Debug.Log($"Craft recipe {recipeId}");
                    var player = ServerManager.Instance.GetLocalPlayer();
                    CraftingManager.Instance.RequestCraftServerRpc(player.NetworkObjectId, recipeId);
                };
            };
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

        public void UpdateCraft(short id)
        {
            if (_craftRecipeIds.Contains(id))
                return;

            _craftRecipeIds.Add(id);
            _craftingList.RefreshItems();
        }



    }
}
