using UnityEngine;

namespace JKTechnologies.CommonPackage.LanguageLocalizations
{
    public class LanguageLocalizationInitializer : MonoBehaviour
    {
        [SerializeField] private string defaultLanguage = "en";
        [SerializeField] private LanguageLocalizationTable languageLocalizationTable;

        private void Awake()
        {
            string currentLanguage = GameEngineService.GetCurrentLanguage();
            if (string.IsNullOrWhiteSpace(currentLanguage))
            {
                currentLanguage = defaultLanguage;
            }
            languageLocalizationTable.Initialize(currentLanguage);
        }

        private void OnDestroy()
        {
            languageLocalizationTable.DeInitialize();
        }
    }
}
