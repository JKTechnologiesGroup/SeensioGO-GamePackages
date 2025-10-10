using UnityEngine;

namespace JKTechnologies.SeensioGo.GameInstances
{
    public interface IGameLeaderboardListener
    {
        public void OnShowLeaderboard();
        public void OnHideLeaderboard();
    }
}