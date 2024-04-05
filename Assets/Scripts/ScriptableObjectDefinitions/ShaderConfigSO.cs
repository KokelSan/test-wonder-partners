using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A scriptable object to bind a general texture type with the corresponding texture property name defined in the shader
/// </summary>
[CreateAssetMenu(menuName = "Wonder Partner's/Shader Config/New Config",  fileName = "_ShaderConfig")]
public class ShaderConfigSO : ScriptableObject
{
    public string Name;
    public List<TextureShaderProperty> TextureProperties;
    
    [Header("Additional Parameters")]
    public List<KeywordShaderProperty> KeywordProperties;
    public List<ColorShaderProperty> ColorProperties;
    
    public MaterialGlobalIlluminationFlags GIFlag;
}