using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ShaderTextureProperty
{
    public TextureType TextureType;
    public PackingMethod PackingMethod;
    public string PropertyName;
}

[Serializable]
public struct ShaderKeywordParameter
{
    public string Keyword;
    public bool Enable;
}

[Serializable]
public struct ShaderColorParameter
{
    public string Name;
    public Color Color;
}

/// <summary>
/// A scriptable object to bind a texture type with the corresponding texture property name in a shader
/// </summary>
[CreateAssetMenu(menuName = "Wonder Partner's/Shader Property Table/New Table",  fileName = "_ShaderPropertyTableSO")]
public class ShaderDescriptionTableSO : ScriptableObject
{
    public string Name;
    public List<ShaderTextureProperty> TextureProperties;
    
    [Header("Additional parameters")]
    public List<ShaderKeywordParameter> KeywordParameters;
    public List<ShaderColorParameter> ColorParameters;
    
    public MaterialGlobalIlluminationFlags GIFlag;
    
}