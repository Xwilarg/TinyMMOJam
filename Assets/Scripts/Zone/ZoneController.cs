using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.Zone
{
    public class ZoneController : NetworkBehaviour
    {
        [SerializeField]
        private string _name;
    }
}
