using MMOJam.Player;
using MMOJam.Manager;
using TMPro;
using UnityEngine;
using Unity.Netcode;

namespace MMOJam
{
    public class IsRessource : AInteractible
    {
        [SerializeField]
        short _res_id = 0;
        [SerializeField]
        long _mine_amount = 1;
        [SerializeField]
        long _res_amount = 10;
        [SerializeField]
        string _name = "PLACEHOLDER";
        [SerializeField] 
        private GameObject _uiRoot;
        private TextMeshPro _ui_text =  null;
        
        private void Awake()
        {
            if (IsClient)
                return;

            if (_uiRoot == null)
            {
                Debug.LogError($"{name}: UI Root is not assigned!");
                return;
            }

            _ui_text = _uiRoot.GetComponent<TextMeshPro>();

            if (_ui_text == null)
            {
                Debug.LogError($"{name}: No TextMeshProUGUI found in UI Root!");
                return;
            }

            UpdateUI();
        }
        public override void InteractClient(PlayerController player)
        {
            var temp = player._ressource_controller;

            temp.RequestRessourceServerRpc(_res_id);
            return;
        }

        public override void InteractServer(PlayerController player)
        {
            var temp = player._ressource_controller;

            long mined = System.Math.Min(_mine_amount, _res_amount);
            temp.ChangeRessources(_res_id, mined);
            _res_amount -= mined;
            updateOnNetworkRpc(_res_amount);
            if (_res_amount < 1) 
            {
                ServerManager.Instance.UnregisterInteractible(this);
                Destroy(gameObject);
            }
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void updateOnNetworkRpc(long amount)
        {
            _res_amount = amount;
            UpdateUI();
        }

        private void UpdateUI()
        {
            _ui_text.text = _name + "\n" + _res_amount;
        }
    }
}
