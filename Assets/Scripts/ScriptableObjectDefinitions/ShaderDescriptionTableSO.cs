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

/// <summary>
/// A scriptable object to bind a texture type with the corresponding texture property name in a shader
/// </summary>
[CreateAssetMenu(menuName = "Wonder Partner's/Shader Property Table/New Table",  fileName = "_ShaderPropertyTableSO")]
public class ShaderDescriptionTableSO : ScriptableObject
{
    public string Name;
    public List<ShaderTextureProperty> TextureProperties;
}