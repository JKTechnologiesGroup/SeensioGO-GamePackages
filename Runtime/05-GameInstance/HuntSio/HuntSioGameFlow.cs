using System.Collections;
using System.Threading.Tasks;
using JKTechnologies.CommonPackage.LanguageLocalizations;
using JKTechnologies.SeensioGo.GameInstances;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JKTechnologies.SeensioGo.GameEngines.HuntSio 
{
    public class HuntSioGameFlow : MonoBehaviour
    {
        [Header("[ Components ]")]
        [SerializeField] private GameObject loadingPanel;
        [SerializeField] private GameObject startingPanel;
        [SerializeField] private GameObject inGamePanel;
        [SerializeField] private GameObject targetScorePanel;
        [SerializeField] private TextMeshProUGUI targetScoreText;
        [SerializeField] private GameObject questCompletedPanel;
        [SerializeField] private GameObject leaderboardPanel;
        [SerializeField] private GameObject finishLevelPanel;
        [SerializeField] private GameObject endGamePanel;
        [SerializeField] private Button watchAdsButton;
        [SerializeField] private float targetScoreTime = 1f;
        [SerializeField] private float ingameDurationSeconds = 60f;
        [SerializeField] private CountdownTimer inGameCountdownTimer;
        private static bool isGameStarted = false;
        private static bool isAdsWatched = false;
        private static bool isQuestCompltedPanelShown = false;

        private IGameInstanceStartListener gameInstanceStarter;
        private IGameInstanceProgressListener gameInstanceProgressListener;
        private IGameInstanceAdsListener gameInstanceAdsListener;
        private IGameInstanceNextLevelListner gameInstanceNextLevelListner;
        private IGameLeaderboardListener gameLeaderboardListener;

        private void Awake()
        {
            if (isAdsWatched)
            {
                watchAdsButton.interactable = false;
            }
        }

        #region Init
        public void Init(IGameInstanceStartListener gameInstanceStarter)
        {
            this.gameInstanceStarter = gameInstanceStarter;
            if (!isGameStarted)
            {
                var targetScore = GameEngineManager.Instance.GetTargetScore();
                gameInstanceProgressListener.SetupTargetScore(targetScore, 0);
                ShowStartingPanel();
            }
            else
            {
                var targetScore = GameEngineManager.Instance.GetTargetScore();
                var currentPlayerScore = GameEngineManager.Instance.GetCurrentPlayerScore();
                gameInstanceProgressListener.SetupTargetScore(targetScore, currentPlayerScore);
                StartNewLevel();
            }
        }
        #endregion


        #region Listen Ads
        public void ListenToAdsCompleted(IGameInstanceAdsListener gameInstanceAdsListener)
        {
            this.gameInstanceAdsListener = gameInstanceAdsListener;
        }

        public void ListenToProgress(IGameInstanceProgressListener gameInstanceProgressListener)
        {
            this.gameInstanceProgressListener = gameInstanceProgressListener;
        }

        public void ListenToNextLevel(IGameInstanceNextLevelListner gameInstanceNextLevelListner)
        {
            this.gameInstanceNextLevelListner = gameInstanceNextLevelListner;
        }

        public void ListenToLeaderboard(IGameLeaderboardListener gameLeaderboardListener)
        {
            this.gameLeaderboardListener = gameLeaderboardListener;
        }

        #endregion

        #region Start Games
        private async Task ShowStartingPanel()
        {
            await Task.Delay(500);
            startingPanel.SetActive(true);
            loadingPanel.SetActive(false);
            isGameStarted = true;
        }

        public void StartGame()
        {
            StartCoroutine(StartGameCoroutine());
        }

        private IEnumerator StartGameCoroutine()
        {
            startingPanel.SetActive(false);
            gameInstanceStarter.StartGame();
            inGamePanel.SetActive(true);

            var targetScore = GameEngineManager.Instance.GetTargetScore();
            targetScoreText.text = LanguageLocalizationHelper.LookupLanguage("TARGET_SCORE").Replace("[n]", targetScore.ToString());

            targetScorePanel.SetActive(true);
            yield return new WaitForSeconds(targetScoreTime);
            targetScorePanel.GetComponent<Animator>().Play("Minimize");
            inGameCountdownTimer.countdownDuration = ingameDurationSeconds;
            inGameCountdownTimer.StartTimer();
        }

        private void StartNewLevel()
        {
            gameInstanceStarter.StartGame();
            inGamePanel.SetActive(true);
        }
        #endregion

        #region Show Leaderboard
        public void ShowLeaderboard()
        {
            inGamePanel.SetActive(false);
            leaderboardPanel.SetActive(true);
            gameLeaderboardListener.OnShowLeaderboard();
        }

        public void HideLeaderboard()
        {
            leaderboardPanel.SetActive(false);
            inGamePanel.SetActive(true);
            gameLeaderboardListener.OnHideLeaderboard();
        }
        #endregion

        #region Show Quest Completed Panel
        public async void ShowQuestCompletedPanel()
        {
            if (isQuestCompltedPanelShown)
                return;

            isQuestCompltedPanelShown = true;

            questCompletedPanel.SetActive(true);
            await Task.Delay(3000);
            questCompletedPanel.SetActive(false);
        }
        #endregion

        #region Next Level
        public void OpenNextLevel()
        {
            gameInstanceNextLevelListner.OpenNextLevel();
        }
        #endregion

        #region Finish Level
        public void OnLevelFinished(int playerScore)
        {
            inGamePanel.SetActive(false);
            GameEngineManager.Instance.UpdatePlayerScore(playerScore, () =>
            {
                finishLevelPanel.SetActive(true);
            });
        }
        #endregion

        #region Game Overs
        public void OnGameOver(int playerScore)
        {
            inGamePanel.SetActive(false);
            GameEngineManager.Instance.UpdatePlayerScore(playerScore, () =>
            {
                endGamePanel.SetActive(true);
            });
        }
        #endregion

        #region Watch Ads
        public void WatchAdsToEarnBonus()
        {
            GameEngineManager.Instance.ShowAds(() =>
            {
                isAdsWatched = true;
                endGamePanel.SetActive(false);
                inGamePanel.SetActive(true);
                gameInstanceAdsListener?.OnAdSCompleted();
            });
        }
        #endregion

        #region Confirm
        public void SubmitResult()
        {
            GameEngineManager.Instance.SubmitPlayerScore((passCondition) =>
            {
                Debug.LogError("On user confirm result: " + passCondition);
                isGameStarted = false;
                isAdsWatched = false;
                isQuestCompltedPanelShown = false;
                GameEngineManager.Instance.QuitGame();
            });
        }
        #endregion
    }
}   