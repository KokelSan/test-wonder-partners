using System;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private MaterialConfigSO MaterialToCreate;

    private List<TextureDef> _downloadingTextures = new List<TextureDef>();
    private List<DownloadedTexture> _downloadedTextures = new List<DownloadedTexture>();
    private int _downloadFailsNb = 0;
    
    private Action _onModelReady;

    private bool DownloadComplete => _downloadingTextures.Count == 0;
    
    public void StartMaterialCreation(Action onModelReady)
    {
        _onModelReady = onModelReady;
        
        foreach (TextureDef textureDef in MaterialToCreate.Textures)
        {
            _downloadingTextures.Add(textureDef);
            StartCoroutine(TextureDownloadService.DownloadTexture(textureDef, MaterialToCreate.TexturesDirectory, MaterialToCreate.TexturesNamePrefix, OnTextureDownloaded));
        }
    }

    private void OnTextureDownloaded(TextureDef textureDef, Texture2D downloadedTexture, string errorMsg)
    {
        if (downloadedTexture == null)
        {
            Debug.LogError($"Download of texture '{textureDef.Type}' failed: {errorMsg}. \nURL: {textureDef.URL}\n");
            _downloadFailsNb++;
        }
        else
        {
            Debug.Log($"Texture '{textureDef.Type}' downloaded. \nURL: {textureDef.URL}\n");
            _downloadedTextures.Add(new DownloadedTexture(textureDef, downloadedTexture));
        }

        _downloadingTextures.Remove(textureDef);
        
        if (DownloadComplete)
        {
            if (_downloadFailsNb == MaterialToCreate.Textures.Count)
            {
                Debug.LogError($"All textures download failed, material creation aborted.");
                _onModelReady?.Invoke();
                _onModelReady = null;
                return;
            }
        
            if (_downloadFailsNb > 0)
            {
                Debug.LogError($"Textures download ended with {_downloadFailsNb} {(_downloadFailsNb == 1 ? "fail" : "fails")} out of {MaterialToCreate.Textures.Count}. Material will be created with downloaded textures only.");
            }
            else
            {
                Debug.Log($"All {MaterialToCreate.Textures.Count} textures have been successfully downloaded.");
            }
            
            CreateMaterialIfPossible();
        }
    }
    
    private void CreateMaterialIfPossible()
    {
        if (MaterialToCreate.ShaderConfig == null)
        {
            Debug.LogError($"No Shader Description, material creation aborted.");
            return;
        }
        
        Shader shader = Shader.Find(MaterialToCreate.ShaderConfig.Name);
        if (shader == null)
        {
            Debug.LogError($"Shader '{MaterialToCreate.ShaderConfig.Name}' is not valid, material creation aborted.");
            return;
        }
        
        if (!TryGetComponent(out Renderer renderer))
        {
            Debug.LogError("This model has no renderer, material creation aborted.");
            return;
        }
        
        CreateMaterial(shader, renderer);
    }
    
    private void CreateMaterial(Shader shader, Renderer renderer)
    {
        Material material = new Material(shader)
        {
            globalIlluminationFlags = MaterialToCreate.ShaderConfig.GIFlag,
        };

        foreach (var downloadedTexture in _downloadedTextures)
        {
            TextureType textureType = downloadedTexture.TextureDef.Type;
            
            // Setting the texture
            ShaderTextureProperty shaderTextureProperty = MaterialToCreate.ShaderConfig.TextureProperties.Find(textureProperty => textureProperty.TextureType == textureType);
            if (shaderTextureProperty != null)
            {
                if (material.HasTexture(shaderTextureProperty.PropertyName))
                {
                    Texture2D finalTexture = TexturePackingService.ComputeTexture(downloadedTexture, shaderTextureProperty.PackingMethod, MaterialToCreate.TexturesDirectory, MaterialToCreate.TexturesNamePrefix);
                    material.SetTexture(shaderTextureProperty.PropertyName, finalTexture);
                }
            }
            
            // Enabling/disabling the keywords related to this texture
            foreach (var keywordParameter in MaterialToCreate.ShaderConfig.KeywordParameters.FindAll(keywordParam => keywordParam.RelatedTexture == textureType))
            {
                if (keywordParameter.Enable)
                {
                    material.EnableKeyword(keywordParameter.Keyword);
                    continue;
                }
                material.DisableKeyword(keywordParameter.Keyword);
            }
            
            // Setting the colors related to this texture
            foreach (var colorParameter in MaterialToCreate.ShaderConfig.ColorParameters.FindAll(colorParam => colorParam.RelatedTexture == textureType))
            {
                if (material.HasColor(colorParameter.Name))
                {
                    material.SetColor(colorParameter.Name, colorParameter.Color);
                }
            }
        }
        
        renderer.material = material;

        _onModelReady?.Invoke();
        _onModelReady = null;
    }
}