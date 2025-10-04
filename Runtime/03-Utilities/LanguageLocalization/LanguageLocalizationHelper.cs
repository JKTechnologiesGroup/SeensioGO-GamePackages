namespace JKTechnologies.CommonPackage.LanguageLocalizations
{
    public static class LanguageLocalizationHelper
    {
        public static string LookupLanguage(string key)
        {
            return LanguageLocalizationTable.Instance?.GetValue(key);
        }
    }
}