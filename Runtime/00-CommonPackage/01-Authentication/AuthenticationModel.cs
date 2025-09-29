using UnityEngine;
using UnityEngine.Events;

namespace JKTechnologies.CommonPackage.Authentication
{
    [CreateAssetMenu(fileName = "AuthenticationModel", menuName = "JKTechnologies/CommonPackage/Authentication/AuthenticationModel", order = 0)]
    public class AuthenticationModel : ScriptableObject
    {
        [Header("Initialize Events")]
        public UnityEvent OnInitialized = new();
        [SerializeField] private bool isInitialized = false;

        #region Initialize
        public void Initialize()
        {
            isInitialized = true;
            OnInitialized.Invoke();
        }
        #endregion

        #region DeInitialize
        public void DeInitialize()
        {
            isInitialized = false;
        }
        #endregion
    }
}