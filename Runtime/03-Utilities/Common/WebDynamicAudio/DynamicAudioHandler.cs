using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace JKTechnologies.SeensioGo.Packages.Unility.Common
{
    public class DynamicAudioHandler : MonoBehaviour
    {   
        [SerializeField] private string url;
        [SerializeField] private bool keep;
        public AudioSource audioSource;
        public bool loaded = false;
        public bool isCanceled = false;
        public bool cachedAudio = false;
        public bool isDestroyed = false;

        // Start is called before the first frame update
        private void Start()
        {
            audioSource = gameObject.GetComponent<AudioSource>();
            if (audioSource == null)
            {
                gameObject.AddComponent<AudioSource>();
                audioSource = gameObject.GetComponent<AudioSource>();
            }  
            
        }

        private async void OnEnable()
        {
            if (audioSource == null)
            {
                audioSource = gameObject.GetComponent<AudioSource>();
            }  

            if (isCanceled)
            {
                loaded = false;
            }

            if (DynamicAudioManager.Instance == null)
            {
                // Debug.LogError("Holdon");
                await Task.Delay(100);
            }

            if (!loaded && !string.IsNullOrEmpty(url))
            {
                // Debug.LogError("Dynamic Audio Handler on Enabled " + gameObject.name);
                LoadAudio(url, keep, cachedAudio);
            }
        }

        private void OnDisable()
        {
            if(!loaded)
            {
                isCanceled = true;
            }
            else
            {
                isCanceled = false;
            }
        }
        
        public void Clear()
        {
            audioSource.clip = null;

            if (!cachedAudio && DynamicAudioManager.Instance != null && !string.IsNullOrEmpty(url))
            {
                DynamicAudioManager.Instance.Destroy(url);
            }

            // Reset flags
            loaded = false;
            isCanceled = false;
            url = null;
        }

        
        public void LoadAudio(string _audioUrl, bool _keep = false, bool _cachedAudio = false)
        {
            if (!string.IsNullOrEmpty(_audioUrl))
            {
                url = _audioUrl;
                keep = _keep;
                cachedAudio = _cachedAudio;

                if (!isDestroyed && this.gameObject.activeInHierarchy)
                {
                    var existingAudioClip = DynamicAudioManager.Instance.Find(url);
                    if (existingAudioClip != null)
                    {
                        if (this != null && gameObject != null)
                        {
                            // Debug.LogError("GameObject " + gameObject.name);
                            // Debug.LogError(audioSource);
                            audioSource.clip = existingAudioClip;
                            if (audioSource.playOnAwake)
                            {
                                audioSource.Play();    
                            }
                            
                        }
                        
                        loaded = true;
                    }
                    else
                    {

                        var options = WebAudioOptions.DiskCache;
                        WebAudioManager.instance.LoadAudioWithURL(url, GetInstanceID(), options,
                        (progress) =>
                        {
                            if (isDestroyed)
                            {
                                return;
                            }
                            
                            if (isDestroyed)
                            {
                                return;
                            }
                        },
                        async (error, audioClip) =>
                        {
                            if (isDestroyed)
                            {
                                return;
                            }
                            if (error != null)
                            {
                                Debug.LogWarning(error.description);                                
                                return;
                            }

                            if (this != null && !string.IsNullOrEmpty(_audioUrl) && audioSource != null && gameObject != null)
                            { 
                                AudioClip audioClipObj = DynamicAudioManager.Instance.CreateAudioClip(url, audioClip, keep);
                                audioSource.clip = audioClipObj;
                                if (audioSource.playOnAwake)
                                {
                                    audioSource.Play();
                                }                                    
                            }                            

                            if (isDestroyed)
                            {
                                return;
                            }
                            
                            loaded = true;
                        });
                    }

                }
                else
                {
                    loaded = false;
                }

            }
            else
            {
                loaded = false;
            }
        }

        protected virtual void OnDestroy()
        {
            isDestroyed = true;
            audioSource.clip = null;
            if (!cachedAudio && DynamicAudioManager.Instance != null)
            {
                DynamicAudioManager.Instance.Destroy(url);
            }
        }
    }
}
