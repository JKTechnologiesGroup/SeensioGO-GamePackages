using System;
using System.Collections.Generic;
using System.Linq;
using JKTechnologies.CommonPackage;
using JKTechnologies.CommonPackage.Ads;
using JKTechnologies.CommonPackage.Leaderboard;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JKTechnologies.SeensioGo.GameEngines
{
    public enum LuckyChanceRarity
    {
        Common,
        Rare,
        SupperRare,
        Legendary
    }

    [Serializable]
    public class LuckyChanceGroup
    {
        public LuckyChanceRarity Rarity;
        public List<GameConditionInstance> ConditionInstances;
    }

    public class GameEngineManager : MonoBehaviour
    {
        public static GameEngineManager Instance => instance;
        private static GameEngineManager instance;
        

        [SerializeField] private UnityAdsController unityAdsController;
        [SerializeField] private LeaderboardUpdater leaderboardUpdater;

        [Header("[ Hunt Sio Quest Components ]")]
        [SerializeField] private GameConditionController gameConditionController;
        [SerializeField] private GameRankRewardController gameRankRewardController;

        [Header("[ Lucky Game Components ]")]
        [SerializeField] private List<LuckyChanceGroup> luckyChanceGroups;

        [Header("[ Mock Scene ]")]
        [SerializeField] private bool isMockup;

        #region Lifecycle
        private void Awake()
        {
            instance = this;
        }
        private void OnDestroy()
        {
            instance = null;
        }
        #endregion

        #region Game Navigation
        public void LoadSceneByName(string sceneName)
        {
            if (IsMockup())
            {
                SceneManager.LoadScene(sceneName);
                return;
            }
            GameEngineService.LoadScene(sceneName);
        }

        public void LoadSceneByIndex(int sceneIndex)
        {
            GameEngineService.LoadSceneByIndex(sceneIndex);
        }

        public void QuitGame()
        {
            if (IsMockup())
            {
                Debug.LogError("Back to SeensioGO");
                return;
            }

            GameEngineService.QuitGame();
        }
        #endregion

        #region Player Score
        public async void UpdatePlayerScore(int newScore, Action callback)
        {
            await leaderboardUpdater.UpdatePlayerScore(newScore);
            callback.Invoke();
        }
        #endregion

        #region Show Ads
        public void ShowAds(Action callback)
        {
            if (IsMockup())
            {
                callback.Invoke();
                return;
            }
            
            unityAdsController.ShowAdWithCallback(callback);
        }
        #endregion

        #region Add Player Score
        public async void SubmitPlayerScore(Action<bool> callback)
        {
            long currentPlayerScore = leaderboardUpdater.GetCurrentPlayerScore();
            long currentPlayerRank = leaderboardUpdater.GetCurrentUserRank();
            bool isPassCondition = await gameConditionController.UpdateNumberConditionValue(currentPlayerScore);
            if (!isPassCondition)
            {
                callback.Invoke(false);
                return;
            }

            await gameRankRewardController.UpdateRankValue((int)currentPlayerRank);
            callback.Invoke(true);
        }
        #endregion

        #region Submit Lucky Chances
        public async void SubmitLuckyChance(LuckyChanceRarity rarity, Action<GameRewardItem> callback)
        {
            var group = luckyChanceGroups.FirstOrDefault(item => item.Rarity == rarity);
            if (group == null || group.ConditionInstances.Count == 0)
            {
                callback.Invoke(null);
                return;
            }

            foreach (var condition in group.ConditionInstances)
            {
                bool hasWon = await condition.HitWin();
                if (hasWon)
                {
                    bool completed = await condition.CompleteCondition();
                    if (!completed)
                    {
                        Debug.LogError("Something wrong happen");
                        callback.Invoke(null);
                        return;
                    }

                    callback.Invoke(condition.GetReward());
                    return;
                }
            }
        }
        #endregion

        #region Private Helpers
        private bool IsMockup()
        {
#if !UNITY_EDITOR
                return false;
#endif

            return isMockup;
        }
        #endregion
    }
}