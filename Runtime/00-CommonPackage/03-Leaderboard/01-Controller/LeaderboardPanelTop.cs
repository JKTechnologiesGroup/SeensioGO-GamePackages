using TMPro;
using Unity.Services.Leaderboards.Models;
using UnityEngine;

namespace JKTechnologies.CommonPackage.Leaderboard
{
    public class LeaderboardTopPanel : MonoBehaviour
    {
        [Header("Dependencies")]
        // [SerializeField] private int top;
        [SerializeField] private LeaderboardModel leaderboardModel;

        [Header("UI Components")]
        [SerializeField] private UserRankingItem top1RankingItem;
        // [SerializeField] private Transform userRankingItemHolder;
        [SerializeField] private UserRankingItem playerRankingItem;
        [SerializeField] private bool isStarted = false;
        private void Start()
        {
            if(leaderboardModel.IsInitialized)
            {
                LoadTopPlayers();
            }
            else
            {
                leaderboardModel.OnInitialized.AddListener(LoadTopPlayers);
            }
        }
        
        private void OnEnable()
        {
            if(isStarted)
            {
                LoadTopPlayers();
            }
        }

        private void OnDestroy()
        {
            leaderboardModel.OnInitialized.RemoveListener(LoadTopPlayers);
        }

        private void LoadTopPlayers()
        {
            isStarted = true;
            // foreach(Transform child in userRankingItemHolder)
            // {
            //     Destroy(child.gameObject);
            // }

            // Debug.LogError("LoadTopPlayers");
            LeaderboardEntry[] leaderboardEntries = leaderboardModel.GetLeaderboardEntries(0);
            
            if(leaderboardEntries.Length > 0)
            {
                top1RankingItem.Setup(leaderboardEntries[0]);
                top1RankingItem.gameObject.SetActive(true);
            }
            // for(int i =0; i < top && i < leaderboardEntries.Length; ++i)
            // {
            //     Debug.LogError("Number spawned: " + i);
            //     Debug.LogError("Leaderboard entry: " + leaderboardEntries[i].Score);
            //     UserRankingItem userRankingItem = Instantiate(userRankingItemPrefab, userRankingItemHolder);
            //     userRankingItem.Setup(leaderboardEntries[i]);
            //     userRankingItem.gameObject.SetActive(true);
                
            // }

            LeaderboardEntry playerLeaderboardEntry = leaderboardModel.GetPlayerLeaderboardEntry();
            if(playerLeaderboardEntry != null)
            {
                playerRankingItem.Setup(playerLeaderboardEntry);
                playerRankingItem.gameObject.SetActive(true);
            }
        }
    }
}