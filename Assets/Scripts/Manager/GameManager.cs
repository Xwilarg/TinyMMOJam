using MMOJam.Zone;
using Sketch.Translation;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace MMOJam.Manager
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { private set; get; }

        [SerializeField]
        private CinemachineCamera _cam;

        [SerializeField]
        private GameObject _bulletPrefab;
        public GameObject BulletPrefab => _bulletPrefab;

        [SerializeField]
        private UIDocument _uiDocument;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(Instance.gameObject);
                Instance = this;
            }

            Debug.Log("[GAM] GameManager awake called");

            Instance = this;
            Sketch.Translation.Translate.Instance.SetLanguages(new string[] { "english", "french", "dutch", "spanishChile", "spanishCostaRica" });

            _cam.Lens.NearClipPlane = -100f;

            _uiDocument.rootVisualElement.Q<Button>("lang_english").clicked += () => { Sketch.Translation.Translate.Instance.CurrentLanguage = "english"; SceneManager.LoadScene("Main"); };
            _uiDocument.rootVisualElement.Q<Button>("lang_french").clicked += () => { Sketch.Translation.Translate.Instance.CurrentLanguage = "french"; SceneManager.LoadScene("Main"); };
            _uiDocument.rootVisualElement.Q<Button>("lang_dutch").clicked += () => { Sketch.Translation.Translate.Instance.CurrentLanguage = "dutch"; SceneManager.LoadScene("Main"); };
            _uiDocument.rootVisualElement.Q<Button>("lang_spChile").clicked += () => { Sketch.Translation.Translate.Instance.CurrentLanguage = "spanishChile"; SceneManager.LoadScene("Main"); };
            _uiDocument.rootVisualElement.Q<Button>("lang_spCostaRica").clicked += () => { Sketch.Translation.Translate.Instance.CurrentLanguage = "spanishCostaRica"; SceneManager.LoadScene("Main"); };

            foreach (var text in _uiDocument.rootVisualElement.Query<TextElement>().ToList())
            {

                var sentence = text.text;
                foreach (var match in Regex.Matches(text.text, "{([^}]+)}").Cast<Match>())
                {
                    sentence = sentence.Replace(match.Value, Sketch.Translation.Translate.Instance.Tr(match.Groups[1].Value));
                }
                text.text = sentence;
            }
        }

        public void InitNetwork()
        {
            EnemyManager.Instance.SpawnWave(2);
            for (int i = 0; i < 10; i++) RessourcesManager.Instance.SpawnResources();
        }

        public void CheckVictoryCondition()
        {
            var winner = ZoneManager.Instance.GetWinningFaction();
            if (winner != null && winner.Id != 0) // All buildings belong to a single faction
            {
                if (ServerManager.Instance.AreOthersFactionDead(winner))
                {
                    // We win yay
                    UIManager.Instance.DispatchPsaRpc(Sketch.Translation.Translate.Instance.Tr("faction_won", Sketch.Translation.Translate.Instance.Tr(winner.Name)), Sketch.Translation.Translate.Instance.Tr("server_restart"));
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
