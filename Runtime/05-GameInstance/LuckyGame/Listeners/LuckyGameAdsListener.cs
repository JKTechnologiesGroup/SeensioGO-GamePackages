using UnityEngine;
using JKTechnologies.SeensioGo.GameInstances;
using UnityEngine.Events;

namespace JKTechnologies.SeensioGo.GameEngines.LuckyGames
{
    public class LuckyGameAdsListener : MonoBehaviour, IGameInstanceAdsListener
    {
        [SerializeField] private UnityEvent OnAdsCompleted;
        [SerializeField] private LuckyGameFlow luckyGameFlow;

        public void WatchAds()
        {
            Debug.LogError("Watch ads: 1");
            luckyGameFlow.WatchAdsToEarnBonus(this);
        }

        public void OnAdSCompleted()
        {
            OnAdsCompleted.Invoke();
        }
    }
}