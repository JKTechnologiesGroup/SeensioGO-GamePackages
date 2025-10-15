using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class WebAudioCache : Singleton<WebAudioCache> {

    #region Internal Fields

    private long maxCacheSize = 0;
    private int maxCacheAge = 7;
    private string cacheDirectoryPath;
    private Dictionary<string, AudioClip> memoryCache;

    #endregion


    #region Unity Callbacks

    protected override void Awake() {
        base.Awake();

        Init();
        // MainThread.instance.Init();
        // ThreadPool.QueueUserWorkItem((state) => {
        //     DeleteOldFilesOnDisk();
        // });
    }

    private void Init() {
        Application.lowMemory += OnLowMemory;

        memoryCache = new Dictionary<string, AudioClip>();

        cacheDirectoryPath = Path.Combine(Application.persistentDataPath, "DynamicAudio");
        if (!Directory.Exists(cacheDirectoryPath)) {
            Directory.CreateDirectory(cacheDirectoryPath);
        }
    }

    private void OnLowMemory() {
        ThreadPool.QueueUserWorkItem((state) => {
            ClearMemoryCache();
        });
    }

    #endregion


    #region Public API

    public void QueryAudioDataFromCacheForURL(string url, WebAudioOptions options, Action<AudioClip> callback) {
        // Debug.LogError("Query Cache");
        if ((options & WebAudioOptions.MemoryCache) != 0 && AudioDataExistsInMemory(url))
        {
            AudioClip data = LoadAudioDataFromMemory(url);

            if (data != null)
            {
                callback(data);
                return;
            }
        }

        if ((options & WebAudioOptions.DiskCache) != 0 && AudioDataExistsInDisk(url)) {
            // Debug.LogError("Exist in Cache");
            LoadAudioDataFromDisk(url, async(audioClip) =>
            {
                AudioClip data = audioClip;
                if ((options & WebAudioOptions.MemoryCache) != 0) {
                    StoreAudioDataInMemory(url, data);
                }

                if (data != null) {
                    callback(data);
                    return;
                }

                callback(null);
            });

            return;
        }

        callback(null);
    }

    public void CacheAudioDataForURL(string url, AudioClip data, WebAudioOptions options) {
        // Debug.LogError("Hello2");
        // Debug.LogError(data == null);
        if ((options & WebAudioOptions.MemoryCache) != 0)
        {
            Debug.LogError("MemoryCache");
            StoreAudioDataInMemory(url, data);
        }

        if ((options & WebAudioOptions.DiskCache) != 0) {
            Debug.LogError("DiskCache");
            StoreAudioDataInDisk(url, data);
        }
    }

    public void RemoveAudioDataFromCache(string url, WebAudioOptions options) {
        if ((options & WebAudioOptions.MemoryCache) != 0) {
            ThreadPool.QueueUserWorkItem((state) => {
                RemoveAudioDataFromMemory(url);
            });
        }

        if ((options & WebAudioOptions.DiskCache) != 0) {
            ThreadPool.QueueUserWorkItem((state) => {
                RemoveAudioDataFromDisk(url);
            });
        }
    }

    #endregion


    #region Memory Cache

    private bool AudioDataExistsInMemory(string url) {
        lock (memoryCache) {
            return memoryCache.ContainsKey(url);
        }
    }

    private void StoreAudioDataInMemory(string url, AudioClip data) {
        lock (memoryCache) {
            if(!memoryCache.ContainsKey(url)){
                memoryCache.Add(url, data);    
            }            
        }
    }

    private AudioClip LoadAudioDataFromMemory(string url) {
        lock (memoryCache) {
            return memoryCache.ContainsKey(url) ? memoryCache[url] : null;
        }
    }

    private void RemoveAudioDataFromMemory(string url) {
        lock (memoryCache) {
            if (memoryCache.ContainsKey(url)) {
                memoryCache.Remove(url);
            }
        }
    }

    private void ClearMemoryCache() {
        lock (memoryCache) {
            memoryCache.Clear();
        }
    }

    #endregion


    #region Disk Cache

    private bool AudioDataExistsInDisk(string url) {
        // Debug.LogError(PathForURL(url));
        string filePath = PathForURL(url);
        if (!filePath.EndsWith(".wav", StringComparison.OrdinalIgnoreCase))
            filePath = filePath.Replace(".mp3", ".wav");

        return File.Exists(filePath);
    }

    private void StoreAudioDataInDisk(string url, AudioClip data)
    {
        // Debug.LogError(PathForURL(url));
        // Debug.LogError("AudioCLip null" + data == null);
        string filePath = PathForURL(url);
        if (!filePath.EndsWith(".wav", StringComparison.OrdinalIgnoreCase))
            filePath = filePath.Replace(".mp3", ".wav");
        SaveWav(filePath, data);
    }

    public void SaveWav(string filepath, AudioClip clip)
    {
        // var filepath = Path.Combine(Application.persistentDataPath, filename);

        if (!filepath.EndsWith(".wav", StringComparison.OrdinalIgnoreCase))
            filepath = filepath.Replace(".mp3", ".wav");

        // Data
        byte[] pcm16 = AudioClipToPCM16Bytes(clip);

        using (var fs = new FileStream(filepath, FileMode.Create, FileAccess.Write))
        using (var bw = new BinaryWriter(fs))
        {
            int channels = clip.channels;
            int sampleRate = clip.frequency;
            int bitsPerSample = 16;
            int byteRate = sampleRate * channels * (bitsPerSample / 8);
            int blockAlign = channels * (bitsPerSample / 8);
            int subchunk2Size = pcm16.Length;
            int chunkSize = 36 + subchunk2Size;

            // RIFF header
            bw.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));
            bw.Write(chunkSize);
            bw.Write(System.Text.Encoding.ASCII.GetBytes("WAVE"));

            // fmt  subchunk
            bw.Write(System.Text.Encoding.ASCII.GetBytes("fmt "));
            bw.Write(16);                  // Subchunk1Size for PCM
            bw.Write((ushort)1);           // AudioFormat = PCM
            bw.Write((ushort)channels);
            bw.Write(sampleRate);
            bw.Write(byteRate);
            bw.Write((ushort)blockAlign);
            bw.Write((ushort)bitsPerSample);

            // data subchunk
            bw.Write(System.Text.Encoding.ASCII.GetBytes("data"));
            bw.Write(subchunk2Size);
            bw.Write(pcm16);
        }
    }

    // PCM16 bytes: interleaved channels, little-endian
    public byte[] AudioClipToPCM16Bytes(AudioClip clip)
    {
        // Debug.LogError("Hello");
        // Debug.LogError("Clip Samples " + clip.samples + " | Channels " + clip.channels);
        float[] samples = new float[clip.samples * clip.channels];
        Debug.LogError("Sample " + samples.Length);
        clip.GetData(samples, 0);

        byte[] bytes = new byte[samples.Length * 2]; // 16-bit = 2 bytes
        int o = 0;
        for (int i = 0; i < samples.Length; i++)
        {
            // Clamp to [-1, 1] then scale to Int16 range
            short s = (short)Mathf.Clamp(Mathf.RoundToInt(samples[i] * 32767f), -32768, 32767);
            var b = BitConverter.GetBytes(s); // little-endian
            bytes[o++] = b[0];
            bytes[o++] = b[1];
        }
        return bytes;
    }

    private void LoadAudioDataFromDisk(string url, Action<AudioClip> completionCallback)
    {
        string path = PathForURL(url);
        if (!path.EndsWith(".wav", StringComparison.OrdinalIgnoreCase))
            path = path.Replace(".mp3", ".wav");

        if (!File.Exists(path))
        {
            Debug.LogWarning($"Audio file not found at path: {path}");
            completionCallback?.Invoke(null);
            return;
        }

        StartCoroutine(InternalLoadAudioDataFromDisk(path, completionCallback));
    }

    // Coroutine that actually loads the clip
    private IEnumerator InternalLoadAudioDataFromDisk(string path, Action<AudioClip> completionCallback)
    {
        // UnityWebRequest needs a URI; local files should be prefixed with file://
        string uri = path;
        if (!path.StartsWith("file://", StringComparison.OrdinalIgnoreCase))
            uri = "file://" + path;

        AudioType audioType = AudioType.WAV;

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(uri, audioType))
        {
            www.timeout = 30;

#if UNITY_2020_2_OR_NEWER
            yield return www.SendWebRequest();
            bool hasError = www.result != UnityWebRequest.Result.Success;
#else
            yield return www.SendWebRequest();
            bool hasError = www.isNetworkError || www.isHttpError;
#endif

            if (hasError)
            {
                Debug.LogError($"Failed to load audio from '{uri}': {www.error}");
                completionCallback?.Invoke(null);
                yield break;
            }

            AudioClip clip = null;
            try
            {
                clip = DownloadHandlerAudioClip.GetContent(www);
                // Debug.LogError("Found Clip");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error extracting AudioClip from '{uri}': {e}");
                completionCallback?.Invoke(null);
                yield break;
            }

            if (clip == null)
            {
                Debug.LogError($"AudioClip is null after download: {uri}");
                completionCallback?.Invoke(null);
                yield break;
            }

            // Nice to have: name the clip from the file
            clip.name = Path.GetFileNameWithoutExtension(path);

            completionCallback?.Invoke(clip);
        }
    }

    private void RemoveAudioDataFromDisk(string url)
    {
        if (AudioDataExistsInDisk(url))
        {
            File.Delete(PathForURL(url));
        }
    }

    private void DeleteOldFilesOnDisk() {
        if (maxCacheAge == 0 && maxCacheSize == 0) {
            return;
        }

        DirectoryInfo directoryInfo = new DirectoryInfo(cacheDirectoryPath);
        FileInfo[] files = directoryInfo.GetFiles("*.*");

        DateTime expirationDate = DateTime.Now.AddDays(-maxCacheAge);
        List<string> filesToDelete = new List<string>();

        long currentCacheSize = 0;
        List<FileInfo> unexpiredCacheFiles = new List<FileInfo>();

        foreach (FileInfo file in files) {
            if (maxCacheAge > 0 && DateTime.Compare(file.LastAccessTime, expirationDate) < 0) {
                filesToDelete.Add(file.Name);
                continue;
            }


            currentCacheSize += file.Length;
            unexpiredCacheFiles.Add(file);
        }

        foreach (string filename in filesToDelete) {
            File.Delete(PathForFilename(filename));
        }

        if (maxCacheSize > 0 && currentCacheSize > maxCacheSize) {
            long desiredCacheSize = maxCacheSize / 2;

            List<FileInfo> sortedFiles = unexpiredCacheFiles.OrderByDescending(f => f.LastWriteTime).ToList();

            foreach (FileInfo file in sortedFiles) {
                File.Delete(PathForFilename(file.Name));

                currentCacheSize -= file.Length;

                if (currentCacheSize < desiredCacheSize) {
                    break;
                }
            }
        }
    }

    #endregion


    #region Helper Methods

    private string PathForFilename(string filename) {
        return Path.Combine(cacheDirectoryPath, filename);
    }

    public string PathForURL(string url) {
        return Path.Combine(cacheDirectoryPath, FilenameForURL(url));
    }

    private string FilenameForURL(string url) {
        string pathExtension = !String.IsNullOrEmpty(Path.GetExtension(url)) ? Path.GetExtension(url) : ".mp3";
        Match pathExtensionMatch = Regex.Match(pathExtension, "(\\.\\w+)");
        string filename = Md5(url) + Path.GetExtension(pathExtensionMatch.Value);
        // string filename = Md5(url) + ".wav"; // Always store in wave

        return filename;
    }

    private string Md5(string url) {
        MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();

        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(url);
        bytes = provider.ComputeHash(bytes);

        string output = "";
        foreach (byte b in bytes) {
            output += b.ToString("x2").ToLower();
        }

        return output;
    }

    #endregion

}