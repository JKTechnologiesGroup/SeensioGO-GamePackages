using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace JKTechnologies.CommonPackage.Utilities
{
    [System.Serializable]
    public class DynamicAudioClip
    {
        public string url;
        public AudioClip audioClip;
        public bool keep;
    }

    public class DynamicAudioManager : MonoBehaviour 
    {
        public static DynamicAudioManager Instance;
        public List<DynamicAudioClip> loadedAudioClips;

        void Awake()
        {
            if(Instance != null && Instance!= this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                loadedAudioClips = new List<DynamicAudioClip>();
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
            for(int i = 0; i < loadedAudioClips.Count; i++) {
                if(loadedAudioClips[i] == null) continue;
                Object.Destroy(loadedAudioClips[i].audioClip);
            }
            loadedAudioClips.Clear(); // clear the list
        }
 
        /// <summary>
        /// Adds a temp object to the manager.
        /// </summary>
        /// <param name="obj">Texture</param>
        public void Add(DynamicAudioClip obj) {
            if(obj == null) return;
            if(loadedAudioClips.Contains(obj)) return; // already in the list
            loadedAudioClips.Add(obj); // add to list
        }
 
        /// <summary>
        /// Destroys the object and removes it from the manager.
        /// </summary>
        /// <param name="obj">Object</param>
        public void Destroy(DynamicAudioClip obj) {
            if(obj == null) return;
            loadedAudioClips.Remove(obj); // remove from list
            Object.Destroy(obj.audioClip); // destroy the object
        }

        public void Destroy(string url) {
            if(string.IsNullOrEmpty(url)) return;
            var found = loadedAudioClips.FirstOrDefault(obj => obj.url == url);
            if(found !=null && !found.keep){
                loadedAudioClips.Remove(found); // remove from list
                Object.Destroy(found.audioClip); // destroy the object
            }            
        }

        public AudioClip Find(string url){
            if(string.IsNullOrEmpty(url)) return null;
            var found = loadedAudioClips.FirstOrDefault(obj => obj.url == url);
            if(found !=null){
                return (AudioClip)found.audioClip;
            } else{
                return null;
            }
        }        
 
        /// <summary>
        /// Creates a temporary audioclip and stores it in the manager.
        /// </summary>
        /// <param name="width">Width of the texture</param>
        /// <param name="height">Height of the texture</param>
        /// <returns>AudioClip</returns>
        public AudioClip CreateAudioClip(string url, AudioClip clip, bool keep = false) {
            var existing = Find(url);
            if (existing)
            {
                return existing;
            }
            else
            {
                clip.name = FileHelper.GetFileNameFromUrl(url);
                var dynamicAudioClip = new DynamicAudioClip();
                dynamicAudioClip.url = url;
                dynamicAudioClip.audioClip = clip;
                dynamicAudioClip.keep = keep;

                loadedAudioClips.Add(dynamicAudioClip);
                return clip;
            }
            
        }
    }
}