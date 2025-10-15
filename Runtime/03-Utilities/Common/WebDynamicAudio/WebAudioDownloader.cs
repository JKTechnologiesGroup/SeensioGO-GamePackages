using System;
using System.Collections;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.Networking;

public class WebAudioDownloader : Singleton<WebAudioDownloader> {

    #region Internal Fields

    private int maxConcurrentDownloads = 6;
    private OrderedDictionary operationsQueue = new OrderedDictionary();

    #endregion

    #region Public API

    public void DownloadAudioWithURL(string url, int objectId, Action<float> progressCallback, Action<WebAudioDownloaderError, AudioClip> completionCallback) {
        RemoveOldInstanceOperationsFromQueue(objectId);

        WebAudioDownloaderOperation[] sameURLOperations = GetOperationsWithPredicate(operation => operation.url == url);
        if (sameURLOperations.Length > 0) {
            ((WebAudioDownloaderOperation)operationsQueue[sameURLOperations[0].url]).AddCallBacks(objectId, progressCallback, completionCallback);
            return;
        }

        WebAudioDownloaderOperation newOperation = new WebAudioDownloaderOperation(url, objectId, InternalDownloadAudioWithURL(url), progressCallback, completionCallback);
        AddOperationToQueue(newOperation);
    }

    #endregion


    #region Image Downloader

    private IEnumerator InternalDownloadAudioWithURL(string url) {
        Action<WebAudioDownloaderError, AudioClip> EndDownloadOperation = (error, data) => {
            if (error != null || data != null) {
                ((WebAudioDownloaderOperation)operationsQueue[url]).CallCompletionCallbacks(error, data);
            }

            if (((WebAudioDownloaderOperation)operationsQueue[url]).downloadProgressTrackingOperation != null) {
                StopCoroutine(((WebAudioDownloaderOperation)operationsQueue[url]).downloadProgressTrackingOperation);
                ((WebAudioDownloaderOperation)operationsQueue[url]).downloadProgressTrackingOperation = null;
            }

            ((WebAudioDownloaderOperation)operationsQueue[url]).finished = true;
            HandleOperationsQueueUpdate();
        };

        if (!IsURLValid(url)) {
            WebAudioDownloaderError error = new WebAudioDownloaderError(WebAudioDownloaderError.ErrorType.InvalidURL);
            EndDownloadOperation(error, null);

            yield break;
        }

        if (Application.internetReachability == NetworkReachability.NotReachable) {
            WebAudioDownloaderError error = new WebAudioDownloaderError(WebAudioDownloaderError.ErrorType.NoInternet);
            EndDownloadOperation(error, null);

            yield break;
        }

        var audioType = AudioType.MPEG;
        string filePath = WebAudioCache.instance.PathForURL(url);
        if (filePath.EndsWith(".wav", StringComparison.OrdinalIgnoreCase)) audioType = AudioType.WAV;
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, audioType))
        {
            www.timeout = 30;

            ((WebAudioDownloaderOperation)operationsQueue[url]).downloadProgressTrackingOperation = TrackDownloadProgress(www);
            StartCoroutine(((WebAudioDownloaderOperation)operationsQueue[url]).downloadProgressTrackingOperation);

            yield return www.SendWebRequest();

            if (!String.IsNullOrEmpty(www.error))
            {
                WebAudioDownloaderError error = new WebAudioDownloaderError(WebAudioDownloaderError.ErrorType.Unknown, www.error);
                EndDownloadOperation(error, null);

                yield break;
            }

            AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
            if (clip == null)
            {
                WebAudioDownloaderError error = new WebAudioDownloaderError(WebAudioDownloaderError.ErrorType.FailedURL);
                EndDownloadOperation(error, null);

                yield break;
            }

            if (www.isDone)
            {
                ((WebAudioDownloaderOperation)operationsQueue[url]).CallProgressCallbacks(1.0f);
                EndDownloadOperation(null, DownloadHandlerAudioClip.GetContent(www));

                yield break;
            }
        }

        EndDownloadOperation(null, null);
        yield break;
    }

    private IEnumerator TrackDownloadProgress(UnityWebRequest www) {
        while (!www.isDone) {
            try{
                if (www.downloadProgress >= 0 && www.downloadProgress < 1 && operationsQueue !=null && operationsQueue[www.url]!=null) {
                    ((WebAudioDownloaderOperation)operationsQueue[www.url]).CallProgressCallbacks(www.downloadProgress);
                }                
            }catch(Exception ex){
                Debug.LogError(ex.ToString());
            }

            yield return new WaitForSeconds(0.1f);
        }

        yield break;
    }

    private bool IsURLValid(string url) {
        return !String.IsNullOrEmpty(url) && url.Substring(0, 4) == "http";
    }

    #endregion


    #region Operations Queue

    private void RemoveOldInstanceOperationsFromQueue(int id) {
        WebAudioDownloaderOperation[] instanceOperations = GetOperationsWithPredicate(operation => operation.IsIdIncluded(id));
        if (instanceOperations.Length > 0) {
            foreach (var operation in instanceOperations) {
                ((WebAudioDownloaderOperation)operationsQueue[operation.url]).RemoveCallbacks(id);
                if (!((WebAudioDownloaderOperation)operationsQueue[operation.url]).IsValid()) {
                    RemoveOperationFromQueue(operation.url);
                }
            }
        }
    }

    private void AddOperationToQueue(WebAudioDownloaderOperation operation) {
        operationsQueue.Add(operation.url, operation);
        HandleOperationsQueueUpdate();
    }

    private void RemoveOperationFromQueue(string url) {
        if (operationsQueue.Contains(url)) {
            if (((WebAudioDownloaderOperation)operationsQueue[url]).downloadOperation != null) {
                StopCoroutine(((WebAudioDownloaderOperation)operationsQueue[url]).downloadOperation);
                ((WebAudioDownloaderOperation)operationsQueue[url]).downloadOperation = null;
            }

            operationsQueue.Remove(url);
            HandleOperationsQueueUpdate();
        }
    }

    private void HandleOperationsQueueUpdate() {
        if (operationsQueue.Count > 0) {
            WebAudioDownloaderOperation[] finishedOperations = GetOperationsWithPredicate(operation => operation.finished);
            foreach (var operation in finishedOperations) {
                RemoveOperationFromQueue(operation.url);
            }

            int pendingOperationsCount = GetOperationsWithPredicate(operation => !operation.running).Length;
            int runningOperationsCount = GetOperationsWithPredicate(operation => operation.running).Length;

            if (pendingOperationsCount > 0 && runningOperationsCount < maxConcurrentDownloads) {
                int availableOperationsCount = maxConcurrentDownloads - runningOperationsCount;

                for (int i = 0; i < operationsQueue.Count; i++) {
                    if (!((WebAudioDownloaderOperation)operationsQueue[i]).running) {
                        ((WebAudioDownloaderOperation)operationsQueue[i]).running = true;
                        StartCoroutine(((WebAudioDownloaderOperation)operationsQueue[i]).downloadOperation);

                        if (--availableOperationsCount <= 0) {
                            break;
                        }
                    }
                }
            }
        }
    }

    private WebAudioDownloaderOperation[] GetOperationsWithPredicate(Predicate<WebAudioDownloaderOperation> predicate) {
        WebAudioDownloaderOperation[] operations = new WebAudioDownloaderOperation[operationsQueue.Count];
        operationsQueue.Values.CopyTo(operations, 0);
        return Array.FindAll(operations, predicate);
    }

    #endregion

}


public class WebAudioDownloaderError {
    public enum ErrorType { Unknown, InvalidURL, NoInternet, UnresolvedHost, NotFound, RequestTimedOut, FailedURL }

    public ErrorType type;
    public string description;

    public WebAudioDownloaderError(ErrorType type, string description = "") {
        this.type = type;
        this.description = description;

        switch (type) {
            case ErrorType.InvalidURL:
                this.description = "Image url isn't valid";
                break;
            case ErrorType.NoInternet:
                this.description = "No internet connection";
                break;
            case ErrorType.FailedURL:
                this.description = "Unable to convert downloaded data into texture";
                break;
            default:
                switch (description) {
                    case "HTTP/1.1 404 Not Found":
                        this.type = ErrorType.NotFound;
                        break;
                    case "Cannot resolve destination host":
                        this.type = ErrorType.UnresolvedHost;
                        break;
                    case "Request timeout":
                        this.type = ErrorType.RequestTimedOut;
                        break;
                }
                break;
        }
    }

}