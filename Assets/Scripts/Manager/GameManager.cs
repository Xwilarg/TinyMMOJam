using MMOJam.Zone;
using Sketch.Translation;
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
            Translate.Instance.SetLanguages(new string[] { "english", "french", "dutch" });
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
                    UIManager.Instance.DispatchPsaRpc(Translate.Instance.Tr("faction_won", Translate.Instance.Tr(winner.Name)), Translate.Instance.Tr("server_restart"));
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
