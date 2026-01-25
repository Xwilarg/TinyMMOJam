using MMOJam.Manager;
using MMOJam.Player;
using Unity.Netcode;

namespace MMOJam
{
    public abstract class AInteractible : NetworkBehaviour
    {
        private ulong _id;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            _id = GetComponent<NetworkObject>().NetworkObjectId;

            ServerManager.Instance.RegisterInteractible(this);
        }

        public abstract void InteractClient(PlayerController player);
        public abstract void InteractServer(PlayerController player);

        public ulong Key => _id;
    }
}
