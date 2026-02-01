using MMOJam.Player;
using MMOJam.Manager;
using UnityEngine;

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
        public override void InteractClient(PlayerController player)
        {
            return;
        }

        public override void InteractServer(PlayerController player)
        {
            var temp = player.GetComponent<RessourcesHolder>();

            long mined = System.Math.Min(_mine_amount, _res_amount);
            temp.ChangeRessources(_res_id, mined);
            _res_amount -= mined;
            if (_res_amount < 1) 
            {
                ServerManager.Instance.UnregisterInteractible(this);
                Destroy(gameObject);
            }
        }
    }
}
