using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public static class TextureDownloadService
{    
    public static List<string> CreatedTexturePaths = new List<string>();

    public static IEnumerator DownloadTexture(TextureDef textureDef, Action<TextureDef, Texture2D, string> onDownloadComplete)
    {
        using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(textureDef.URL))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                onDownloadComplete?.Invoke(textureDef, null, webRequest.error);
                yield break;
            }
            
            Texture2D texture = DownloadHandlerTexture.GetContent(webRequest);
            if (texture == null)
            {
                onDownloadComplete?.Invoke(textureDef, null, "downloaded texture is null");
                yield break;
            }
            
            byte[] textureBytes = texture.EncodeToPNG();
            
            try
            {
                File.WriteAllBytes(textureDef.FullPath, textureBytes);
                CreatedTexturePaths.Add(textureDef.FullPath);
                onDownloadComplete?.Invoke(textureDef, texture, string.Empty);
            }
            catch (Exception e)
            {
                onDownloadComplete?.Invoke(textureDef, null, e.Message);
            }
        }
    }

    public static void CleanDownloads()
    {
        foreach (string path in CreatedTexturePaths)
        {
            try
            {
                File.Delete(path);
                File.Delete(path + ".meta"); // No exception thrown if file doesn't exist, no additional try catch needed
            }
            catch (Exception e)
            {
                Debug.LogError($"File deletion failed: {e.Message} \nPath: {path}");
            }
        }
        CreatedTexturePaths.Clear();
    }
}