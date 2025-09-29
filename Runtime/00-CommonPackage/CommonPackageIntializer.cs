using UnityEngine;
using System.Threading.Tasks;

using JKTechnologies.CommonPackage.Authentication;
using JKTechnologies.CommonPackage.Analytics;
using JKTechnologies.CommonPackage.Leaderboard;
using JKTechnologies.CommonPackage.Ads;

namespace JKTechnologies.CommonPackage
{
    [CreateAssetMenu(fileName = "CommonPackageInitializer", menuName = "JKTechnologies/CommonPackage/CommonPackageInitializer", order = 0)]
    public class CommonPackageInitializer : ScriptableObject
    {
        [Header("Components")]
        [SerializeField] private AuthenticationModel authenticationModel;
        [SerializeField] private AnalyticsModel analyticsModel;
        [SerializeField] private LeaderboardModel leaderboardModel;
        [SerializeField] private UnityAdsModel unityAdsModel;

        [Header("Config from server")]
        [SerializeField] private GameLeaderboardConfig gameLeaderboardConfig;
        [SerializeField] private bool isUsingDefaultLeaderboardConfig = false;
        [SerializeField] private GameUnityAdsConfig gameUnityAdsConfig;
        [SerializeField] private bool isUsingDefaultUnityAdsConfig = false;

        [Header("Default Config")]
        [SerializeField] private GameLeaderboardConfig defaultGameLeaderboardConfig;
        [SerializeField] private GameUnityAdsConfig defaultGameUnityIOSAdsConfig;
        [SerializeField] private GameUnityAdsConfig defaultGameUnityAndroidAdsConfig;

        [Header("State")]
        [SerializeField] private bool isInitialized = false;

        #region Initialize
        public async void Initialize()
        {
            InitializeAuthentication();
            InitializeAnalytics();
            await InitializeUnityAds();
            await InitializeLeaderboard();
            isInitialized = true;
        }

        private void InitializeAuthentication()
        {
            authenticationModel.Initialize();
        }

        private void InitializeAnalytics()
        {
            analyticsModel.Initialize();
        }

        private async Task InitializeLeaderboard()
        {
            gameLeaderboardConfig = await GameQuestService.GetGameLeaderboardConfig();
            if(gameLeaderboardConfig == null)
            {
                isUsingDefaultLeaderboardConfig = true;
                gameLeaderboardConfig = defaultGameLeaderboardConfig;
            }
            else
            {
                isUsingDefaultLeaderboardConfig = false;
            }
            await leaderboardModel.Initialize(gameLeaderboardConfig);
        }

        private async Task InitializeUnityAds()
        {
            gameUnityAdsConfig = await GameQuestService.GetGameUnityAdsConfig();
            if(gameUnityAdsConfig == null)
            {
                isUsingDefaultUnityAdsConfig = true;
                if(Application.platform == RuntimePlatform.Android)
                {
                    gameUnityAdsConfig = defaultGameUnityIOSAdsConfig;
                }
                else 
                {
                    gameUnityAdsConfig = defaultGameUnityIOSAdsConfig;
                }
            }
            else
            {
                isUsingDefaultUnityAdsConfig = false;
            }
            unityAdsModel.Initialize(gameUnityAdsConfig);
        }
        #endregion

        #region DeInitialize
        public void DeInitialize()
        {
            isInitialized = false;
        }
        #endregion
    }
}