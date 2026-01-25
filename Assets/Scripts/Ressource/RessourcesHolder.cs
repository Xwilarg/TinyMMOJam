using Unity.Netcode;
using MMOJam.Manager;

namespace MMOJam
{
    public class RessourcesHolder : NetworkBehaviour
    {
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            RessourcesManager.Instance.RegisterHolder(this);
        }
    }
}
