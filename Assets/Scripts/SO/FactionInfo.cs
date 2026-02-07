using UnityEngine;

namespace MMOJam.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/FactionInfo", fileName = "FactionInfo")]
    public class FactionInfo : ScriptableObject
    {
        // Must be unique and not zero
        public int Id;

        public string Name;
        public string Description;
        public Color Color;

        public Material Material;
    }
}
