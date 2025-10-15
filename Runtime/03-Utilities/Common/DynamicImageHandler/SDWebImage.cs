// 
// SDWebImage.cs
// SDWebImage
//
// Created by Abdalla Tawfik
// Copyright © 2018 RIZMY Studio. All rights reserved.
//

using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace JKTechnologies.SeensioGo.Packages.Unility.Common
{
    [AddComponentMenu("Miscellaneous/SDWebImage")]
    public class SDWebImage : MonoBehaviour
    {

        #region Public Fields

        /// <summary>
        /// Url of a web image to be loaded
        /// </summary>
        public string imageURL;

        /// <summary>
        /// Placeholder Texture to be used while loading the web image
        /// </summary>
        public Texture2D placeholderImage;

        /// <summary>
        /// Sets whether or not the image will preserve its aspect ratio (Image Component only)
        /// </summary>
        public bool preserveAspect = false;

        /// <summary>
        /// Sets whether or not the image will start loading automatically using the inspector image url
        /// </summary>
        public bool autoDownload = true;

        /// <summary>
        /// Sets whether or not the image will be cached in memory
        /// </summary>
        public bool memoryCache = false;

        /// <summary>
        /// Sets whether or not the image will be cached in disk
        /// </summary>
        public bool diskCache = true;

        /// <summary>
        /// Sets whether or not the loading indicator will be shown while loading the web image
        /// </summary>
        public bool showLoadingIndicator = true;

        /// <summary>
        /// Loading indicator object to be shown while loading the web image
        /// </summary>
        public GameObject loadingIndicator;

        /// <summary>
        /// List of the loading indicator types
        /// </summary>
        public enum LoadingIndicatorType { None, RoundedRect, Circle, Circles }

        /// <summary>
        /// Sets loading indicator type
        /// </summary>
        public LoadingIndicatorType loadingIndicatorType;

        /// <summary>
        /// Sets loading indicator scale
        /// </summary>
        public float loadingIndicatorScale = 1;

        /// <summary>
        /// Sets loading indicator color
        /// </summary>
        public Color loadingIndicatorColor = Color.black;

        #endregion


        #region Internal Fields

        private Component _targetComponent;
        private int _targetMaterial = 0;

        #endregion


        #region Delegates

        /// <summary>
        /// Called when the Image is loaded and the texture size is available.
        /// </summary>
        public delegate void OnImageSizeReadyAction(Vector2 size);
        public event OnImageSizeReadyAction OnImageSizeReady;

        /// <summary>
        /// Called when the Image could not be loaded.
        /// </summary>
        public delegate void OnLoadingErrorAction(SDWebImageDownloaderError error);
        public event OnLoadingErrorAction OnLoadingError;

        #endregion


        #region Unity Callbacks

        void Start()
        {
            Init();
            if (autoDownload)
            {
                SetImage();
            }
        }

        #endregion


        #region Public API

        public void Init()
        {
            _targetComponent = GetTargetComponent();
            if (loadingIndicator)
            {
                loadingIndicator.SetActive(false);
            }
        }

        /// <summary>
        /// Sets the target component with a web image of the inspector url
        /// </summary>
        public void SetImage()
        {
            if (!String.IsNullOrEmpty(imageURL))
            {
                SetImageWithURL(imageURL, placeholderImage);
            }
        }

        /// <summary>
        /// Sets the target component with a web image
        /// </summary>
        /// <param name="url">URL of the image to be loaded</param>
        public void SetImageWithURL(string url)
        {
            InternalSetImageWithURL(url, null, GetSDWebImageOptions(), null, SetTexture);
        }

        /// <summary>
        /// Sets the target component with a web image
        /// </summary>
        /// <param name="url">URL of the image to be loaded</param>
        /// <param name="placeholder">Texture to be used during image loading</param>
        public void SetImageWithURL(string url, Texture2D placeholder)
        {
            InternalSetImageWithURL(url, placeholder, GetSDWebImageOptions(), null, SetTexture);
        }

        /// <summary>
        /// Sets the target component with a web image
        /// </summary>
        /// <param name="url">URL of the image to be loaded</param>
        /// <param name="placeholder">Texture to be used during image loading</param>
        /// <param name="options">Custom options for the SDWebImage</param>
        public void SetImageWithURL(string url, Texture2D placeholder, SDWebImageOptions options)
        {
            InternalSetImageWithURL(url, placeholder, options, null, SetTexture);
        }

        /// <summary>
        /// Loads a web image
        /// </summary>
        /// <param name="url">URL of the image to be loaded</param>
        /// <param name="completionCallback">Called when the image is loaded</param>
        public void SetImageWithURL(string url, Action<Texture2D> completionCallback)
        {
            InternalSetImageWithURL(url, null, GetSDWebImageOptions(), null, completionCallback);
        }

        /// <summary>
        /// Sets the target component with a web image
        /// </summary>
        /// <param name="url">URL of the image to be loaded</param>
        /// <param name="placeholder">Texture to be used during image loading</param>
        /// <param name="progressCallback">Called periodically to indicate the dowload progress</param>
        public void SetImageWithURL(string url, Texture2D placeholder, Action<float> progressCallback)
        {
            InternalSetImageWithURL(url, placeholder, GetSDWebImageOptions(), progressCallback, SetTexture);
        }

        /// <summary>
        /// Loads a web image
        /// </summary>
        /// <param name="url">URL of the image to be loaded</param>
        /// <param name="placeholder">Texture to be used during image loading</param>
        /// <param name="completionCallback">Called when the image is loaded</param>
        public void SetImageWithURL(string url, Texture2D placeholder, Action<Texture2D> completionCallback)
        {
            InternalSetImageWithURL(url, placeholder, GetSDWebImageOptions(), null, completionCallback);
        }

        /// <summary>
        /// Sets the target component with a web image
        /// </summary>
        /// <param name="url">URL of the image to be loaded</param>
        /// <param name="placeholder">Texture to be used during image loading</param>
        /// <param name="options">Custom options for the SDWebImage</param>
        /// <param name="progressCallback">Called periodically to indicate the dowload progress</param>
        public void SetImageWithURL(string url, Texture2D placeholder, SDWebImageOptions options, Action<float> progressCallback)
        {
            InternalSetImageWithURL(url, placeholder, options, progressCallback, SetTexture);
        }

        /// <summary>
        /// Loads a web image
        /// </summary>
        /// <param name="url">URL of the image to be loaded</param>
        /// <param name="placeholder">Texture to be used during image loading</param>
        /// <param name="options">Custom options for the SDWebImage</param>
        /// <param name="completionCallback">Called when the image is loaded</param>
        public void SetImageWithURL(string url, Texture2D placeholder, SDWebImageOptions options, Action<Texture2D> completionCallback)
        {
            InternalSetImageWithURL(url, placeholder, options, null, completionCallback);
        }

        /// <summary>
        /// Loads a web image
        /// </summary>
        /// <param name="url">URL of the image to be loaded</param>
        /// <param name="placeholder">Texture to be used during image loading</param>
        /// <param name="progressCallback">Called periodically to indicate the dowload progress</param>
        /// <param name="completionCallback">Called when the image is loaded</param>
        public void SetImageWithURL(string url, Texture2D placeholder, Action<float> progressCallback, Action<Texture2D> completionCallback)
        {
            InternalSetImageWithURL(url, placeholder, GetSDWebImageOptions(), progressCallback, completionCallback);
        }

        /// <summary>
        /// Loads a web image
        /// </summary>
        /// <param name="url">URL of the image to be loaded</param>
        /// <param name="placeholder">Texture to be used during image loading</param>
        /// <param name="options">Custom options for the SDWebImage</param>
        /// <param name="progressCallback">Called periodically to indicate the dowload progress</param>
        /// <param name="completionCallback">Called when the image is loaded</param>
        public void SetImageWithURL(string url, Texture2D placeholder, SDWebImageOptions options, Action<float> progressCallback, Action<Texture2D> completionCallback)
        {
            InternalSetImageWithURL(url, placeholder, options, progressCallback, completionCallback);
        }

        #endregion


        #region Image Controller

        private void InternalSetImageWithURL(string url, Texture2D placeholder, SDWebImageOptions options, Action<float> progressCallback, Action<Texture2D> completionCallback)
        {
            if (placeholder != null)
            {
                if (completionCallback != null)
                {
                    completionCallback(placeholder);
                }
            }

            if (String.IsNullOrEmpty(url))
            {
                Debug.LogWarning("Image url is't set");
                return;
            }


            if ((options & SDWebImageOptions.ShowLoadingIndicator) != 0 && loadingIndicator)
            {
                loadingIndicator.GetComponent<RectTransform>().localPosition = Vector3.zero;
                loadingIndicator.SetActive(true);
            }

            SDWebImageManager.instance.LoadImageWithURL(url, GetInstanceID(), options, progressCallback, (error, imageData) =>
            {
                if ((options & SDWebImageOptions.ShowLoadingIndicator) != 0 && loadingIndicator)
                {
                    loadingIndicator.SetActive(false);
                }

                if (error != null)
                {
                    Debug.LogWarning(error.description);
                    if (OnLoadingError != null)
                    {
                        OnLoadingError(error);
                    }

                    return;
                }

                Texture2D texture = TextureFromData(imageData);

                if (texture != null)
                {
                    if (OnImageSizeReady != null)
                    {
                        OnImageSizeReady(new Vector2(texture.width, texture.height));
                    }

                    if (completionCallback != null)
                    {
                        completionCallback(texture);
                    }
                }
            });

        }

        private Component GetTargetComponent()
        {
            var components = GetComponents<Component>();
            return components.FirstOrDefault(component => component is Renderer || component is RawImage || component is Image);
        }

        private void SetTexture(Texture2D texture)
        {
            if (_targetComponent == null) return;

            // SpriteRenderer
            if (_targetComponent is SpriteRenderer)
            {
                var target = (SpriteRenderer)_targetComponent;
#if UNITY_5_6_OR_NEWER
                var oldSize = target.size;
#endif
                var newSprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect);
                newSprite.name = "Web Image Sprite";
                newSprite.hideFlags = HideFlags.HideAndDontSave;
                target.sprite = newSprite;
#if UNITY_5_6_OR_NEWER
                target.size = oldSize;
#endif
                return;
            }

            // Renderer
            if (_targetComponent is Renderer)
            {
                var target = (Renderer)_targetComponent;
                if (target.sharedMaterial == null) return;
                if (target.sharedMaterials.Length > 0 && target.sharedMaterials.Length > _targetMaterial)
                {
                    target.sharedMaterials[_targetMaterial].mainTexture = texture;
                }
                else
                {
                    target.sharedMaterial.mainTexture = texture;
                }
                return;
            }

            // RawImage
            if (_targetComponent is RawImage)
            {
                var target = (RawImage)_targetComponent;
                target.texture = texture;
                return;
            }

            // Image
            if (_targetComponent is Image)
            {
                var target = (Image)_targetComponent;
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
                target.preserveAspect = preserveAspect;
                target.sprite = sprite;
                return;
            }
        }

        #endregion


        #region Helper Methods

        private SDWebImageOptions GetSDWebImageOptions()
        {
            var options = SDWebImageOptions.None;

            if (memoryCache)
            {
                options |= SDWebImageOptions.MemoryCache;
            }

            if (diskCache)
            {
                options |= SDWebImageOptions.DiskCache;
            }

            if (showLoadingIndicator)
            {
                options |= SDWebImageOptions.ShowLoadingIndicator;
            }

            return options;
        }

        private Texture2D TextureFromData(byte[] data)
        {
            Texture2D texture = new Texture2D(8, 8);
            texture.LoadImage(data);

            return texture == null || (texture.width == 8 && texture.height == 8) ? null : texture;
        }

        #endregion

    }
}