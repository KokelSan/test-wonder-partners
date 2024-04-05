using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum TextureType
{
    BaseMap,
    Emissive,
    MetallicRoughness,
    Normal,
    Occlusion
}

public enum ImageFormat
{
    png,
    jpg
}

public enum PackingMethod
{
    Standard,
    glTF2,
}

[Serializable]
public class TextureDef
{
    public TextureType Type;
    public ImageFormat Format;
    public PackingMethod PackingMethod;
    public string URL;
    
    public string Extension => $".{Format}";
    [HideInInspector] public string Path; // the full path without extension, allowing insertion before extension
} 

[CreateAssetMenu(menuName = "Wonder Partner's/Model Texture Config/New Config",  fileName = "_ModelTextureConfigSO")]
public class ModelTextureConfigSO : ScriptableObject
{
    public List<TextureDef> Textures;
    public string SavePath;
    
    [Tooltip("The final name of the downloaded texture will be TexturesPrefixName + TextureType + Format. For example: DamagedHelmet_BaseMap.png")]
    public string TexturesPrefixName;
    
    public string CommonPath => SavePath + TexturesPrefixName;
}