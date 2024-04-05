using System;
using System.Collections.Generic;
using UnityEngine;

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
} 

[CreateAssetMenu(menuName = "Wonder Partner's/Material Config/New Config",  fileName = "_MaterialConfig")]
public class MaterialConfigSO : ScriptableObject
{
    [Header("Textures To Download")]
    public List<TextureDef> Textures;
    
    [Header("Textures File Creation")]
    public string TexturesDirectory;
    
    [Tooltip("The final name of a downloaded texture will be (TexturesDirectory/) TexturesNamePrefix + TextureType + Extension. For example: DamagedHelmet_BaseMap.png")]
    public string TexturesNamePrefix;
    
    [Header("Shader Config")]
    public ShaderConfigSO ShaderConfig;
}