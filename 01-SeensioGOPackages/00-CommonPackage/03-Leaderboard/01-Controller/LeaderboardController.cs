using System.Collections;
using TMPro;
using Unity.Services.Leaderboards.Models;
using UnityEngine;

namespace JKTechnologies.CommonPackage.Leaderboard
{
    public class LeaderboardController : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private LeaderboardModel leaderboardModel;

        [Header("UI Components")]
        [SerializeField] private UserRankingItem userRankingItemPrefab;
        [SerializeField] private Transform userRankingItemHolder;
        [SerializeField] private UserRankingItem playerRankingItem;
        [SerializeField] private int currentOffset = 0;
        [SerializeField] private bool canLoadMore = false;
        [SerializeField] private bool isLoading = false;

        private bool needToRefresh = false;
        private bool isEnabled = false;
        private bool isStarted = false;
        private void OnEnable()
        {
            isEnabled = true;
            if(isStarted && needToRefresh)
            {
                ReLoadLeaderboard();
            }
        }

        private void OnDisable()
        {
            isEnabled = false;
        }

        private void Start()
        {
            if(leaderboardModel.IsInitialized)
            {
                Debug.LogError("IsInitialized");
                StartLeaderboard();
            }
            else
            {
                leaderboardModel.OnInitialized.AddListener(StartLeaderboard);
            }            
        }

        private void OnDestroy()
        {
            leaderboardModel.OnInitialized.RemoveListener(StartLeaderboard);
            leaderboardModel.OnLeaderboardUpdated.RemoveListener(OnLeaderboardUpdated);
        }

        private void StartLeaderboard()
        {
            isStarted = true;
            ReLoadLeaderboard();
            leaderboardModel.OnLeaderboardUpdated.AddListener(OnLeaderboardUpdated);
        }

        private void OnLeaderboardUpdated()
        {
            if(isEnabled)
            {
                ReLoadLeaderboard();
            }
            else
            {
                needToRefresh = true;
            }
        }

        private void ReLoadLeaderboard()
        {
            for(int i = userRankingItemHolder.childCount - 1; i >= 0; i--)
            {
                Destroy(userRankingItemHolder.GetChild(i).gameObject);
            }
            StartCoroutine(LoadLeaderboard());
        }

        private IEnumerator LoadLeaderboard()
        {
            LeaderboardEntry[] leaderboardEntries = leaderboardModel.GetLeaderboardEntries(0);
            foreach(LeaderboardEntry entry in leaderboardEntries)
            {
                UserRankingItem userRankingItem = Instantiate(userRankingItemPrefab, userRankingItemHolder);
                yield return new WaitForSeconds(0.05f);
                userRankingItem.Setup(entry);
            }

            LeaderboardEntry playerLeaderboardEntry = leaderboardModel.GetPlayerLeaderboardEntry();
            if(playerLeaderboardEntry != null)
            {
                playerRankingItem.Setup(playerLeaderboardEntry);
                playerRankingItem.gameObject.SetActive(true);
            }
            currentOffset = 0;
        }

        public async void LoadNextLeaderboardOffset()
        {
            if(!canLoadMore)
            {
                return;
            }

            if(isLoading)
            {
                return;
            }
            isLoading = true;
            currentOffset = currentOffset + 1;
            LeaderboardEntry[] leaderboardEntries = await leaderboardModel.GetLeaderboardEntriesAsync(currentOffset);
            if(leaderboardEntries != null && leaderboardEntries.Length > 0)
            {
                foreach(LeaderboardEntry entry in leaderboardEntries)
                {
                    UserRankingItem userRankingItem = Instantiate(userRankingItemPrefab, userRankingItemHolder);
                    userRankingItem.Setup(entry);
                }
                canLoadMore = true;
            }
            else
            {
                canLoadMore = false;
                Debug.LogError("No more leaderboard entries");
            }
            isLoading = false;
        }
    }
}