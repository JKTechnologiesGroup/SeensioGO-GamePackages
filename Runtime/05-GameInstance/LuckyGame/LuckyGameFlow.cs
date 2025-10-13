using System.Threading.Tasks;
using JKTechnologies.CommonPackage;
using JKTechnologies.SeensioGo.GameInstances;
using UnityEngine;

namespace JKTechnologies.SeensioGo.GameEngines.LuckyGames 
{
    public class LuckyGameFlow : MonoBehaviour
    {
        [Header("[ UIs ]")]
        [SerializeField] private GameObject startingPanel;
        [SerializeField] private GameObject inGamePanel;

        [Header("[ Results UIs ]")]
        [SerializeField] private GameObject waitingForSubmittingPanel;
        [SerializeField] private GameRewardItemLoader rewardLoader;
        [SerializeField] private GameObject resultPanel;

        private IGameInstanceStartListener gameInstanceStarter;

        #region Init
        public void Init(IGameInstanceStartListener gameInstanceStarter)
        {
            this.gameInstanceStarter = gameInstanceStarter;
            ShowStartingPanel();
        }
        #endregion


        #region Start Games
        private void ShowStartingPanel()
        {
            startingPanel.SetActive(true);
        }

        public void StartGame()
        {
            startingPanel.SetActive(false);
            inGamePanel.SetActive(true);
            gameInstanceStarter.StartGame();
        }
        #endregion

        #region Watch Ads
        public void WatchAdsToEarnBonus(IGameInstanceAdsListener gameInstanceAdsListener)
        {
            Debug.LogError("Watch ads");
            GameEngineManager.Instance.ShowAds(() =>
            {
                Debug.LogError("Watch ads: Done");
                gameInstanceAdsListener?.OnAdSCompleted();
            });
        }
        #endregion

        #region Submit Lucky Chances
        public async void SubmitLuckyChance(LuckyChanceRarity rarity)
        {
            inGamePanel.SetActive(false);
            waitingForSubmittingPanel.SetActive(true);
            await Task.Delay(2000);
            GameEngineManager.Instance.SubmitLuckyChance(rarity, reward =>
            {
                waitingForSubmittingPanel.SetActive(false);
                resultPanel.SetActive(true);
                rewardLoader.Setup(reward);
            });
        }
        #endregion

        #region 
        public void GoBackToSeensioGo()
        {
            GameEngineManager.Instance.QuitGame();
        }
        #endregion
    }
}   