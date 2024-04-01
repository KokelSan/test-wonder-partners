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

    private void OnTextureDownloaded(TextureDef textureDef, Texture2D downloadedTexture)
    {
        _downloadingTextures.Remove(textureDef);
        _downloadedTextures.Add(textureDef.Type, new DownloadedTexture(textureDef, downloadedTexture));
        
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
        
        if (!TryGetComponent(out Renderer renderer))
        {
            Debug.LogError("This model has no renderer to receive the material");
            return;
        }
        
        renderer.material = new Material(shader);
        
        foreach (ShaderTextureProperty shaderTextureProperty in ShaderDescription.TextureProperties)
        {
            if (_downloadedTextures.TryGetValue(shaderTextureProperty.TextureType, out DownloadedTexture texture))
            {
                if (renderer.material.HasTexture(shaderTextureProperty.PropertyName))
                {
                    // renderer.material.EnableKeyword("");
                    Texture2D finalTexture = TexturePackingService.ComputeTexture(texture.Texture, texture.TextureDef.PackingMethod, shaderTextureProperty.PackingMethod);
                    renderer.material.SetTexture(shaderTextureProperty.PropertyName, finalTexture);
                }
            }
        }

        
    }
}