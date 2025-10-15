using System;
using System.Collections;
using System.Collections.Generic;

namespace JKTechnologies.CommonPackage.Utilities
{
    public class SDWebImageDownloaderOperation
    {

        public string url;
        public bool running;
        public bool finished;
        public IEnumerator downloadOperation;
        public IEnumerator downloadProgressTrackingOperation;

        private Dictionary<int, Action<float>> progressCallbacks;
        private Dictionary<int, Action<SDWebImageDownloaderError, byte[]>> completionCallbacks;

        public SDWebImageDownloaderOperation(string url, int id, IEnumerator downloadOperation, Action<float> progressCallback, Action<SDWebImageDownloaderError, byte[]> completionCallback)
        {
            this.url = url;
            this.downloadOperation = downloadOperation;

            progressCallbacks = new Dictionary<int, Action<float>> {
            { id, progressCallback }
        };

            completionCallbacks = new Dictionary<int, Action<SDWebImageDownloaderError, byte[]>> {
            { id, completionCallback }
        };
        }

        public void AddCallBacks(int id, Action<float> progressCallback, Action<SDWebImageDownloaderError, byte[]> completionCallback)
        {
            progressCallbacks.Add(id, progressCallback);
            completionCallbacks.Add(id, completionCallback);
        }

        public void RemoveCallbacks(int id)
        {
            if (progressCallbacks.ContainsKey(id))
            {
                progressCallbacks.Remove(id);
            }

            if (completionCallbacks.ContainsKey(id))
            {
                completionCallbacks.Remove(id);
            }
        }

        public void CallProgressCallbacks(float progress)
        {
            foreach (var progressCallback in progressCallbacks)
            {
                if (progressCallback.Value != null)
                {
                    progressCallback.Value(progress);
                }
            }
        }

        public void CallCompletionCallbacks(SDWebImageDownloaderError error, byte[] data)
        {
            foreach (var completionCallback in completionCallbacks)
            {
                if (completionCallback.Value != null)
                {
                    completionCallback.Value(error, data);
                }
            }
        }

        public bool IsValid()
        {
            return progressCallbacks.Count > 0 || completionCallbacks.Count > 0;
        }

        public bool IsIdIncluded(int id)
        {
            return progressCallbacks.ContainsKey(id) || completionCallbacks.ContainsKey(id);
        }
    }
}