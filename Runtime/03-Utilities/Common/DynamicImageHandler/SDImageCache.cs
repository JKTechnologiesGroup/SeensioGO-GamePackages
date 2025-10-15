using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using UnityEngine;

namespace JKTechnologies.CommonPackage.Utilities
{
    public class SDImageCache : Singleton<SDImageCache>
    {

        #region Internal Fields

        private long maxCacheSize = 0;
        private int maxCacheAge = 7;
        private string cacheDirectoryPath;
        private Dictionary<string, byte[]> memoryCache;

        #endregion


        #region Unity Callbacks

        protected override void Awake()
        {
            base.Awake();

            Init();
            MainThread.instance.Init();
            ThreadPool.QueueUserWorkItem((state) =>
            {
                DeleteOldFilesOnDisk();
            });
        }

        private void Init()
        {
            Application.lowMemory += OnLowMemory;

            memoryCache = new Dictionary<string, byte[]>();

            cacheDirectoryPath = Path.Combine(Application.persistentDataPath, "SDWebImage");
            if (!Directory.Exists(cacheDirectoryPath))
            {
                Directory.CreateDirectory(cacheDirectoryPath);
            }
        }

        private void OnLowMemory()
        {
            ThreadPool.QueueUserWorkItem((state) =>
            {
                ClearMemoryCache();
            });
        }

        #endregion


        #region Public API

        public void QueryImageDataFromCacheForURL(string url, SDWebImageOptions options, Action<byte[]> callback)
        {
            if ((options & SDWebImageOptions.MemoryCache) != 0 && ImageDataExistsInMemory(url))
            {
                byte[] data = LoadImageDataFromMemory(url);

                if (data != null)
                {
                    callback(data);
                    return;
                }
            }

            if ((options & SDWebImageOptions.DiskCache) != 0 && ImageDataExistsInDisk(url))
            {
                ThreadPool.QueueUserWorkItem((state) =>
                {
                    byte[] data = LoadImageDataFromDisk(url);

                    if ((options & SDWebImageOptions.MemoryCache) != 0)
                    {
                        StoreImageDataInMemory(url, data);
                    }

                    MainThread.instance.Execute(() =>
                    {
                        if (data != null)
                        {
                            callback(data);
                            return;
                        }

                        callback(null);
                    });
                });

                return;
            }

            callback(null);
        }

        public void CacheImageDataForURL(string url, byte[] data, SDWebImageOptions options)
        {
            if ((options & SDWebImageOptions.MemoryCache) != 0)
            {
                // Debug.LogError("MemoryCache");
                ThreadPool.QueueUserWorkItem((state) =>
                {
                    StoreImageDataInMemory(url, data);
                });
            }

            if ((options & SDWebImageOptions.DiskCache) != 0)
            {
                // Debug.LogError("DiskCache");
                ThreadPool.QueueUserWorkItem((state) =>
                {
                    StoreImageDataInDisk(url, data);
                });
            }
        }

        public void RemoveImageDataFromCache(string url, SDWebImageOptions options)
        {
            if ((options & SDWebImageOptions.MemoryCache) != 0)
            {
                ThreadPool.QueueUserWorkItem((state) =>
                {
                    RemoveImageDataFromMemory(url);
                });
            }

            if ((options & SDWebImageOptions.DiskCache) != 0)
            {
                ThreadPool.QueueUserWorkItem((state) =>
                {
                    RemoveImageDataFromDisk(url);
                });
            }
        }

        #endregion


        #region Memory Cache

        private bool ImageDataExistsInMemory(string url)
        {
            lock (memoryCache)
            {
                return memoryCache.ContainsKey(url);
            }
        }

        private void StoreImageDataInMemory(string url, byte[] data)
        {
            lock (memoryCache)
            {
                if (!memoryCache.ContainsKey(url))
                {
                    memoryCache.Add(url, data);
                }
            }
        }

        private byte[] LoadImageDataFromMemory(string url)
        {
            lock (memoryCache)
            {
                return memoryCache.ContainsKey(url) ? memoryCache[url] : null;
            }
        }

        private void RemoveImageDataFromMemory(string url)
        {
            lock (memoryCache)
            {
                if (memoryCache.ContainsKey(url))
                {
                    memoryCache.Remove(url);
                }
            }
        }

        private void ClearMemoryCache()
        {
            lock (memoryCache)
            {
                memoryCache.Clear();
            }
        }

        #endregion


        #region Disk Cache

        private bool ImageDataExistsInDisk(string url)
        {
            return File.Exists(PathForURL(url));
        }

        private void StoreImageDataInDisk(string url, byte[] data)
        {
            File.WriteAllBytes(PathForURL(url), data);
        }

        private byte[] LoadImageDataFromDisk(string url)
        {
            string path = PathForURL(url);

            if (File.Exists(path))
            {
                byte[] data = File.ReadAllBytes(path);

                return data;
            }

            return null;
        }

        private void RemoveImageDataFromDisk(string url)
        {
            if (ImageDataExistsInDisk(url))
            {
                File.Delete(PathForURL(url));
            }
        }

        private void DeleteOldFilesOnDisk()
        {
            if (maxCacheAge == 0 && maxCacheSize == 0)
            {
                return;
            }

            DirectoryInfo directoryInfo = new DirectoryInfo(cacheDirectoryPath);
            FileInfo[] files = directoryInfo.GetFiles("*.*");

            DateTime expirationDate = DateTime.Now.AddDays(-maxCacheAge);
            List<string> filesToDelete = new List<string>();

            long currentCacheSize = 0;
            List<FileInfo> unexpiredCacheFiles = new List<FileInfo>();

            foreach (FileInfo file in files)
            {
                if (maxCacheAge > 0 && DateTime.Compare(file.LastAccessTime, expirationDate) < 0)
                {
                    filesToDelete.Add(file.Name);
                    continue;
                }


                currentCacheSize += file.Length;
                unexpiredCacheFiles.Add(file);
            }

            foreach (string filename in filesToDelete)
            {
                File.Delete(PathForFilename(filename));
            }

            if (maxCacheSize > 0 && currentCacheSize > maxCacheSize)
            {
                long desiredCacheSize = maxCacheSize / 2;

                List<FileInfo> sortedFiles = unexpiredCacheFiles.OrderByDescending(f => f.LastWriteTime).ToList();

                foreach (FileInfo file in sortedFiles)
                {
                    File.Delete(PathForFilename(file.Name));

                    currentCacheSize -= file.Length;

                    if (currentCacheSize < desiredCacheSize)
                    {
                        break;
                    }
                }
            }
        }

        #endregion


        #region Helper Methods

        private string PathForFilename(string filename)
        {
            return Path.Combine(cacheDirectoryPath, filename);
        }

        private string PathForURL(string url)
        {
            return Path.Combine(cacheDirectoryPath, FilenameForURL(url));
        }

        private string FilenameForURL(string url)
        {
            string pathExtension = !String.IsNullOrEmpty(Path.GetExtension(url)) ? Path.GetExtension(url) : ".img";
            Match pathExtensionMatch = Regex.Match(pathExtension, "(\\.\\w+)");
            string filename = Md5(url) + Path.GetExtension(pathExtensionMatch.Value);

            return filename;
        }

        private string Md5(string url)
        {
            MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();

            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(url);
            bytes = provider.ComputeHash(bytes);

            string output = "";
            foreach (byte b in bytes)
            {
                output += b.ToString("x2").ToLower();
            }

            return output;
        }

        #endregion

    }
}