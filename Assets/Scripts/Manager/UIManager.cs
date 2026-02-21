using MMOJam.Player;
using MMOJam.SO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

namespace MMOJam.Manager
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { private set; get; }

        [SerializeField]
        private UIDocument _ui;
        public UIDocument UI => _ui;
        [SerializeField]
        private FactionInfo[] _factions;
        public FactionInfo[] Factions => _factions;

        [SerializeField]
        private CinemachineCamera _cam;
        public CinemachineCamera Cam => _cam;

        private ListView _craftingList;
        private Label _label;
        private Label _factionName, _factionSubtitle;
        private Label _ressource_amount;
        private List<short> _craftRecipeIds = new();

        private Image _minimap;

        private float _psaTimer;

        private PlayerController _player;
        public PlayerController Player
        {
            set
            {
                _player = value;
                UpdateCraftingList();
            }
            private get => _player;
        }

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

            _minimap = _ui.rootVisualElement.Q<Image>("minimap");

            foreach (var text in _ui.rootVisualElement.Query<TextElement>().ToList())
            {

                var sentence = text.text;
                foreach (var match in Regex.Matches(text.text, "{([^}]+)}").Cast<Match>())
                {
                    sentence = sentence.Replace(match.Value, Sketch.Translation.Translate.Instance.Tr(match.Groups[1].Value));
                }
                text.text = sentence;
            }
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

                var recipe = CraftingManager.Instance.GetRecipe(recipeId);
                button.text = Sketch.Translation.Translate.Instance.Tr("craft_hint", Sketch.Translation.Translate.Instance.Tr(recipe.resultName));
                button.SetEnabled(Player != null && CraftingManager.Instance.CanCraft(Player, recipeId, out var _));
                button.clicked += () =>
                {
                    Debug.Log($"Craft recipe {recipe.resultName}");
                    var player = ServerManager.Instance.GetLocalPlayer();
                    ServerManager.Instance.RequestCraftServerRpc(player.NetworkObjectId, recipeId);
                };
            };
        }

        private void Update()
        {
            if (_psaTimer > 0f)
            {
                _psaTimer -= Time.deltaTime;
                if (_psaTimer < 0f)
                {
                    _factionName.visible = false;
                    _factionSubtitle.visible = false;
                }
            }
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void DispatchPsaRpc(string message, string subtitle)
        {
            _factionName.visible = true;
            _factionSubtitle.visible = true;

            _factionName.text = message;
            _factionSubtitle.text = subtitle;

            _psaTimer = 3f;
        }

        public void ToggleMinimap(bool value)
        {
            _minimap.visible = value;
        }

        public void ToggleCraftingList(bool value)
        {
            _craftingList.visible = value;
        }

        public void ShowFactionName(int factionId)
        {
            var faction = ServerManager.Instance.GetFaction(factionId);

            _factionName.visible = true;
            _factionSubtitle.visible = true;

            _factionName.text = Sketch.Translation.Translate.Instance.Tr("faction_assignation", Sketch.Translation.Translate.Instance.Tr(faction.Name));
            _factionSubtitle.text = Sketch.Translation.Translate.Instance.Tr(faction.Description);

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
            UpdateCraftingList();
        }

        public void UpdateCraftingList()
        {
            _craftingList.RefreshItems();
        }
    }
}
