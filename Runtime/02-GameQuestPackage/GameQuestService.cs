using UnityEngine;
using System.Threading.Tasks;

namespace JKTechnologies.CommonPackage
{
    public abstract class GameQuestService : ScriptableObject
    {
        protected static GameQuestService instance;
        protected virtual GameQuestInstance Handle_GetGameQuestRewards() {return null;}
        protected virtual Task<bool> Handle_CompleteCondition(string conditionId, string gameConditionId){return null;}
        protected virtual Task<bool> Handle_CompleteRankReward(int rankIndex){return null;}
        protected virtual Task<GameLeaderboardConfig> HandleGetGameLeaderboardConfig(){return null;}
        protected virtual Task<GameUnityAdsConfig> HandleGetGameUnityAdsConfig(){return null;}
        protected virtual string HandleGetCampaignId(){return null;}
        protected virtual string HandleGetQuestPackPoolId(){return null;}
        #region Interfaces
        public static GameQuestInstance GetGameQuestInstance()
        {
            if(instance == null)
            {
                return null;
            }
            return instance.Handle_GetGameQuestRewards();
        }

        public static async Task<bool> CompleteCondition(string conditionId, string gameConditionId)
        {
            if(instance == null)
            {
                return false;
            }
            return await instance.Handle_CompleteCondition(conditionId, gameConditionId);
        }

        public static async Task<bool> CompleteRankReward(int rankIndex)
        {
            if(instance == null)
            {
                return false;
            }
            return await instance.Handle_CompleteRankReward(rankIndex);
        }
        #endregion

        #region Get Leaderboard Config
        public static async Task<GameLeaderboardConfig> GetGameLeaderboardConfig()
        {
            if(instance == null)
            {
                Debug.LogError("Game Engine Service is null");
                return null;
            }
            return await instance.HandleGetGameLeaderboardConfig();
        }
        #endregion

        #region Get UnityAds Config
        public static async Task<GameUnityAdsConfig> GetGameUnityAdsConfig()
        {
            if(instance == null)
            {
                Debug.LogError("Game Engine Service is null");
                return null;
            }
            return await instance.HandleGetGameUnityAdsConfig();
        }
        #endregion

        #region Get Campaign Id
        public static string GetCampaignId()
        {
            if(instance == null)
            {
                Debug.LogError("Game Engine Service is null");
                return null;
            }
            return instance.HandleGetCampaignId();
        }

        public static string GetQuestPackPoolId()
        {
            if(instance == null)
            {
                Debug.LogError("Game Engine Service is null");
                return null;
            }
            return instance.HandleGetQuestPackPoolId();
        }
        #endregion
    }
}