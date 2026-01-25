using Unity.Netcode;

public class ZoneManager : NetworkBehaviour
{
    public static ZoneManager Instance { private set; get; }

    private void Awake()
    {
        Instance = this;
    }
}
