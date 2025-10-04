using System;
using System.Collections.Generic;

namespace JKTechnologies.CommonPackage.LanguageLocalizations
{
    [Serializable]
    public class LanguageObject
    {
        public string Key;
        public List<LanguageTextValue> TextValues;
    }
}