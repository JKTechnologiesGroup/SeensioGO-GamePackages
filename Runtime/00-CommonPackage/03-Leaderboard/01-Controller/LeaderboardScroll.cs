using UnityEngine;

namespace JKTechnologies.CommonPackage.Leaderboard
{
    public class LeaderboardScroll : MonoBehaviour
    {
        [SerializeField] private LeaderboardController leaderboardController;
        public void OnScrollValueChange(Vector2 vector2)
        {
            if(vector2.y < 0.01)
            {
                leaderboardController.LoadNextLeaderboardOffset();
            }
        }
    }
}