using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace JKTechnologies.CommonPackage.Utilities
{
    [AddComponentMenu("Miscellaneous/SDAnimatedImage")]
    public class SDAnimatedImage : MonoBehaviour
    {

        public enum State { None, PreDecoding, Decoding, DecodingError, Playing, Paused }

        #region Public Fields

        /// <summary>
        /// Current state of the animated image player
        /// </summary>
        public State state { get; private set; }

        /// <summary>
        /// Url of a web animated image to be loaded
        /// </summary>
        public string imageURL;

        /// <summary>
        /// Placeholder Texture to be used while loading the web animated image
        /// </summary>
        public Texture2D placeholderImage;

        /// <summary>
        /// Sets whether or not the animated image will preserve its aspect ratio (Image Component only)
        /// </summary>
        public bool preserveAspect = false;

        /// <summary>
        /// Sets whether or not the animated image will start loading automatically using the inspector image url
        /// </summary>
        public bool autoDownload = true;

        /// <summary>
        /// Sets whether or not the animated image will start playing automatically after it is loaded
        /// </summary>
        public bool autoPlay = true;

        /// <summary>
        /// Sets whether or not the animated image will never stop playing
        /// </summary>
        public bool loop = true;

        /// <summary>
        /// Sets whether or not the animated image will be cached in memory
        /// </summary>
        public bool memoryCache = false;

        /// <summary>
        /// Sets whether or not the animated image will be cached in disk
        /// </summary>
        public bool diskCache = true;

        /// <summary>
        /// Sets whether or not the loading indicator will be shown while loading the web animated image
        /// </summary>
        public bool showLoadingIndicator = true;

        /// <summary>
        /// Loading indicator object to be shown while loading the web animated image
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


        #region Internal fields

        private Texture2D animatedImageTexture;
        private Component _targetComponent;
        private int _targetMaterial = 0;

        private SeensioGifDecoder decoder;
        private bool firstFrameShown;

        private List<SeensioGifDecoder.GifFrame> framesCache;
        private SeensioGifDecoder.GifFrame currentFrame;
        private bool frameIsReady;
        private int currentFrameIndex;
        private float currentFrameRemainingTime;

        private readonly object locker = new object();
        private Thread decoderThread;
        private bool threadIsRunning;
        private readonly AutoResetEvent autoResetEvent = new AutoResetEvent(false);

        #endregion


        #region Delegates

        /// <summary>
        /// Called when the Animated Image is loaded and the texture size is available.
        /// </summary>
        public delegate void OnImageSizeReadyAction(Vector2 size);
        public event OnImageSizeReadyAction OnImageSizeReady;

        /// <summary>
        /// Called when the Animated Image could not be decoded.
        /// </summary>
        public delegate void OnDecodingErrorAction();
        public event OnDecodingErrorAction OnDecodingError;

        /// <summary>
        /// Called when the Animated Image could not be loaded.
        /// </summary>
        public delegate void OnLoadingErrorAction(SDWebImageDownloaderError error);
        public event OnLoadingErrorAction OnLoadingError;

        #endregion


        #region Unity Callbacks

        void Start()
        {
            if (autoDownload)
            {
                SetAnimatedImage();
            }
        }

        void Update()
        {
            TrackAnimatedImageDecoding();
        }

        void OnApplicationQuit()
        {
            TerminateDecoderThread();
        }

        #endregion


        #region Public API

        /// <summary>
        /// Sets the target component with a web image of the inspector url
        /// </summary>
        public void SetAnimatedImage()
        {
            if (!String.IsNullOrEmpty(imageURL))
            {
                SetAnimatedImageWithURL(imageURL, placeholderImage);
            }
        }

        /// <summary>
        /// Sets the target component with a web image
        /// </summary>
        /// <param name="url">URL of the image to be loaded</param>
        public void SetAnimatedImageWithURL(string url)
        {
            InternalSetAnimatedImageWithURL(url, null, GetSDWebImageOptions(), null);
        }

        /// <summary>
        /// Sets the target component with a web image
        /// </summary>
        /// <param name="url">URL of the image to be loaded</param>
        /// <param name="placeholder">Texture to be used during image loading</param>
        public void SetAnimatedImageWithURL(string url, Texture2D placeholder)
        {
            InternalSetAnimatedImageWithURL(url, placeholder, GetSDWebImageOptions(), null);
        }

        /// <summary>
        /// Sets the target component with a web image
        /// </summary>
        /// <param name="url">URL of the image to be loaded</param>
        /// <param name="placeholder">Texture to be used during image loading</param>
        /// <param name="options">Custom options for the SDWebImage</param>
        public void SetAnimatedImageWithURL(string url, Texture2D placeholder, SDWebImageOptions options)
        {
            InternalSetAnimatedImageWithURL(url, placeholder, options, null);
        }

        /// <summary>
        /// Sets the target component with a web image
        /// </summary>
        /// <param name="url">URL of the image to be loaded</param>
        /// <param name="placeholder">Texture to be used during image loading</param>
        /// <param name="progressCallback">Called periodically to indicate the dowload progress</param>
        public void SetAnimatedImageWithURL(string url, Texture2D placeholder, Action<float> progressCallback)
        {
            InternalSetAnimatedImageWithURL(url, placeholder, GetSDWebImageOptions(), progressCallback);
        }

        /// <summary>
        /// Sets the target component with a web image
        /// </summary>
        /// <param name="url">URL of the image to be loaded</param>
        /// <param name="placeholder">Texture to be used during image loading</param>
        /// <param name="options">Custom options for the SDWebImage</param>
        /// <param name="progressCallback">Called periodically to indicate the dowload progress</param>
        public void SetAnimatedImageWithURL(string url, Texture2D placeholder, SDWebImageOptions options, Action<float> progressCallback)
        {
            InternalSetAnimatedImageWithURL(url, placeholder, options, progressCallback);
        }

        /// <summary>
        /// Starts Animated Image playback.
        /// </summary>
        public void Play()
        {
            if (state == State.Paused)
            {
                state = State.Playing;
            }
        }

        /// <summary>
        /// Pauses Animated Image playback.
        /// </summary>
        public void Pause()
        {
            if (state == State.Playing)
            {
                state = State.Paused;
            }
        }

        #endregion


        #region Animated Image Player

        private void CacheGifFrame(SeensioGifDecoder.GifFrame frame)
        {
            var imageCopy = new byte[frame.data.Length];
            Buffer.BlockCopy(frame.data, 0, imageCopy, 0, frame.data.Length);
            frame.data = imageCopy;

            lock (framesCache)
            {
                framesCache.Add(frame);
            }
        }

        private void StartDecoder()
        {
            ReadNextFrame();
            state = State.Paused;

            if (OnImageSizeReady != null)
            {
                OnImageSizeReady(new Vector2(decoder.GetImageWidth(), decoder.GetImageHeight()));
            }

            if (autoPlay)
            {
                Play();
            }
        }

        private void UpdateFrameTime()
        {
            if (state != State.Playing)
            {
                return;
            }

            currentFrameRemainingTime -= Time.deltaTime;
        }

        private void UpdateFrame()
        {
            if (decoder.TotalNumberOfFrames > 0 && decoder.TotalNumberOfFrames == currentFrameIndex)
            {
                currentFrameIndex = 0;

                if (!loop)
                {
                    Pause();
                    return;
                }
            }

            if (loop)
            {
                lock (framesCache)
                {
                    currentFrame = framesCache.Count > currentFrameIndex ? framesCache[currentFrameIndex] : decoder.GetCurrentFrame();
                }

                if (!decoder.AllFramesDecoded)
                {
                    ReadNextFrame();
                }
            }
            else
            {
                currentFrame = decoder.GetCurrentFrame();
            }

            UpdateTexture();
            currentFrameRemainingTime = currentFrame.delay;
            currentFrameIndex++;

            if (!loop)
            {
                ReadNextFrame();
            }
        }

        private void TrackAnimatedImageDecoding()
        {
            if (state != State.Playing && firstFrameShown || !frameIsReady)
            {
                return;
            }

            if (state == State.Decoding)
            {
                return;
            }

            if (!firstFrameShown)
            {
                SetTexture();
                lock (locker)
                {
                    UpdateFrame();
                }

                firstFrameShown = true;
                return;
            }

            UpdateFrameTime();
            if (currentFrameRemainingTime > 0) return;

            lock (locker)
            {
                UpdateFrame();
            }
        }

        private void UpdateTexture()
        {
            animatedImageTexture.LoadRawTextureData(currentFrame.data);
            animatedImageTexture.Apply();
        }

        private void ReadNextFrame()
        {
            frameIsReady = false;
            autoResetEvent.Set();
        }

        private void StartDecoderThread()
        {
            if (decoderThread != null)
            {
                return;
            }

            threadIsRunning = true;
            decoderThread = new Thread(DecodeAnimatedImageData);
            decoderThread.Name = "Gif_Decoder_" + decoderThread.ManagedThreadId;
            decoderThread.IsBackground = true;
            decoderThread.Start();
        }

        private void TerminateDecoderThread()
        {
            if (!threadIsRunning)
            {
                return;
            }

            threadIsRunning = false;
            autoResetEvent.Set();
        }

        private void DecodeAnimatedImageData()
        {
            autoResetEvent.WaitOne();

            while (threadIsRunning)
            {
                lock (locker)
                {
                    decoder.ReadContents(!loop);

                    if (loop && decoder.AllFramesDecoded)
                    {
                        frameIsReady = true;
                        break;
                    }

                    if (loop)
                    {
                        CacheGifFrame(decoder.GetCurrentFrame());
                    }

                    frameIsReady = true;
                }

                autoResetEvent.WaitOne();
            }

            threadIsRunning = false;
            decoderThread = null;
        }

        #endregion


        #region Image Controller

        private void Init()
        {
            _targetComponent = GetTargetComponent();

            decoder = new SeensioGifDecoder();

            currentFrameIndex = 0;
            firstFrameShown = false;
            frameIsReady = false;

            StartDecoderThread();

            if (loadingIndicator)
            {
                loadingIndicator.SetActive(false);
            }

            if (loop)
            {
                framesCache = new List<SeensioGifDecoder.GifFrame>();
            }
        }

        private void InternalSetAnimatedImageWithURL(string url, Texture2D placeholder, SDWebImageOptions options, Action<float> progressCallback)
        {
            if (placeholder != null)
            {
                animatedImageTexture = placeholder;
                SetTexture();
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

                lock (locker)
                {
                    Init();
                    if (decoder.Read(new System.IO.MemoryStream(imageData)) == SeensioGifDecoder.Status.Ok)
                    {
                        state = State.PreDecoding;
                        CreateTargetTexture();
                        StartDecoder();
                    }
                    else
                    {
                        state = State.DecodingError;
                        Debug.LogWarning("Error decoding gif");
                        if (OnDecodingError != null)
                        {
                            OnDecodingError();
                        }
                    }
                }
            });
        }

        private Component GetTargetComponent()
        {
            var components = GetComponents<Component>();
            return components.FirstOrDefault(component => component is Renderer || component is RawImage || component is Image);
        }

        private void SetTexture()
        {
            if (_targetComponent == null) return;

            // SpriteRenderer
            if (_targetComponent is SpriteRenderer)
            {
                var target = (SpriteRenderer)_targetComponent;
#if UNITY_5_6_OR_NEWER
                var oldSize = target.size;
#endif
                var newSprite = Sprite.Create(animatedImageTexture, new Rect(0.0f, 0.0f, animatedImageTexture.width, animatedImageTexture.height), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect);
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
                    target.sharedMaterials[_targetMaterial].mainTexture = animatedImageTexture;
                }
                else
                {
                    target.sharedMaterial.mainTexture = animatedImageTexture;
                }
                return;
            }

            // RawImage
            if (_targetComponent is RawImage)
            {
                var target = (RawImage)_targetComponent;
                target.texture = animatedImageTexture;
                return;
            }

            // Image
            if (_targetComponent is Image)
            {
                var target = (Image)_targetComponent;
                Sprite sprite = Sprite.Create(animatedImageTexture, new Rect(0, 0, animatedImageTexture.width, animatedImageTexture.height), new Vector2(0, 0));
                target.preserveAspect = preserveAspect;
                target.sprite = sprite;
                return;
            }
        }

        private void CreateTargetTexture()
        {
            if (animatedImageTexture != null && decoder != null && animatedImageTexture.width == decoder.GetImageWidth() && animatedImageTexture.height == decoder.GetImageHeight()) return; // Target texture already set

            if (decoder == null || decoder.GetImageWidth() == 0 || decoder.GetImageWidth() == 0)
            {
                animatedImageTexture = Texture2D.blackTexture;
                return;
            }

            if (animatedImageTexture != null && animatedImageTexture.hideFlags == HideFlags.HideAndDontSave) DestroyImmediate(animatedImageTexture);

            animatedImageTexture = CreateTexture(decoder.GetImageWidth(), decoder.GetImageHeight());
            animatedImageTexture.hideFlags = HideFlags.HideAndDontSave;
        }

        private static Texture2D CreateTexture(int width, int height)
        {
            return new Texture2D(width, height, TextureFormat.RGBA32, false);
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

        #endregion

    }
}