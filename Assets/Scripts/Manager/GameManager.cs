using MMOJam.Zone;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

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
                    UIManager.Instance.DispatchPsaRpc($"{winner.Name} won!", "The server will now restart for the next game");
                    StartCoroutine(WaitAndRestart());
                }
            }
        }

        private IEnumerator WaitAndRestart()
        {
            yield return new WaitForSeconds(5f);
            NetworkManager.Singleton.Shutdown();
            Destroy(gameObject);
            yield return new WaitForEndOfFrame();
            SceneManager.LoadScene("Main");
        }
    }
}
