using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace JKTechnologies.SeensioGo.Packages.Unility.Common
{
    [System.Serializable]
    public class DynamicTexture
    {
        public string url;
        public int width;
        public int height;
        public Object texture;
        public bool keep;
    }

    public class DynamicTextureManager : MonoBehaviour 
    {
        public static DynamicTextureManager Instance;
        public List<DynamicTexture> loadedTextures;

        void Awake()
        {
            if(Instance != null && Instance!= this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                loadedTextures = new List<DynamicTexture>();
                DontDestroyOnLoad(gameObject);
            }
        }
        
        /// <summary>
        /// Call this from some MonoBehaviour in OnDisable to destroy all objects.
        /// </summary>
        public void OnDisable() 
        {
            if(Instance != null && Instance== this)
            {
                Clear();
            }
        }
 
        public void Clear() {
            // Destroy all temp objects in the manager
            for(int i = 0; i < loadedTextures.Count; i++) {
                if(loadedTextures[i] == null) continue;
                Object.Destroy(loadedTextures[i].texture);
            }
            loadedTextures.Clear(); // clear the list
        }
 
        /// <summary>
        /// Adds a temp object to the manager.
        /// </summary>
        /// <param name="obj">Texture</param>
        public void Add(DynamicTexture obj) {
            if(obj == null) return;
            if(loadedTextures.Contains(obj)) return; // already in the list
            loadedTextures.Add(obj); // add to list
        }
 
        /// <summary>
        /// Destroys the object and removes it from the manager.
        /// </summary>
        /// <param name="obj">Object</param>
        public void Destroy(DynamicTexture obj) {
            if(obj == null) return;
            loadedTextures.Remove(obj); // remove from list
            Object.Destroy(obj.texture); // destroy the object
        }

        public void Destroy(string url) {
            if(string.IsNullOrEmpty(url)) return;
            var found = loadedTextures.FirstOrDefault(obj => obj.url == url);
            if(found !=null && !found.keep){
                loadedTextures.Remove(found); // remove from list
                Object.Destroy(found.texture); // destroy the object
            }            
        }

        public void Destroy(string url, int width, int height) {
            if(string.IsNullOrEmpty(url)) return;
            var found = loadedTextures.FirstOrDefault(obj => (obj.url == url && obj.width == width && obj.height == height));
            if(found !=null && !found.keep){
                loadedTextures.Remove(found); // remove from list
                Object.Destroy(found.texture); // destroy the object
            }            
        }

        public Texture2D Find(string url){
            if(string.IsNullOrEmpty(url)) return null;
            var found = loadedTextures.FirstOrDefault(obj => obj.url == url);
            if(found !=null){
                return (Texture2D)found.texture;
            } else{
                return null;
            }
        }

        public Texture2D Find(string url, int width, int height){
            if(string.IsNullOrEmpty(url)) return null;
            var found = loadedTextures.FirstOrDefault(obj => (obj.url == url && obj.width == width && obj.height == height));
            if(found !=null){
                return (Texture2D)found.texture;
            } else{
                return null;
            }
        }
 
        /// <summary>
        /// Creates a temporary texture and stores it in the manager.
        /// </summary>
        /// <param name="width">Width of the texture</param>
        /// <param name="height">Height of the texture</param>
        /// <returns>Texture2D</returns>
        public Texture2D CreateTexture2D(int width, int height, string url, bool keep = false) {
            var existing = Find(url, width, height);
            if(existing){
                return existing;
            }else{
                Texture2D tex = new Texture2D(width, height);
                tex.name = FileHelper.GetFileNameFromUrl(url);
                tex.hideFlags = HideFlags.HideAndDontSave;
                var dynamicTexture = new DynamicTexture();
                dynamicTexture.url = url;
                dynamicTexture.width = width;
                dynamicTexture.height = height;
                dynamicTexture.texture = tex;
                dynamicTexture.keep = keep;

                loadedTextures.Add(dynamicTexture);
                return tex;
            }
            
        }
    }
}