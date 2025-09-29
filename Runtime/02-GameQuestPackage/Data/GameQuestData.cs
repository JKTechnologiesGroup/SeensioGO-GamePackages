using System;

namespace JKTechnologies.CommonPackage
{
    #region Game Quest Data
    [Serializable]
    public class GameQuestInstance   
    {
        public GameConditionReward[] rankRewards;
        public GameConditionReward[] conditionRewards;
    }

    [Serializable]
    public class GameConditionReward
    {
        public string id;
        public bool canClaim;
        public GameCondition[] conditions;
        public GameRewardItem[] rewards;
    }

    [Serializable]
    public class GameRewardItem
    {
        public string title;
        public string photoUrl;
        public int amount;
    }

    [Serializable]
    public class GameCondition
    {
        public string id;
        public string name;
        public string stringValue;
        public float numberValue;
        public GameConditionType conditionType;
        public bool isCompleted;
    }

    public enum GameConditionType
    {
        None = 0,
        Greater = 1,
        Less = 2,
        Equal = 3,
        EqualOrGreater = 4,
    }
    #endregion

    [Serializable]
    public class GameLeaderboardConfig
    {
        public string leaderboardId;
        public string leaderboardName;
    }

    [Serializable]
    public class GameUnityAdsConfig
    {
        public string gameId;
        public string bannerPlacementID;
        public string interstitialPlacementID;
        public string rewardedPlacementID;
    }

}