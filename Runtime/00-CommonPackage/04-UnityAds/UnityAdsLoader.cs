using UnityEngine;
using UnityEngine.Events;

namespace JKTechnologies.CommonPackage.Ads
{
    public class UnityAdsLoader : MonoBehaviour
    {
        [SerializeField] private UnityAdType adType = UnityAdType.Interstitial;
        [SerializeField] private UnityEvent OnAdCompleted;
        [SerializeField] private UnityAdsModel unityAdsModel;

        #region Actions
        public void ShowAd()
        {
#if SEENSIOGO
            unityAdsModel.OnUnityAdsShowComplete.AddListener(OnAdCompletedEvent);
            unityAdsModel.OnStartShowAd.Invoke(adType);
#else
            OnAdCompleted.Invoke();
#endif
        }

        private void OnAdCompletedEvent()
        {
            unityAdsModel.OnUnityAdsShowComplete.RemoveListener(OnAdCompletedEvent);
            OnAdCompleted.Invoke();
        }
        #endregion

        private void OnDestroy()
        {
            unityAdsModel.OnUnityAdsShowComplete.RemoveListener(OnAdCompletedEvent);
        }
    }
}