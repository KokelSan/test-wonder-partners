using System;
using UnityEngine;

[Serializable]
public class BaseShaderProperty
{
    public string PropertyName;
    public TextureType RelatedTexture;
}

[Serializable]
public class TextureShaderProperty : BaseShaderProperty
{
    public PackingMethod PackingMethod;
}

[Serializable]
public class KeywordShaderProperty : BaseShaderProperty
{
    public bool Enable;
}

[Serializable]
public class ColorShaderProperty : BaseShaderProperty
{
    public Color Color;
}