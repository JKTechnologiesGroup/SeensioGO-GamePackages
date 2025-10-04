using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;

namespace JKTechnologies.CommonPackage.Ads
{
    public enum UnityAdType
    {
        Banner,
        Interstitial,
        Rewarded
    }

    [CreateAssetMenu(fileName = "UnityAdsModel", menuName = "JKTechnologies/CommonPackage/Ads/UnityAdsModel", order = 0)]
    public class UnityAdsModel : ScriptableObject
    {
        [Header("Initialize")]
        public UnityEvent OnInitialized = new();
        public bool IsInitialized => isInitialized;

        [Header("Request Ads Events")]
        public UnityEvent<UnityAdType> OnStartShowAd;

        [Header("Unity Ads Events")]
        public UnityEvent OnUnityAdsAdLoaded;
        public UnityEvent OnUnityAdsShowStart;
        public UnityEvent OnUnityAdsShowClick;
        public UnityEvent OnUnityAdsShowComplete;
        public UnityEvent OnUnityAdsShowFailed;
        public UnityEvent OnUnityAdsFailedToLoad;

        [Header("Unity Ads Initialization Events")]
        public UnityEvent OnInitializationComplete;
        public UnityEvent OnInitializationFailed;
        
        [Header("Components")]
        [SerializeField] private GameUnityAdsConfig unityAdsConfig;
        [SerializeField] private bool isInitialized = false;

        #region Initialize
        public void Initialize(GameUnityAdsConfig unityAdsConfig)
        {
            if(isInitialized)
            {
                Debug.LogError("Unity Ads Model is already  initialized: " + isInitialized);
            }
            this.unityAdsConfig = unityAdsConfig;
            isInitialized = true;
            OnInitialized.Invoke();
        }
        #endregion
        
        #region Get Config
        public GameUnityAdsConfig GetGameUnityAdsConfig()
        {
            return unityAdsConfig;
        }
        #endregion


        #region DeInitialize
        public void DeInitialize()
        {
            isInitialized = false;
            unityAdsConfig = new();
        }
        #endregion
    }
}