using System.Collections.Generic;
using UnityEngine;

public class MaterialInstantiator : MonoBehaviour
{
    public ModelTextureConfigSO TextureConfig;

    private List<TextureDef> _downloadingTextures = new List<TextureDef>();
    private Dictionary<TextureType, Texture> _downloadedTextures = new Dictionary<TextureType, Texture>();

    private bool AllTexturesAreDownloaded => _downloadingTextures.Count == 0;
    
    private void Start()
    {
        foreach (TextureDef textureDef in TextureConfig.Textures)
        {
            textureDef.FullPath = TextureConfig.CommonPath + textureDef.PathEnd;
            _downloadingTextures.Add(textureDef);
            TextureDownloadManager.Instance.DownloadTexture(textureDef, OnTextureDownloaded, OnDownloadFailed);
        }
    }

    private void OnTextureDownloaded(TextureDef textureDef, Texture downloadedTexture)
    {
        // Debug.Log($"Texture {textureDef.Type} successfully downloaded from {textureDef.URL}");

        _downloadingTextures.Remove(textureDef);
        _downloadedTextures.Add(textureDef.Type, downloadedTexture);
        
        if (AllTexturesAreDownloaded)
        {
            // Debug.Log("All textures are downloaded!");
        }
    }

    private void OnDownloadFailed(TextureDef textureDef, string errorMsg)
    {
        Debug.LogError($"Download failed: {errorMsg}. \nURL: {textureDef.URL}");
    }
}