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

    private bool DownloadIsComplete => _downloadedTextures.Count + _downloadFailsNb == MaterialToCreate.Textures.Count;
    
    public void StartMaterialCreation(Action onModelReady)
    {
        _onModelReady = onModelReady;
        
        foreach (TextureDef textureDef in MaterialToCreate.Textures)
        {
            string path = $"{MaterialToCreate.TexturesDirectory}/{MaterialToCreate.TexturesNamePrefix}{textureDef.Type}{textureDef.Extension}";
            if (FileIOService.TryLoadImage(path, out Texture2D texture))
            {
                Debug.Log($"Texture '{textureDef.Type}' already downloaded, loading it from disk.");
                _downloadedTextures.Add(new DownloadedTexture(textureDef, texture));
                CheckIfDownloadIsComplete();
                continue;
            }
            
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
        CheckIfDownloadIsComplete();
    }

    private void CheckIfDownloadIsComplete()
    {
        if (DownloadIsComplete)
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
                Debug.Log($"All {MaterialToCreate.Textures.Count} textures have been successfully loaded.");
            }
            
            CreateMaterialIfPossible();
        }
    }
    
    private void CreateMaterialIfPossible()
    {
        if (MaterialToCreate.ShaderConfig == null)
        {
            Debug.LogError($"No Shader Description referenced in the material config, material creation aborted.");
            return;
        }
        
        Shader shader = Shader.Find(MaterialToCreate.ShaderConfig.Name);
        if (shader == null)
        {
            Debug.LogError($"'{MaterialToCreate.ShaderConfig.Name}' is not a valid shader name, material creation aborted.");
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
            TextureShaderProperty textureShaderProperty = MaterialToCreate.ShaderConfig.TextureProperties.Find(textureProperty => textureProperty.RelatedTexture == textureType);
            if (textureShaderProperty != null)
            {
                if (material.HasTexture(textureShaderProperty.PropertyName))
                {
                    Texture2D finalTexture = TexturePackingService.ComputeTexture(downloadedTexture, textureShaderProperty.PackingMethod, MaterialToCreate.TexturesDirectory, MaterialToCreate.TexturesNamePrefix);
                    material.SetTexture(textureShaderProperty.PropertyName, finalTexture);
                }
            }
            
            // Enabling/disabling the keywords related to this texture
            foreach (var keywordParameter in MaterialToCreate.ShaderConfig.KeywordProperties.FindAll(keywordParam => keywordParam.RelatedTexture == textureType))
            {
                if (keywordParameter.Enable)
                {
                    material.EnableKeyword(keywordParameter.PropertyName);
                    continue;
                }
                material.DisableKeyword(keywordParameter.PropertyName);
            }
            
            // Setting the colors related to this texture
            foreach (var colorParameter in MaterialToCreate.ShaderConfig.ColorProperties.FindAll(colorParam => colorParam.RelatedTexture == textureType))
            {
                if (material.HasColor(colorParameter.PropertyName))
                {
                    material.SetColor(colorParameter.PropertyName, colorParameter.Color);
                }
            }
        }
        
        renderer.material = material;

        _onModelReady?.Invoke();
        _onModelReady = null;
    }
}