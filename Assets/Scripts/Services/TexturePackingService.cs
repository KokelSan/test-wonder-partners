using System.IO;
using UnityEngine;

public static class TexturePackingService
{
        public static Texture2D ComputeTexture(Texture2D texture, PackingMethod currentPackingMethod, PackingMethod targetPackingMethod)
        {
                if (currentPackingMethod == targetPackingMethod)
                {
                        return texture;
                }
                
                return TransformTexture(texture, currentPackingMethod, targetPackingMethod);
        }

        private static Texture2D TransformTexture(Texture2D texture, PackingMethod currentPackingMethod, PackingMethod targetPackingMethod)
        {
                Debug.Log($"Transforming texture from {currentPackingMethod} to {targetPackingMethod}");
                
                if (currentPackingMethod == PackingMethod.glTF2 && targetPackingMethod == PackingMethod.Standard)
                {
                        Texture2D invertedTexture = new Texture2D(texture.width, texture.height);

                        for (int y = 0; y < texture.height; y++)
                        {
                                for (int x = 0; x < texture.width; x++)
                                {
                                        Color originalColor = texture.GetPixel(x, y);
                                        Color invertedColor = new Color(originalColor.r, originalColor.g, originalColor.b, originalColor.g);
                                        invertedTexture.SetPixel(x, y, invertedColor);
                                }
                        }

                        invertedTexture.Apply();
                        
                        byte[] textureBytes = invertedTexture.EncodeToPNG();
                        File.WriteAllBytes("Assets/Textures/DamagedHelmet/ModifiedTextures/invertedTexture.png", textureBytes);
                        
                        return invertedTexture;
                }
                return texture;
        }
}