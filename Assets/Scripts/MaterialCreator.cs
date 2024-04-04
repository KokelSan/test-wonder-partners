using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DownloadedTexture
{
    public TextureDef TextureDef;
    public Texture2D Texture;

    public DownloadedTexture(TextureDef textureDef, Texture2D texture)
    {
        TextureDef = textureDef;
        Texture = texture;
    }
}

public class MaterialCreator : MonoBehaviour
{
    public ModelTextureConfigSO TextureConfig;
    public ShaderDescriptionTableSO ShaderDescription;

    private List<TextureDef> _downloadingTextures = new List<TextureDef>();
    private Dictionary<TextureType, DownloadedTexture> _downloadedTextures = new Dictionary<TextureType, DownloadedTexture>();
    private int _downloadFailsNb = 0;
    private Action _onMaterialCreated;

    private bool DownloadComplete => _downloadingTextures.Count == 0;
    
    public void StartMaterialCreation(Action onMaterialCreated)
    {
        foreach (TextureDef textureDef in TextureConfig.Textures)
        {
            textureDef.FullPath = TextureConfig.CommonPath + textureDef.PathEnd;
            _downloadingTextures.Add(textureDef);
            StartCoroutine(TextureDownloadService.DownloadTexture(textureDef, OnTextureDownloaded));
        }
        _onMaterialCreated = onMaterialCreated;
    }

    private void OnTextureDownloaded(TextureDef textureDef, Texture2D downloadedTexture, string errorMsg)
    {
        if (downloadedTexture == null)
        {
            Debug.LogError($"Download of texture '{textureDef.Type}' failed: {errorMsg}. \nURL: {textureDef.URL}");
            _downloadFailsNb++;
        }
        else
        {
            Debug.Log($"Texture '{textureDef.Type}' downloaded. \nURL: {textureDef.URL}");
        }

        _downloadingTextures.Remove(textureDef);
        _downloadedTextures.Add(textureDef.Type, new DownloadedTexture(textureDef, downloadedTexture));
        
        if (DownloadComplete)
        {
            CreateMaterial();
        }
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
        
        if (!TryGetComponent(out Renderer renderer))
        {
            Debug.LogError("This model has no renderer to receive the material");
            return;
        }
        
        renderer.material = new Material(shader);
        
        foreach (ShaderTextureProperty shaderTextureProperty in ShaderDescription.TextureProperties)
        {
            if (_downloadedTextures.TryGetValue(shaderTextureProperty.TextureType, out DownloadedTexture downloadedTexture))
            {
                if (downloadedTexture.Texture == null)
                {
                    continue;
                }

                if (renderer.material.HasTexture(shaderTextureProperty.PropertyName))
                {
                    // renderer.material.EnableKeyword("");
                    Texture2D finalTexture = TexturePackingService.ComputeTexture(downloadedTexture.Texture, downloadedTexture.TextureDef.PackingMethod, shaderTextureProperty.PackingMethod);
                    renderer.material.SetTexture(shaderTextureProperty.PropertyName, finalTexture);
                }
            }
        }

        if (_downloadFailsNb == TextureConfig.Textures.Count)
        {
            Debug.LogError($"Material creation error: all textures download failed.");
        }
        else if (_downloadFailsNb > 0)
        {
            Debug.LogError($"Material creation ended with {_downloadFailsNb} textures download fails.");
        }
        else
        {
            Debug.Log($"Textures successfully downloaded, material created.");
        }

        _onMaterialCreated?.Invoke();
        _onMaterialCreated = null;
    }
}