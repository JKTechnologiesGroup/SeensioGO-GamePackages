using Unity.Services.Analytics;
using UnityEngine;
using UnityEngine.Events;

namespace JKTechnologies.CommonPackage.Analytics
{
    [CreateAssetMenu(fileName = "AnalyticsModel", menuName = "JKTechnologies/CommonPackage/Analytics/AnalyticsModel", order = 0)]
    public class AnalyticsModel : ScriptableObject
    {
        public UnityEvent OnInitialized = new();
        [SerializeField] private bool isInitialized = false; 


        #region Initialize
        public void Initialize()
        {
            DeInitialize();
            // AnalyticsService.Instance?.StartDataCollection();
            // Debug.LogError("Analytics Season ID:" + AnalyticsService.Instance.SessionID);
            // Debug.LogError("Analytics User ID:" + AnalyticsService.Instance.GetAnalyticsUserID());
            isInitialized = true;
            OnInitialized.Invoke();
        }
        #endregion

        #region DeInitialize
        public void DeInitialize()
        {
            isInitialized = false;
            // AnalyticsService.Instance?.StopDataCollection();
        }
        #endregion
    }
}