using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class TextureDownloadManager : MonoBehaviour
{
    public static TextureDownloadManager Instance;
    
    public bool DeleteDownloadsOnApplicationQuit = true;
    private List<string> _downloadsPaths = new List<string>();
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            return;
        }
        
        Destroy(this);
    }

    public void DownloadTexture(TextureDef textureDef, Action<TextureDef, Texture> onDownloadSucceeded, Action<TextureDef, string> onDownloadFailed)
    {
        StartCoroutine(Download(textureDef, onDownloadSucceeded, onDownloadFailed));
    }

    private IEnumerator Download(TextureDef textureDef, Action<TextureDef, Texture> onDownloadSucceeded, Action<TextureDef, string> onDownloadFailed)
    {
        using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(textureDef.URL))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                onDownloadFailed?.Invoke(textureDef, webRequest.error);
                yield break;
            }
            
            Texture2D texture = DownloadHandlerTexture.GetContent(webRequest);
            byte[] textureBytes = texture.EncodeToPNG();
            
            try
            {
                File.WriteAllBytes(textureDef.FullPath, textureBytes);
                _downloadsPaths.Add(textureDef.FullPath);
                onDownloadSucceeded?.Invoke(textureDef, texture);
            }
            catch (Exception e)
            {
                onDownloadFailed?.Invoke(textureDef, e.Message);
            }
        }
    }

    private void OnApplicationQuit()
    {
        if (DeleteDownloadsOnApplicationQuit)
        {
            foreach (string path in _downloadsPaths)
            {
                try
                {
                    File.Delete(path);
                    File.Delete(path + ".meta"); // No exception thrown if file doesn't exist, no additional try catch needed
                    
                    // Debug.Log($"File deleted: {path}");
                }
                catch (Exception e)
                {
                    Debug.LogError($"File deletion failed: {e.Message} \nPath: {path}");
                }
            }
        }
        _downloadsPaths.Clear();
    }
}