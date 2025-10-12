using System.Collections;
using System.Threading.Tasks;
using JKTechnologies.SeensioGo.GameInstances;
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
        [SerializeField] private GameObject leaderboardPanel;
        [SerializeField] private GameObject finishLevelPanel;
        [SerializeField] private GameObject endGamePanel;
        [SerializeField] private Button watchAdsButton;
        [SerializeField] private float targetScoreTime = 1f;
        private static bool isGameStarted = false;
        private static bool isAdsWatched = false;

        private IGameInstanceStartListener gameInstanceStarter;
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
                ShowStartingPanel();
            }
            else
            {
                StartNewLevel();
            }
        }
        #endregion


        #region Listen Ads
        public void ListenToAdsCompleted(IGameInstanceAdsListener gameInstanceAdsListener)
        {
            this.gameInstanceAdsListener = gameInstanceAdsListener;
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

            targetScorePanel.SetActive(true);
            yield return new WaitForSeconds(targetScoreTime);
            targetScorePanel.SetActive(false);
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
                GameEngineManager.Instance.QuitGame();
            });
        }
        #endregion
    }
}   