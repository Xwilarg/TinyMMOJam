using MMOJam.Player;
using MMOJam.SO;
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

        public bool SpawnAtFaction(int factionId, PlayerController player)
        {
            // This should work because SO are global instances? Unity docs better not have lied
            var zone = _zones.FirstOrDefault(z => z.Faction.Id == factionId);
            if (zone == null) return false;
            zone.SpawnPlayer(player);
            return true;
        }

        public FactionInfo GetWinningFaction()
        {
            var alives = _zones.GroupBy(x => x.Faction.Id);

            if (alives.Count() == 1) return alives.First().First().Faction;

            return null;
        }

        public ZoneController GetZoneFromTransform(Transform transform)
        {
            return _zones.FirstOrDefault(zone => zone.Contains(transform));
        }
    }
}
