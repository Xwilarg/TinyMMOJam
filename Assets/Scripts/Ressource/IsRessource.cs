using Unity.Netcode;
using MMOJam.Manager;
using System.Collections.Generic;
using System.Data.Common;
using MMOJam.Player;
using MMOJam.SO;
using UnityEngine;

namespace MMOJam
{
    public class IsRessource : AInteractible
    {
        [SerializeField]
        short _res_id = 0;
        [SerializeField]
        long _amount = 1;
        public override void InteractClient(PlayerController player)
        {
            return;
        }

        public override void InteractServer(PlayerController player)
        {
            var temp = player.GetComponent<RessourcesHolder>();

            temp.ChangeRessources(_res_id, _amount);
        }
    }
}
