using MMOJam.Manager;
using MMOJam.Player;
using Unity.Netcode;

namespace MMOJam
{
    public abstract class AInteractible : NetworkBehaviour
    {
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            ServerManager.Instance.RegisterInteractible(this);
        }

        public abstract void Interact(PlayerController player);
    }
}
