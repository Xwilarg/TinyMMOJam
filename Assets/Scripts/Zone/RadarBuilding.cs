using MMOJam.Manager;
using MMOJam.Player;
using MMOJam.SO;
using UnityEngine;

namespace MMOJam.Zone
{
    internal class RadarBuilding : ABuilding
    {
        [SerializeField]
        private GameObject _camera;

        private void Awake()
        {
            _camera.SetActive(false);
        }

        public override void BuildingDestroyed(FactionInfo f)
        {
            base.BuildingDestroyed(f);
            Debug.Log("[BLD] Radar destoyed");

            if (_isPlayerInside != null) UIManager.Instance.ToggleMinimap(false);
        }

        public override void BuildingRestored(FactionInfo f)
        {
            if (_isPlayerInside != null && _isPlayerInside.CurrentFaction.Value == f.Id) UIManager.Instance.ToggleMinimap(true); 
        }

        public override void OnZoneEnter(PlayerController player)
        {
            base.OnZoneEnter(player);
            if (player.IsLocalHuman)
            {
                if (player.CurrentFaction.Value == AttachedZone.Faction.Id)
                {
                    UIManager.Instance.ToggleMinimap(true);
                    _camera.SetActive(true);
                }
            }
        }
        public override void OnZoneExit(PlayerController player)
        {
            base.OnZoneExit(player);
            if (player.IsLocalHuman)
            {
                UIManager.Instance.ToggleMinimap(false);
                _camera.SetActive(false);
            }
        }

        public override void SpawnPlayer()
        {
            _camera.SetActive(IsAlive);
            UIManager.Instance.ToggleMinimap(IsAlive);
        }
    }
}
