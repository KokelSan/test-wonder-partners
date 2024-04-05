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

public class ModelMaterialCreator : MonoBehaviour
{
    [SerializeField] private ModelTextureConfigSO TextureConfig;
    [SerializeField] private ShaderDescriptionTableSO ShaderDescription;

    private List<TextureDef> _downloadingTextures = new List<TextureDef>();
    private Dictionary<TextureType, DownloadedTexture> _downloadedTextures = new Dictionary<TextureType, DownloadedTexture>();
    private int _downloadFailsNb = 0;
    private Action _onMaterialCreated;

    private bool DownloadComplete => _downloadingTextures.Count == 0;
    
    public void StartMaterialCreation(Action onMaterialCreated)
    {
        _onMaterialCreated = onMaterialCreated;
        SetAllTexturesFullPath();

        if (TryLocateTextures())
        {
            
        }
        
        foreach (TextureDef textureDef in TextureConfig.Textures)
        {
            _downloadingTextures.Add(textureDef);
            StartCoroutine(TextureDownloadService.DownloadTexture(textureDef, OnTextureDownloaded));
        }
    }

    private void SetAllTexturesFullPath()
    {
        foreach (TextureDef textureDef in TextureConfig.Textures)
        {
            textureDef.Path = TextureConfig.CommonPath + textureDef.Type;
        }
    }
    
    private bool TryLocateTextures()
    {
        // foreach (TextureDef textureDef in TextureConfig.Textures)
        // {
        //     
        // }

        return false;
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
        
        Material material = new Material(shader)
        {
            globalIlluminationFlags = ShaderDescription.GIFlag,
        };

        foreach (ShaderTextureProperty shaderTextureProperty in ShaderDescription.TextureProperties)
        {
            if (_downloadedTextures.TryGetValue(shaderTextureProperty.TextureType, out DownloadedTexture downloadedTexture))
            {
                if (downloadedTexture.Texture != null)
                {
                    if (material.HasTexture(shaderTextureProperty.PropertyName))
                    {
                        Texture2D finalTexture = TexturePackingService.ComputeTexture(downloadedTexture, shaderTextureProperty.PackingMethod);
                        material.SetTexture(shaderTextureProperty.PropertyName, finalTexture);
                    }
                }
            }
        }
        
        foreach (var keywordParam in ShaderDescription.KeywordParameters)
        {
            if (keywordParam.Enable)
            {
                material.EnableKeyword(keywordParam.Keyword);
                continue;
            }
            material.DisableKeyword(keywordParam.Keyword);
        }
        
        foreach (var colorParam in ShaderDescription.ColorParameters)
        {
            if (material.HasColor(colorParam.Name))
            {
                material.SetColor(colorParam.Name, colorParam.Color);
            }
        }
        
        renderer.material = material;
        
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