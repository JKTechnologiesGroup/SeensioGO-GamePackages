
using System;
using UnityEngine;
using UnityEngine.Advertisements;

namespace JKTechnologies.CommonPackage.Ads
{
    // Banner
    // Interstitial
    // Rewarded
    public class UnityAdsController : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
    {
        [Header("Dependencies")]
        [SerializeField] private UnityAdsModel unityAdsModel;

        [Header("Settings")]
        [SerializeField] private GameUnityAdsConfig currentConfig;
        [SerializeField] private UnityAdType adType = UnityAdType.Interstitial;
        [SerializeField] private bool loadPreAd = true;
        [SerializeField] private bool forceShow = false;
        [SerializeField] private bool showBanner = false;
        [SerializeField] private BannerPosition bannerPosition = BannerPosition.BOTTOM_CENTER;

        [Header("State")]
        [SerializeField] private string currentPlacementID;
        [SerializeField] private bool isAdLoaded = false;

        private Action showAdCallback;

        #region Initialize
        private void Start()
        {
            if (unityAdsModel.IsInitialized)
            {
                Setup();
            }
            else
            {
                unityAdsModel.OnInitialized.AddListener(Setup);
            }
        }

        private void OnDestroy()
        {
            unityAdsModel.OnInitialized.RemoveListener(Setup);
            unityAdsModel.OnStartShowAd.RemoveListener(OnStartShowAdEvent);
            unityAdsModel.OnUnityAdsShowComplete.RemoveListener(OnAdShowCompleted);
        }
        #endregion

        #region Setup
        public void Setup()
        {
            if (!Advertisement.isSupported)
            {
                Debug.LogError("Advertisement is not supported");
                return;
            }
            currentConfig = unityAdsModel.GetGameUnityAdsConfig();
            InitializeUnityAdsService();
            unityAdsModel.OnStartShowAd.AddListener(OnStartShowAdEvent);
            unityAdsModel.OnUnityAdsShowComplete.AddListener(OnAdShowCompleted);
        }

        private void InitializeUnityAdsService()
        {
            Debug.LogError("Check if multiple invoke: " + currentConfig.gameId);
#if UNITY_EDITOR
            Advertisement.Initialize(currentConfig.gameId, true, this);
#else
            Advertisement.Initialize(currentConfig.gameId, false, this);
#endif
        }

        private void OnStartShowAdEvent(UnityAdType unityAdType)
        {
            if (unityAdType != this.adType)
            {
                return;
            }
            ShowAd();
        }

        private void OnAdShowCompleted()
        {
            if (loadPreAd)
            {
                LoadAdPre();
            }
        }
        #endregion

        #region Initialize Interfaces
        public void OnInitializationComplete()
        {
            Debug.LogError("Init Success");
            unityAdsModel.OnInitializationComplete.Invoke();
            if (loadPreAd)
            {
                LoadAdPre();
            }
        }

        public void OnInitializationFailed(UnityAdsInitializationError error, string message)
        {
            Debug.LogError($"Init Failed: [{error}]: {message}");
            unityAdsModel.OnInitializationFailed.Invoke();
        }
        #endregion

        #region Load Ads
        public void LoadAdPre()
        {
            loadPreAd = true;
            LoadAd();
        }

        public void LoadAd()
        {
            Debug.LogError("Begin loading the ad");
            if (isAdLoaded)
            {
                Debug.LogError("Ad already loaded");
                return;
            }
            switch (adType)
            {
                case UnityAdType.Banner:
                    Advertisement.Load(currentConfig.bannerPlacementID, this);
                    break;
                case UnityAdType.Interstitial:
                    Advertisement.Load(currentConfig.interstitialPlacementID, this);
                    break;
                case UnityAdType.Rewarded:
                    Advertisement.Load(currentConfig.rewardedPlacementID, this);
                    break;
                default:
                    Advertisement.Load(currentConfig.interstitialPlacementID, this);
                    break;
            }
        }
        #endregion

        #region Load Interfaces
        public void OnUnityAdsAdLoaded(string placementId)
        {
            Debug.LogError($"Load Success: {placementId}");
            unityAdsModel.OnUnityAdsAdLoaded.Invoke();
            currentPlacementID = placementId;
            isAdLoaded = true;
            if (forceShow)
            {
                forceShow = false;
                ShowAd();
            }
        }

        public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
        {
            Debug.LogError($"Load Failed: [{error}:{placementId}] {message}");
            unityAdsModel.OnUnityAdsFailedToLoad.Invoke();
            isAdLoaded = false;

            showAdCallback?.Invoke();
            showAdCallback = null;
        }
        #endregion

        #region Show Interfaces
        public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
        {
            Debug.LogError($"OnUnityAdsShowFailure: [{error}]: {message}");
            unityAdsModel.OnUnityAdsShowFailed.Invoke();

            showAdCallback?.Invoke();
            showAdCallback = null;
        }

        public void OnUnityAdsShowStart(string placementId)
        {
            Debug.LogError($"OnUnityAdsShowStart: {placementId}");
            unityAdsModel.OnUnityAdsShowStart.Invoke();
        }

        public void OnUnityAdsShowClick(string placementId)
        {
            Debug.LogError($"OnUnityAdsShowClick: {placementId}");
            unityAdsModel.OnUnityAdsShowClick.Invoke();
        }

        public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
        {
            Debug.LogError($"OnUnityAdsShowComplete: [{showCompletionState}]: {placementId}");
            currentPlacementID = null;
            isAdLoaded = false;
            unityAdsModel.OnUnityAdsShowComplete.Invoke();

            showAdCallback?.Invoke();
            showAdCallback = null;
        }
        #endregion

        #region Handle events
        public void ShowBanner()
        {
            if (showBanner)
            {
                Advertisement.Banner.SetPosition(bannerPosition);
                Advertisement.Banner.Show(currentConfig.bannerPlacementID);
            }
            else
            {
                Advertisement.Banner.Hide(false);
            }
        }

        private void ShowAd()
        {
            if (!isAdLoaded)
            {
                forceShow = true;
                LoadAd();
                return;
            }
            Debug.LogError("Begin showing the ad");
            Advertisement.Show(currentPlacementID, this);
        }
        #endregion

        public void ShowAdWithCallback(Action callback)
        {
            this.showAdCallback = callback;
            ShowAd();
        }
    }
}