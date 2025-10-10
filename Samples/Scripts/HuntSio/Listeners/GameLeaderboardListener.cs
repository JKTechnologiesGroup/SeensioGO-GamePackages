using UnityEngine;
using JKTechnologies.SeensioGo.GameInstances;
using System.Collections;

namespace JKTechnologies.SeensioGo.GameEngines.HuntSio
{
    public class GameLeaderboardListener : MonoBehaviour, IGameLeaderboardListener
    {
        [SerializeField] private HuntSioGameFlow huntSioGameFlow;

        [Header("[ In Game UI Components ]")]
        [SerializeField] private GameObject inGameUIComponentHolder;

        private void Awake()
        {
            huntSioGameFlow.ListenToLeaderboard(this);
        }

        public void OnShowLeaderboard()
        {
            inGameUIComponentHolder.SetActive(false);
        }

        public void OnHideLeaderboard()
        {
            inGameUIComponentHolder.SetActive(true);
        }
    }
}