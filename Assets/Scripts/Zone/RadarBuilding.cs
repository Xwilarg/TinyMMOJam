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

        private PlayerController _isPlayerInside;

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
            if (player.IsLocalHuman)
            {
                if (player.CurrentFaction.Value == AttachedZone.Faction.Id)
                {
                    UIManager.Instance.ToggleMinimap(true);
                    _camera.SetActive(true);
                }
                _isPlayerInside = player;
            }
        }
        public override void OnZoneExit(PlayerController player)
        {
            if (player.IsLocalHuman)
            {
                UIManager.Instance.ToggleMinimap(false);
                _isPlayerInside = null;
                _camera.SetActive(false);
            }
        }

        public override void SpawnPlayer()
        {
            _camera.SetActive(true);
        }
    }
}
