using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public static class TextureDownloadService
{    
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

            string path = textureDef.Path + textureDef.Extension;
            if (!FileIOService.TryCreateFile(path, texture.EncodeToPNG(), out string error))
            {
                onDownloadComplete?.Invoke(textureDef, null, error);
                yield break;
            }
            
            onDownloadComplete?.Invoke(textureDef, texture, string.Empty);
        }
    }
}