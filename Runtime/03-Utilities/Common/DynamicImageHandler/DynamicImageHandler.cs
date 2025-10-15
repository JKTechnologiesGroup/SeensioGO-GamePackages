using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace JKTechnologies.SeensioGo.Packages.Unility.Common
{
    public class DynamicImageHandler : MonoBehaviour
    {   
        private int width;
        private int height;
        [SerializeField] private string url;
        [SerializeField] private bool keep;
        [SerializeField] private int maxSize;
        [SerializeField] private Animator fadeIn;

        public Image m_image;
        public Sprite m_sprite;
        public Sprite defaultSprite;
        public GameObject photoLoading;
        public bool loaded = false;
        public bool isCanceled = false;
        public bool cachedImage = false;
        public bool isDestroyed = false;

        // Start is called before the first frame update
        private void Start()
        {
            m_image = gameObject.GetComponent<Image>();
            if(photoLoading == null)
            {
                if(gameObject.transform.Find("PhotoLoading")){
                    photoLoading = gameObject.transform.Find("PhotoLoading").gameObject;
                }
            }
        }


        private async void OnEnable()
        {
            if(isCanceled)
            {
                loaded = false;
            }

            if(DynamicTextureManager.Instance == null){
                // Debug.LogError("Holdon");
                await Task.Delay(100);
            }

            if(!loaded && !string.IsNullOrEmpty(url)){
                // Debug.LogError("Image Handler on Enabled " + gameObject.name);
                LoadPhoto(url, maxSize, keep, cachedImage);
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
        
        public void ClearImage(bool destroyTexture = false)
        {
            // Reset the Image component
            if (m_image != null)
            {
                if (defaultSprite != null)
                {
                    m_image.sprite = defaultSprite;
                }
                else
                {
                    m_image.sprite = null;
                }
            }

            // Destroy current sprite if it exists
            if (m_sprite != null)
            {
                Destroy(m_sprite);
                m_sprite = null;
            }

            // Optionally clear from DynamicTextureManager if not cached
            if (destroyTexture && !cachedImage && DynamicTextureManager.Instance != null && !string.IsNullOrEmpty(url))
            {
                DynamicTextureManager.Instance.Destroy(url, width, height);
            }

            // Reset flags
            loaded = false;
            isCanceled = false;
            url = null;
        }

        
        public async void LoadPhoto(string _imageUrl, int _maxSize = 1024, bool _keep = false, bool _cachedImage = false)
        {
            if (!string.IsNullOrEmpty(_imageUrl))
            {
                url = _imageUrl;
                maxSize = _maxSize;
                keep = _keep;
                cachedImage = _cachedImage;

                if (!isDestroyed && this.gameObject.activeInHierarchy)
                {
                    // Debug.LogError("Image Handler active in hierarchy" + gameObject.name);
                    // var options = SDWebImageOptions.None;
                    // options |= SDWebImageOptions.DiskCache;
                    // Try to find existing texture with the same url first
                    var existingTexture = DynamicTextureManager.Instance.Find(url);
                    if (existingTexture != null)
                    {
                        int w = maxSize;
                        int h = maxSize;
                        if (existingTexture.width >= existingTexture.height && existingTexture.width > w)
                        {
                            h = (int)(w * existingTexture.height / existingTexture.width);
                        }
                        else if (existingTexture.height > existingTexture.width && existingTexture.height > h)
                        {
                            w = (int)(h * existingTexture.width / existingTexture.height);
                        }
                        else
                        {
                            w = existingTexture.width;
                            h = existingTexture.height;
                        }

                        width = w;
                        height = h;
                        // Debug.LogError("Case exising textures " + width + "-" + height);        
                        if (this != null && gameObject != null)
                        { // Make sure we dont continue if object is already destroyed
                            if (m_image == null)
                            {
                                m_image = gameObject.GetComponent<Image>();
                            }

                            if (existingTexture.width != w || existingTexture.height != h)
                            {
                                // Debug.LogError("Case 1 " + existingTexture.width + "-" + existingTexture.height);
                                var existingTextureCorrectSize = DynamicTextureManager.Instance.Find(url, w, h);
                                if (existingTextureCorrectSize != null)
                                {
                                    // Debug.LogError("Case 2");
                                    ResizeTextureAndApply(existingTextureCorrectSize, false, false);
                                }
                                else
                                {
                                    // Debug.LogError("Case 3");
                                    ResizeTextureAndApply(existingTexture, false);
                                }
                            }
                            else
                            {
                                // Debug.LogError("Case 4");
                                ResizeTextureAndApply(existingTexture, false, false);
                            }
                        }

                        if (photoLoading)
                        {
                            photoLoading.GetComponent<CanvasGroup>().alpha = 0.0f;
                        }

                        if (fadeIn != null)
                        {
                            fadeIn.Play("In");
                            await Task.Delay(200);
                            fadeIn.enabled = false;
                        }
                        CanvasGroup canvasGroup = gameObject.GetComponent<CanvasGroup>();
                        if (canvasGroup != null)
                        {
                            canvasGroup.alpha = 1;
                        }
                        loaded = true;
                    }
                    else
                    {

                        var options = SDWebImageOptions.DiskCache;
                        SDWebImageManager.instance.LoadImageWithURL(url, GetInstanceID(), options,
                        (progress) =>
                        {
                            if (isDestroyed)
                            {
                                return;
                            }
                            // Debug.LogError("Downloading image " + imageUrl); 
                            if (photoLoading)
                            {
                                photoLoading.GetComponent<CanvasGroup>().alpha = 1.0f;
                            }
                            if (isDestroyed)
                            {
                                return;
                            }
                        },
                        async (error, imageData) =>
                        {
                            if (isDestroyed)
                            {
                                return;
                            }
                            if (error != null)
                            {
                                Debug.LogWarning(error.description);
                                if (photoLoading)
                                {
                                    photoLoading.GetComponent<CanvasGroup>().alpha = 0.0f;
                                }
                                return;
                            }

                            Texture2D m_texture = new Texture2D(128, 128);
                            m_texture.LoadImage(imageData);
                            int w = maxSize;
                            int h = maxSize;
                            if (m_texture.width >= m_texture.height && m_texture.width > w)
                            {
                                h = (int)(w * m_texture.height / m_texture.width);
                            }
                            else if (m_texture.height > m_texture.width && m_texture.height > h)
                            {
                                w = (int)(h * m_texture.width / m_texture.height);
                            }
                            else
                            {
                                w = m_texture.width;
                                h = m_texture.height;
                            }

                            width = w;
                            height = h;

                            if (this != null && !string.IsNullOrEmpty(_imageUrl) && m_texture != null && gameObject != null)
                            { // Make sure we dont continue if object is already destroyed
                                if (m_image == null)
                                {
                                    m_image = gameObject.GetComponent<Image>();
                                }
                                ResizeTextureAndApply(m_texture);
                            }

                            if (photoLoading)
                            {
                                photoLoading.GetComponent<CanvasGroup>().alpha = 0.0f;
                            }

                            if (fadeIn != null)
                            {
                                fadeIn.Play("In");
                                await Task.Delay(200);
                                if (fadeIn != null)
                                {
                                    fadeIn.enabled = false;
                                }
                            }

                            if (isDestroyed)
                            {
                                return;
                            }
                            CanvasGroup canvasGroup = gameObject?.GetComponent<CanvasGroup>();
                            if (canvasGroup != null)
                            {
                                canvasGroup.alpha = 1;
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

        private void ResizeTextureAndApply(Texture2D original, bool destoyOriginal = true, bool resize = true){
            if(resize){
                RenderTexture rt = new RenderTexture(width, height, 24);
                RenderTexture.active = rt;
                Graphics.Blit(original, rt);
                Texture2D resizedTexture = DynamicTextureManager.Instance.CreateTexture2D(width, height, url, keep);
                resizedTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
                resizedTexture.Apply();
                
                // Try to fill an ButtonIcon first
                if(m_sprite!=null){
                    m_sprite = Sprite.Create(resizedTexture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));                        
                }else if(m_image!=null){
                    // Debug.LogError("Load image texture " + imageUrl); 
                    // Try to fill an Image Object
                    m_image.sprite = Sprite.Create(resizedTexture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));                        
                    // m_image.color = Color.white;
                }                      
            }else{
                if(m_sprite!=null){
                    m_sprite = Sprite.Create(original, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));                        
                }else if(m_image!=null){
                    // Debug.LogError("Load image texture " + imageUrl); 
                    // Try to fill an Image Object
                    m_image.sprite = Sprite.Create(original, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));                        
                    // m_image.color = Color.white;
                }      
            }
             

            if(destoyOriginal){
                Object.Destroy(original);
            }            
        }

        protected virtual void OnDestroy()
        {
            isDestroyed = true;
            if(!cachedImage && DynamicTextureManager.Instance!=null)
            {
                DynamicTextureManager.Instance.Destroy(url, width, height);
            }
            // Debug.LogError("Image GO get destroyed " + gameObject.name);
            // if(m_texture != null)
            // {
            //     Destroy(m_texture);
            //     m_texture = null;
            // }

            if(m_sprite != null){
                Destroy(m_sprite);
                m_sprite = null;
            }
        }
    }
}
