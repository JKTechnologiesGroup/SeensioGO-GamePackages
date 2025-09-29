using System.Threading.Tasks;
using UnityEngine;

namespace JKTechnologies.CommonPackage.Leaderboard
{
    [CreateAssetMenu(fileName = "LeaderboardUpdater", menuName = "JKTechnologies/CommonPackage/Leaderboard/LeaderboardUpdater", order = 0)]
    public class LeaderboardUpdater : ScriptableObject
    {
        [SerializeField] private LeaderboardModel leaderboardModel;

        public async Task UpdatePlayerScore(int newScore)
        {
            await leaderboardModel.UpdatePlayerScore(newScore);
        }

        public long GetCurrentUserRank()
        {
            return leaderboardModel.GetCurrentUserRank();
        }
    }
}