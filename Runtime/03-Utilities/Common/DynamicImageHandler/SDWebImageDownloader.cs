using System;
using System.Collections;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.Networking;

namespace JKTechnologies.CommonPackage.Utilities
{

    public class SDWebImageDownloader : Singleton<SDWebImageDownloader>
    {

        #region Internal Fields

        private int maxConcurrentDownloads = 6;
        private OrderedDictionary operationsQueue = new OrderedDictionary();

        #endregion

        #region Public API

        public void DownloadImageWithURL(string url, int objectId, Action<float> progressCallback, Action<SDWebImageDownloaderError, byte[]> completionCallback)
        {
            RemoveOldInstanceOperationsFromQueue(objectId);

            SDWebImageDownloaderOperation[] sameURLOperations = GetOperationsWithPredicate(operation => operation.url == url);
            if (sameURLOperations.Length > 0)
            {
                ((SDWebImageDownloaderOperation)operationsQueue[sameURLOperations[0].url]).AddCallBacks(objectId, progressCallback, completionCallback);
                return;
            }

            SDWebImageDownloaderOperation newOperation = new SDWebImageDownloaderOperation(url, objectId, InternalDownloadImageWithURL(url), progressCallback, completionCallback);
            AddOperationToQueue(newOperation);
        }

        #endregion


        #region Image Downloader

        private IEnumerator InternalDownloadImageWithURL(string url)
        {
            Action<SDWebImageDownloaderError, byte[]> EndDownloadOperation = (error, data) =>
            {
                if (error != null || data != null)
                {
                    ((SDWebImageDownloaderOperation)operationsQueue[url]).CallCompletionCallbacks(error, data);
                }

                if (((SDWebImageDownloaderOperation)operationsQueue[url]).downloadProgressTrackingOperation != null)
                {
                    StopCoroutine(((SDWebImageDownloaderOperation)operationsQueue[url]).downloadProgressTrackingOperation);
                    ((SDWebImageDownloaderOperation)operationsQueue[url]).downloadProgressTrackingOperation = null;
                }

                ((SDWebImageDownloaderOperation)operationsQueue[url]).finished = true;
                HandleOperationsQueueUpdate();
            };

            if (!IsURLValid(url))
            {
                SDWebImageDownloaderError error = new SDWebImageDownloaderError(SDWebImageDownloaderError.ErrorType.InvalidURL);
                EndDownloadOperation(error, null);

                yield break;
            }

            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                SDWebImageDownloaderError error = new SDWebImageDownloaderError(SDWebImageDownloaderError.ErrorType.NoInternet);
                EndDownloadOperation(error, null);

                yield break;
            }

            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
            {
                www.timeout = 30;

                ((SDWebImageDownloaderOperation)operationsQueue[url]).downloadProgressTrackingOperation = TrackDownloadProgress(www);
                StartCoroutine(((SDWebImageDownloaderOperation)operationsQueue[url]).downloadProgressTrackingOperation);

                yield return www.SendWebRequest();

                if (!String.IsNullOrEmpty(www.error))
                {
                    SDWebImageDownloaderError error = new SDWebImageDownloaderError(SDWebImageDownloaderError.ErrorType.Unknown, www.error);
                    EndDownloadOperation(error, null);

                    yield break;
                }

                Texture texture = DownloadHandlerTexture.GetContent(www);
                SeensioGifDecoder gifDecoder = new SeensioGifDecoder();
                if ((texture == null || (texture.width == 8 && texture.height == 8)) && gifDecoder.Read(new System.IO.MemoryStream(www.downloadHandler.data)) != SeensioGifDecoder.Status.Ok)
                {
                    SDWebImageDownloaderError error = new SDWebImageDownloaderError(SDWebImageDownloaderError.ErrorType.FailedURL);
                    EndDownloadOperation(error, null);

                    yield break;
                }

                if (www.isDone && www.downloadHandler.data != null)
                {
                    ((SDWebImageDownloaderOperation)operationsQueue[url]).CallProgressCallbacks(1.0f);
                    EndDownloadOperation(null, www.downloadHandler.data);

                    yield break;
                }
            }

            EndDownloadOperation(null, null);
            yield break;
        }

        private IEnumerator TrackDownloadProgress(UnityWebRequest www)
        {
            while (!www.isDone)
            {
                try
                {
                    if (www.downloadProgress >= 0 && www.downloadProgress < 1 && operationsQueue != null && operationsQueue[www.url] != null)
                    {
                        ((SDWebImageDownloaderOperation)operationsQueue[www.url]).CallProgressCallbacks(www.downloadProgress);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex.ToString());
                }

                yield return new WaitForSeconds(0.1f);
            }

            yield break;
        }

        private bool IsURLValid(string url)
        {
            return !String.IsNullOrEmpty(url) && url.Substring(0, 4) == "http";
        }

        #endregion


        #region Operations Queue

        private void RemoveOldInstanceOperationsFromQueue(int id)
        {
            SDWebImageDownloaderOperation[] instanceOperations = GetOperationsWithPredicate(operation => operation.IsIdIncluded(id));
            if (instanceOperations.Length > 0)
            {
                foreach (var operation in instanceOperations)
                {
                    ((SDWebImageDownloaderOperation)operationsQueue[operation.url]).RemoveCallbacks(id);
                    if (!((SDWebImageDownloaderOperation)operationsQueue[operation.url]).IsValid())
                    {
                        RemoveOperationFromQueue(operation.url);
                    }
                }
            }
        }

        private void AddOperationToQueue(SDWebImageDownloaderOperation operation)
        {
            operationsQueue.Add(operation.url, operation);
            HandleOperationsQueueUpdate();
        }

        private void RemoveOperationFromQueue(string url)
        {
            if (operationsQueue.Contains(url))
            {
                if (((SDWebImageDownloaderOperation)operationsQueue[url]).downloadOperation != null)
                {
                    StopCoroutine(((SDWebImageDownloaderOperation)operationsQueue[url]).downloadOperation);
                    ((SDWebImageDownloaderOperation)operationsQueue[url]).downloadOperation = null;
                }

                operationsQueue.Remove(url);
                HandleOperationsQueueUpdate();
            }
        }

        private void HandleOperationsQueueUpdate()
        {
            if (operationsQueue.Count > 0)
            {
                SDWebImageDownloaderOperation[] finishedOperations = GetOperationsWithPredicate(operation => operation.finished);
                foreach (var operation in finishedOperations)
                {
                    RemoveOperationFromQueue(operation.url);
                }

                int pendingOperationsCount = GetOperationsWithPredicate(operation => !operation.running).Length;
                int runningOperationsCount = GetOperationsWithPredicate(operation => operation.running).Length;

                if (pendingOperationsCount > 0 && runningOperationsCount < maxConcurrentDownloads)
                {
                    int availableOperationsCount = maxConcurrentDownloads - runningOperationsCount;

                    for (int i = 0; i < operationsQueue.Count; i++)
                    {
                        if (!((SDWebImageDownloaderOperation)operationsQueue[i]).running)
                        {
                            ((SDWebImageDownloaderOperation)operationsQueue[i]).running = true;
                            StartCoroutine(((SDWebImageDownloaderOperation)operationsQueue[i]).downloadOperation);

                            if (--availableOperationsCount <= 0)
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }

        private SDWebImageDownloaderOperation[] GetOperationsWithPredicate(Predicate<SDWebImageDownloaderOperation> predicate)
        {
            SDWebImageDownloaderOperation[] operations = new SDWebImageDownloaderOperation[operationsQueue.Count];
            operationsQueue.Values.CopyTo(operations, 0);
            return Array.FindAll(operations, predicate);
        }

        #endregion

    }


    public class SDWebImageDownloaderError
    {
        public enum ErrorType { Unknown, InvalidURL, NoInternet, UnresolvedHost, NotFound, RequestTimedOut, FailedURL }

        public ErrorType type;
        public string description;

        public SDWebImageDownloaderError(ErrorType type, string description = "")
        {
            this.type = type;
            this.description = description;

            switch (type)
            {
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
                    switch (description)
                    {
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
}