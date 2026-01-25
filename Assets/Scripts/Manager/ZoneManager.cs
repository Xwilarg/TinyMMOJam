using Assets.Scripts.Zone;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class ZoneManager : NetworkBehaviour
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

    public ZoneController GetZoneFromTransform(Transform transform)
    {
        return _zones.FirstOrDefault(zone => zone.Contains(transform));
    }
}
