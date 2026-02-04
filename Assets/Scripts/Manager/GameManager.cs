using UnityEngine;

namespace MMOJam.Manager
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { private set; get; }

        public void InitNetwork()
        {
            EnemyManager.Instance.SpawnWave(2);
        }
    }
}
