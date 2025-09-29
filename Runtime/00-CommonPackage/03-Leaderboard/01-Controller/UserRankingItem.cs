using Newtonsoft.Json;
using TMPro;
using Unity.Services.Leaderboards.Models;
using UnityEngine;
using UnityEngine.UI;

namespace JKTechnologies.CommonPackage.Leaderboard
{
    public class UserRankingItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI rank;
        [SerializeField] private Image userPhoto;
        [SerializeField] private TextMeshProUGUI userName;
        [SerializeField] private TextMeshProUGUI score;

        public void Setup(LeaderboardEntry leaderboardEntry)
        {
            rank.text = $"{leaderboardEntry.Rank + 1}";
            string playerNameOnly = leaderboardEntry.PlayerName;
            if(leaderboardEntry.PlayerName.Contains("#"))
            {
                playerNameOnly = leaderboardEntry.PlayerName.Split("#")[0];
            }
            playerNameOnly = playerNameOnly.Replace("*", " ");
            userName.text = playerNameOnly;
            score.text = $"{leaderboardEntry.Score}";

            if(leaderboardEntry.Metadata != null)
            {
                GameUserRankingMetadata metadata = JsonConvert.DeserializeObject<GameUserRankingMetadata>(leaderboardEntry.Metadata);
                if(metadata != null && !string.IsNullOrEmpty(metadata.photoUrl))
                {
                    if(userPhoto != null)
                    {
                        GameInstanceModel gameInstanceModel = new GameInstanceModel();
                        gameInstanceModel.UtilitiesService.LoadImageFromUrl(userPhoto, metadata.photoUrl, 256);
                    }
                }
            }

        }
    }
}