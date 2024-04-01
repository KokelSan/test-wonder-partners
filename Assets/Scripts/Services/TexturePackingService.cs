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
                
                return InverseTexture(texture, currentPackingMethod, targetPackingMethod);
        }

        private static Texture2D InverseTexture(Texture2D texture, PackingMethod currentPackingMethod, PackingMethod targetPackingMethod)
        {
                Debug.Log("InverseTexture");
                
                if (currentPackingMethod == PackingMethod.glTF2 && targetPackingMethod == PackingMethod.Standard)
                {
                        Debug.Log("Inverting G and A canals");
                        
                        // Texture2D rTexture = new Texture2D(texture.width, texture.height);
                        // Texture2D gTexture = new Texture2D(texture.width, texture.height);
                        // Texture2D bTexture = new Texture2D(texture.width, texture.height);
                        // Texture2D aTexture = new Texture2D(texture.width, texture.height);
                        
                        Texture2D invertedTexture = new Texture2D(texture.width, texture.height);

                        for (int y = 0; y < texture.height; y++)
                        {
                                for (int x = 0; x < texture.width; x++)
                                {
                                        Color originalColor = texture.GetPixel(x, y);
                                        Color invertedColor = new Color(originalColor.r, originalColor.g, originalColor.b, originalColor.g);
                                        invertedTexture.SetPixel(x, y, invertedColor);
                                        
                                        //////
                                        // rTexture.SetPixel(x, y, new Color(originalColor.r,0, 0, 1));
                                        // gTexture.SetPixel(x, y, new Color(0,originalColor.g, 0, 1));
                                        // bTexture.SetPixel(x, y, new Color(0,0, originalColor.b, 1));
                                        // aTexture.SetPixel(x, y, new Color(originalColor.a,0, 0, 1));
                                }
                        }

                        invertedTexture.Apply();
                        
                        byte[] textureBytes = invertedTexture.EncodeToPNG();
                        File.WriteAllBytes("Assets/Textures/DamagedHelmet/ModifiedTextures/invertedTexture.png", textureBytes);
                        
                        // //////////
                        // textureBytes = rTexture.EncodeToPNG();
                        // File.WriteAllBytes("Assets/Textures/DamagedHelmet/ModifiedTextures/rTexture.png", textureBytes);
                        //
                        // textureBytes = gTexture.EncodeToPNG();
                        // File.WriteAllBytes("Assets/Textures/DamagedHelmet/ModifiedTextures/gTexture.png", textureBytes);
                        //
                        // textureBytes = bTexture.EncodeToPNG();
                        // File.WriteAllBytes("Assets/Textures/DamagedHelmet/ModifiedTextures/bTexture.png", textureBytes);
                        //
                        // textureBytes = aTexture.EncodeToPNG();
                        // File.WriteAllBytes("Assets/Textures/DamagedHelmet/ModifiedTextures/aTexture.png", textureBytes);
                        
                        return invertedTexture;
                }
                
                return texture;
        }
}