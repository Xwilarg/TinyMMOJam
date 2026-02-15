using MMOJam.Player;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace MMOJam.Manager
{
    public class CraftingManager : MonoBehaviour
    {
        [SerializeField]
        private List<CraftingRecipe> _existing = new();
        private Dictionary<short, CraftingRecipe> _recipes = new();
        public static CraftingManager Instance { private set; get; }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            foreach (var recipe in _existing)
            {
                RegisterRecipe(recipe);
            }
        }

        public void RegisterRecipe(CraftingRecipe recipe)
        {
            _recipes[recipe.recipeId] = recipe;
            UIManager.Instance.UpdateCraft(recipe.recipeId);
        }

        public CraftingRecipe GetRecipe(short recipeId)
        {
            return _recipes[recipeId];
        }

        public bool TryCraft(PlayerController player, short recipeId)
        {
            RessourcesHolder holder = player.GetComponent<RessourcesHolder>();

            if (!_recipes.TryGetValue(recipeId, out var recipe))
                return false;

            foreach (var res in recipe.inputs)
            {
                if (!holder.CheckRessources(res.resourceId, res.amount))
                    return false;
            }
            
            // Consume
            foreach (var input in recipe.inputs)
                holder.ChangeRessources(input.resourceId, -input.amount);

            // Produce
            foreach (var output in recipe.outputs)
                holder.ChangeRessources(output.resourceId, output.amount);

            SpawnCraft(recipe.crafted, player.transform.position);

            return true;
        }
        public void SpawnCraft(GameObject to_spawn, Vector3 pos)
        {
            if (!NetworkManager.Singleton.IsServer)
                return;

            var go = Instantiate(to_spawn);
            var p = Random.insideUnitCircle.normalized * 3f;
            go.transform.position = pos + new Vector3(p.x, 0f, p.y);

            var netObj = go.GetComponent<NetworkObject>();
            netObj.Spawn();
        }

        public void CraftRecipe(ulong PlayerNetworkId, short recipeId)
        {
            var player = ServerManager.Instance.GetNetworkPlayer(PlayerNetworkId);
            if (player == null)
            {
                Debug.Log($"[CMC] Player is null");
                return;
            }

            var holder = player._ressource_controller;
            if (holder == null)
            {
                Debug.Log($"[CMC] Holder is null");
                return;
            }

            bool success = CraftingManager.Instance.TryCraft(player, recipeId);
            
            Debug.Log($"[CMC] craft is {success}");
        }

    }
}