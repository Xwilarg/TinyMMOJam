using MMOJam.Player;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MMOJam.Zone
{
    public class ZoneManager : MonoBehaviour
    {
        public static ZoneManager Instance { private set; get; }

        private readonly List<ZoneController> _zones = new();

        private void Awake()
        {
            Instance = this;
        }

        public void RegisterZone(ZoneController zone)
        {
            _zones.Add(zone);
        }

        public void SpawnAtFaction(int factionId, PlayerController player)
        {
            // This should work because SO are global instances? Unity docs better not have lied
            _zones.First(z => z.Faction.Id == factionId).SpawnPlayer(player);
        }

        public ZoneController GetZoneFromTransform(Transform transform)
        {
            return _zones.FirstOrDefault(zone => zone.Contains(transform));
        }
    }
}
