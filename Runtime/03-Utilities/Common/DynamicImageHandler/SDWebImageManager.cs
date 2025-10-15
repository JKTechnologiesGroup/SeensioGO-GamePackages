using System;
using System.Collections.Generic;
using UnityEngine;

namespace JKTechnologies.CommonPackage.Utilities
{
    [Flags]
    public enum SDWebImageOptions
    {
        None = 0,
        MemoryCache = 1,
        DiskCache = 2,
        ShowLoadingIndicator = 4
    }

    public class SDWebImageManager : Singleton<SDWebImageManager>
    {

        #region Internal Fields

        private HashSet<string> failedURLs;

        #endregion


        #region Unity Callbacks

        protected override void Awake()
        {
            base.Awake();

            failedURLs = new HashSet<string>();
        }

        #endregion


        #region Public API

        public void LoadImageWithURL(string url, int objectId, SDWebImageOptions options, Action<float> progressCallback, Action<SDWebImageDownloaderError, byte[]> completionCallback)
        {
            if (failedURLs.Contains(url))
            {
                if (completionCallback != null)
                {
                    completionCallback(new SDWebImageDownloaderError(SDWebImageDownloaderError.ErrorType.FailedURL), null);
                }

                return;
            }

            SDImageCache.instance.QueryImageDataFromCacheForURL(url, options, (cachedData) =>
            {
                if (cachedData != null)
                {
                    if (completionCallback != null)
                    {
                        completionCallback(null, cachedData);
                    }

                    return;
                }

                SDWebImageDownloader.instance.DownloadImageWithURL(url, objectId, progressCallback, (error, downloadedData) =>
                {
                    if (error != null)
                    {
                        if (error.type == SDWebImageDownloaderError.ErrorType.InvalidURL
                            || error.type == SDWebImageDownloaderError.ErrorType.NotFound
                            || error.type == SDWebImageDownloaderError.ErrorType.FailedURL)
                        {
                            failedURLs.Add(url);
                        }

                        if (completionCallback != null)
                        {
                            completionCallback(error, null);
                        }

                        return;
                    }

                    if (downloadedData != null)
                    {
                        if (completionCallback != null)
                        {
                            completionCallback(null, downloadedData);
                        }

                        SDImageCache.instance.CacheImageDataForURL(url, downloadedData, options);
                    }
                });
            });
        }

        #endregion

    }
}
