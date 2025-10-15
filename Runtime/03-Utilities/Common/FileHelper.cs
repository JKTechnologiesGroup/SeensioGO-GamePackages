using System.IO;
using UnityEngine;

namespace JKTechnologies.CommonPackage.Utilities
{
    public class FileHelper: MonoBehaviour
    {
        public static string GetFileNameFromUrl(string url){
            if (string.IsNullOrWhiteSpace(url))
                return "";
                
            string name = url;
            if(url.IndexOf("%2F") > -1){
                name = url.Substring(url.IndexOf("%2F")+ 3);
            }
			
            if(name.IndexOf("?") > -1){
                name = name.Substring(0, name.LastIndexOf("?"));
            }
            
            name = name.Replace("%2F", "_");
            name = System.Text.RegularExpressions.Regex.Replace(name, "[^\\w\\._]", "_");

            // Limit name to 60 chars
            if(name.Length > 60){
                // Debug.LogError("Name Length " + name.Length);
                name = name.Substring(name.Length - 60);
            }			

			if(string.IsNullOrEmpty(name)){
				return (UnityEngine.Random.Range(1, 10000)).ToString();
			}else{
				return name;
			}
		}

        public static string GetTileFileNameFromUrl(string url){
            string name = url;
            if(url.IndexOf("%2F") > -1){
                name = url.Substring(url.LastIndexOf("%2F")+ 3);
            }
			
            if(name.IndexOf("?") > -1){
                name = name.Substring(0, name.LastIndexOf("?"));
            }
            
            name = name.Replace("%2F", "_");
            name = System.Text.RegularExpressions.Regex.Replace(name, "[^\\w\\._]", "_");

            // Limit name to 60 chars
            if(name.Length > 60){
                // Debug.LogError("Name Length " + name.Length);
                name = name.Substring(name.Length - 60);
            }			

			if(string.IsNullOrEmpty(name)){
				return (UnityEngine.Random.Range(1, 10000)).ToString();
			}else{
				return name;
			}
		}

        public static string GetObjectNameFromUrl(string url){
			string name = url.Substring(url.LastIndexOf("%2F")+ 3);
            name = name.Substring(0, name.LastIndexOf("?"));
            name = name.Substring(0, name.LastIndexOf("."));
            name = name.Replace("%2F", "_");
            name = System.Text.RegularExpressions.Regex.Replace(name, "[^\\w\\._]", "_");

            // Limit name to 60 chars
            if(name.Length > 60){
                // Debug.LogError("Name Length " + name.Length);
                name = name.Substring(name.Length - 60);
            }			

			if(string.IsNullOrEmpty(name)){
				return (UnityEngine.Random.Range(1, 10000)).ToString();
			}else{
				return name;
			}
		}

        public static string GetFileName(string filePath)
        {
            return Path.GetFileName(filePath);
        }
    }

    
}
