using System;
using System.Collections.Generic;
using UnityEngine;

[Flags]
public enum WebAudioOptions {
    None = 0,
    MemoryCache = 1,
    DiskCache = 2,
    ShowLoadingIndicator = 4
}

public class WebAudioManager : Singleton<WebAudioManager> {

    #region Internal Fields

    private HashSet<string> failedURLs;

    #endregion


    #region Unity Callbacks

    protected override void Awake() {
        base.Awake();

        failedURLs = new HashSet<string>();
    }

    #endregion


    #region Public API

    public void LoadAudioWithURL(string url, int objectId, WebAudioOptions options, Action<float> progressCallback, Action<WebAudioDownloaderError, AudioClip> completionCallback) {
        if (failedURLs.Contains(url)) {
            if (completionCallback != null) {
                completionCallback(new WebAudioDownloaderError(WebAudioDownloaderError.ErrorType.FailedURL), null);
            }

            return;
        }

        WebAudioCache.instance.QueryAudioDataFromCacheForURL(url, options, (cachedData) => {
            if (cachedData != null) {
                if (completionCallback != null) {
                    completionCallback(null, cachedData);
                }

                return;
            }

            WebAudioDownloader.instance.DownloadAudioWithURL(url, objectId, progressCallback, (error, downloadedData) =>
            {
                // Debug.LogError("Download Audio");
                if (error != null)
                {
                    // Debug.LogError("Error");
                    if (error.type == WebAudioDownloaderError.ErrorType.InvalidURL
                        || error.type == WebAudioDownloaderError.ErrorType.NotFound
                        || error.type == WebAudioDownloaderError.ErrorType.FailedURL)
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
                    // Debug.LogError("Success");

                    if (completionCallback != null)
                    {
                        completionCallback(null, downloadedData);
                    }

                    WebAudioCache.instance.CacheAudioDataForURL(url, downloadedData, options);
                }

                // Debug.LogError("Hello");
            });
        });
    }

    #endregion

}
