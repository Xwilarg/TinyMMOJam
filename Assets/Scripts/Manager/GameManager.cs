using MMOJam.Zone;
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

        public void CheckVictoryCondition()
        {
            var winner = ZoneManager.Instance.GetWinningFaction();
            if (winner != null) // All buildings belong to a single faction
            {
                if (ServerManager.Instance.AreOthersFactionDead(winner))
                {
                    // We win yay
                    Debug.Log($"{winner.Name} won!");
                }
            }
        }
    }
}
