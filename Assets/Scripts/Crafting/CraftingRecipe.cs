using UnityEngine;
using System.Collections.Generic;

namespace MMOJam
{
    [CreateAssetMenu(menuName = "ScriptableObject/Recipe")]
    public class CraftingRecipe : ScriptableObject
    {
        public short recipeId;

        public List<ResourceAmount> inputs;
        public List<ResourceAmount> outputs;
        public GameObject crafted;
    }

    [System.Serializable]
    public struct ResourceAmount
    {
        public short resourceId;
        public long amount;
    }
}
