using System.Collections.Generic;
using UnityEngine;

public class MaterialCreator : MonoBehaviour
{
    public ModelTextureConfigSO TextureConfig;
    public ShaderDescriptionTableSO ShaderDescription;

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
        _downloadingTextures.Remove(textureDef);
        _downloadedTextures.Add(textureDef.Type, downloadedTexture);
        
        if (AllTexturesAreDownloaded)
        {
            CreateMaterial();
        }
    }

    private void OnDownloadFailed(TextureDef textureDef, string errorMsg)
    {
        Debug.LogError($"Download failed: {errorMsg}. \nURL: {textureDef.URL}");
    }

    private void CreateMaterial()
    {
        if (ShaderDescription == null)
        {
            Debug.LogError($"No Shader Description, material creation aborted.");
            return;
        }
        
        Shader shader = Shader.Find(ShaderDescription.Name);
        if (shader == null)
        {
            Debug.LogError($"Shader '{ShaderDescription.Name}' is not valid, material creation aborted.");
            return;
        }
        
        Material material = new Material(shader);

        foreach (var shaderTextureProperty in ShaderDescription.TextureProperties)
        {
            if (_downloadedTextures.TryGetValue(shaderTextureProperty.TextureType, out var texture))
            {
                if (material.HasTexture(shaderTextureProperty.PropertyName))
                {
                    material.SetTexture(shaderTextureProperty.PropertyName, texture);
                }
            }
        }

        if (TryGetComponent(out Renderer renderer))
        {
            renderer.material = material;
        }
    }
}