using System;
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
            unityAdsModel.OnUnityAdsShowComplete.AddListener(OnAdCompletedEvent);
            unityAdsModel.OnStartShowAd.Invoke(adType);
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