using UnityEngine;

namespace JKTechnologies.CommonPackage.LanguageLocalizations
{
    public class LanguageLocalizationInitializer : MonoBehaviour
    {
        [SerializeField] private string currentLanguage = "en";
        [SerializeField] private LanguageLocalizationTable languageLocalizationTable;

        private void Awake()
        {
            languageLocalizationTable.Initialize(currentLanguage);
        }

        private void OnDestroy()
        {
            languageLocalizationTable.DeInitialize();
        }
    }
}
