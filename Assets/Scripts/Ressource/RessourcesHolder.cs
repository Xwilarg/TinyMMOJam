using Unity.Netcode;

namespace MMOJam
{
    public abstract class RessourcesHolder : NetworkBehaviour
    {
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            RessourcesManager.Instance.RegisterInteractible(this);
        }

        public abstract void Interact(PlayerController player);
    }
}
