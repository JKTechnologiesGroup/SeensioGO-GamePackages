using TMPro;
using UnityEngine;

namespace JKTechnologies.CommonPackage.LanguageLocalizations
{
    public class LocalizationStringEvent : MonoBehaviour
    {
        [SerializeField] private string key;
        [SerializeField] private TextMeshProUGUI textMeshProUGUI;

        #region Lifecycle
        public void Start()
        {
            if (LanguageLocalizationTable.IsInitialized)
            {
                StartStringEvent();
            }
            else
            {
                LanguageLocalizationTable.OnInitialized.AddListener(StartStringEvent);
            }
        }

        public void OnDestroy()
        {
            LanguageLocalizationTable.OnInitialized?.RemoveListener(StartStringEvent);
            LanguageLocalizationTable.Instance?.OnLanguageChanged.RemoveListener(LoadString);
        }
        #endregion

        #region Load String
        private void StartStringEvent()
        {
            LanguageLocalizationTable.Instance?.OnLanguageChanged.AddListener(LoadString);
            LoadString();
        }
        private void LoadString()
        {
            textMeshProUGUI.text = LanguageLocalizationTable.Instance.GetValue(key);
        }
        #endregion
    }
}