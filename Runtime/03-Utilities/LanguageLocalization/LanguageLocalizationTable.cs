using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace JKTechnologies.CommonPackage.LanguageLocalizations
{
    [CreateAssetMenu(fileName = "LanguageLocalizationTable", menuName = "JKTechnologies/CommonPackages/LanguageLocalizationTable", order = 0)]
    public class LanguageLocalizationTable : ScriptableObject
    {
        public static LanguageLocalizationTable Instance { get; private set; }
        public static bool IsInitialized = false;
        public static UnityEvent OnInitialized = new();
        public UnityEvent OnLanguageChanged;
        [SerializeField] private string currentLanguage;
        [SerializeField] private string onNotFound;
        [SerializeField] List<LanguageObject> languageObjects;
        private Dictionary<string, LanguageObject> languageObjectDict;

        #region Init
        public void Initialize(string lang)
        {
            this.currentLanguage = lang;
            languageObjectDict = languageObjects.ToDictionary(item => item.Key, item => item);
            OnInitialized.Invoke();
            IsInitialized = true;
            Instance = this;
        }

        public void DeInitialize()
        {
            Instance = null;
            IsInitialized = false;
        }
        #endregion

        #region Set Language
        public void SetLanguage(string language)
        {
            currentLanguage = language;
            OnLanguageChanged.Invoke();
        }
        #endregion

        #region Get Value
        public string GetValue(string key)
        {
            if (!languageObjectDict.TryGetValue(key, out var languageObject))
            {
                return onNotFound;
            }
            var result = languageObject.TextValues.FirstOrDefault(item => item.Language == currentLanguage);
            if (result is null)
            {
                return onNotFound;
            }
            return result.Value;
        }
        #endregion
    }
}