using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;

namespace JKTechnologies.CommonPackage.Leaderboard
{
    [CreateAssetMenu(fileName = "LeaderboardModel", menuName = "JKTechnologies/CommonPackage/Leaderboard/LeaderboardModel", order = 0)]
    public class LeaderboardModel : ScriptableObject
    {
        public bool IsInitialized {
            get
            {
                if (IsMockup())
                    return true;
                    
                return isInitialized;
            }
        }

        public UnityEvent OnInitialized = new();
        public UnityEvent OnLeaderboardUpdated = new();
        [SerializeField] private GameLeaderboardConfig leaderboardConfig;
        [SerializeField] private int Offset = 0;
        [SerializeField] private int Limit = 10;
        [SerializeField] private List<LeaderboardScoresPage> leaderboardScoresPageList = new();
        [SerializeField] private LeaderboardEntry currentPlayerLeaderboardEntry = null;
        [SerializeField] private bool isInitialized = false;

        [Header("Mockup Data")]
        [SerializeField] private bool isMockup = false;
        [SerializeField] private long mockupPlayerScore = 0;
        private bool IsMockup()
        {
#if !UNITY_EDITOR
                return false;
#endif
            return isMockup;
        }

        private LeaderboardEntry GetMockEntry()
        {
            return new LeaderboardEntry("MockPlayerId", "MockPlayerName", 0, mockupPlayerScore);
        }
        #region Initialize
        public async Task Initialize(GameLeaderboardConfig leaderboardConfig)
        {
            if (IsMockup())
                return;

            DeInitialize();
            this.leaderboardConfig = leaderboardConfig;
            await LoadLeaderboard();
            await LeaderPlayerLeaderboardEntry();
            isInitialized = true;
            OnInitialized.Invoke();
        }

        private async Task LoadLeaderboard()
        {
            if (isMockup)
                return;

            leaderboardScoresPageList.Clear();
            LeaderboardScoresPage leaderboardScoresPage = await LeaderboardsService.Instance.GetScoresAsync(leaderboardConfig.leaderboardId, new GetScoresOptions{Offset = Offset, Limit = Limit, IncludeMetadata = true});
            if(leaderboardScoresPage != null)
            {
                leaderboardScoresPageList.Add(leaderboardScoresPage);
            }
        }

        private async Task LeaderPlayerLeaderboardEntry()
        {
            
            if (IsMockup())
                return;
                
            try
            {
                LeaderboardEntry playerLeaderboardEntry = await LeaderboardsService.Instance.GetPlayerScoreAsync(leaderboardConfig.leaderboardId, new GetPlayerScoreOptions() { IncludeMetadata = true });
                if (playerLeaderboardEntry != null)
                {
                    currentPlayerLeaderboardEntry = playerLeaderboardEntry;
                }
                else
                {
                    Debug.LogError("Current player do not have leaderboard record");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Exception: " + ex.Message);
            }
        }
        #endregion

        #region Interface
        public string GetLeaderboardName()
        {
            return leaderboardConfig.leaderboardName;
        }
        public LeaderboardEntry[] GetLeaderboardEntries(int offset)
        {
            if (IsMockup())
                return new LeaderboardEntry[1]{GetMockEntry()} ;

            Debug.LogError("Get leaderboard entries by offset: " + offset);
            LeaderboardScoresPage leaderboardScoresPage = leaderboardScoresPageList.FirstOrDefault(item => item.Offset == offset);
            if(leaderboardScoresPage != null)
            {
                return leaderboardScoresPage.Results.ToArray();
            }
            else
            {
                return new LeaderboardEntry[0];
            }
        }

        public async Task<LeaderboardEntry[]> GetLeaderboardEntriesAsync(int offset)
        {
            if (IsMockup())
                return new LeaderboardEntry[1]{GetMockEntry()} ;

            LeaderboardEntry[] leaderboardEntries = GetLeaderboardEntries(offset);
            if(leaderboardEntries == null || leaderboardEntries.Length == 0)
            {
                LeaderboardScoresPage result = await LeaderboardsService.Instance.GetScoresAsync(leaderboardConfig.leaderboardId, new GetScoresOptions{Offset = Offset, Limit = Limit, IncludeMetadata = true});
                if(result != null)
                {
                    leaderboardScoresPageList.Add(result);
                    leaderboardEntries = result.Results.ToArray();
                }
            }
            return leaderboardEntries;
        }

        public LeaderboardEntry GetPlayerLeaderboardEntry()
        {
            if (IsMockup())
                return GetMockEntry();

            return currentPlayerLeaderboardEntry;
        }
        #endregion

        #region Update score
        public async Task UpdatePlayerScore(int newScore)
        {
            if (IsMockup())
            {
                mockupPlayerScore = newScore;
                return;
            }

            GameInstanceModel gameInstanceModel = new();
            GameUserInfo gameUserInfo = gameInstanceModel.UserService.GetUserInfo();

            AddPlayerScoreOptions options = new AddPlayerScoreOptions();
            GameUserRankingMetadata gameUserRankingMetadata = new()
            {
                displayName = gameUserInfo.displayName,
                photoUrl = gameUserInfo.photoUrl
            };
            options.Metadata = gameUserRankingMetadata;

            try
            {
                LeaderboardEntry leaderboardEntry = await LeaderboardsService.Instance.AddPlayerScoreAsync(leaderboardConfig.leaderboardId, newScore, options);
                if(leaderboardEntry != null)
                {
                    if(currentPlayerLeaderboardEntry == null || leaderboardEntry.Score != currentPlayerLeaderboardEntry.Score)
                    {
                        await LoadLeaderboard();
                        currentPlayerLeaderboardEntry = leaderboardEntry;
                        OnLeaderboardUpdated.Invoke();
                    }
                }
                else
                {
                    Debug.LogError("Update score failed: " + leaderboardConfig.leaderboardId);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Leaderboard exception: " + ex.Message);
                return;
            }
        }
        #endregion

        #region Get current user rank
        public long GetCurrentUserRank()
        {
            if (IsMockup())
                return 0;

            if (currentPlayerLeaderboardEntry == null)
            {
                return long.MaxValue;
            }
            return currentPlayerLeaderboardEntry.Rank + 1;
        }

        public long GetCurrentPlayerScore()
        {
            if (IsMockup())
                return mockupPlayerScore;

            if (currentPlayerLeaderboardEntry == null)
            {
                return long.MinValue;
            }
            return (long)currentPlayerLeaderboardEntry.Score;
        }
        #endregion

        #region DeInitialize
        public void DeInitialize()
        {
            isInitialized = false;
            leaderboardConfig = new();
            leaderboardScoresPageList = new();
            currentPlayerLeaderboardEntry = null;
        }
        #endregion
    }
}