using JKTechnologies.SeensioGo.GameInstances;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace JKTechnologies.SeensioGo.GameEngines.HuntSio
{
    public class GameInstanceAdsListener : MonoBehaviour, IGameInstanceAdsListener
    {
        [SerializeField] private UnityEvent OnAdCompleted;
        [SerializeField] private HuntSioGameFlow huntSioGameFlow;
        [SerializeField] private GameObject inGameComponents;
        [SerializeField] private TextMeshProUGUI AdsCompletedText;

        [Header("[ In Game UI Components ]")]
        [SerializeField] private GameObject inGameUIComponents;

        private void Awake()
        {
            huntSioGameFlow.ListenToAdsCompleted(this);
        }

        public void OnAdSCompleted()
        {
            inGameUIComponents.SetActive(true);
            AdsCompletedText.text = "Ads Completed. Continue to play game";
            OnAdCompleted.Invoke();
        }
    }
}