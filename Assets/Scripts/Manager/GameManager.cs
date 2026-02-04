using System.Resources;
using UnityEngine;

namespace MMOJam.Manager
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { private set; get; }

        private void Awake()
        {
            Instance = this;
        }

        public void InitNetwork()
        {
            EnemyManager.Instance.SpawnWave(2);
            RessourcesManager.Instance.SpawnResources();
        }
    }
}
