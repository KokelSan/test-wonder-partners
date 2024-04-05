using System;
using System.IO;
using UnityEngine;

public static class TexturePackingService
{
        public static Texture2D ComputeTexture(DownloadedTexture downloadedTexture, PackingMethod targetPackingMethod)
        {
                if (downloadedTexture.TextureDef.PackingMethod == targetPackingMethod)
                {
                        return downloadedTexture.Texture;
                }
                
                return TransformTexture(downloadedTexture, targetPackingMethod);
        }

        private static Texture2D TransformTexture(DownloadedTexture downloadedTexture, PackingMethod targetPackingMethod)
        {
                Debug.Log($"Transforming texture '{downloadedTexture.TextureDef.Type}' from {downloadedTexture.TextureDef.PackingMethod} to {targetPackingMethod}");
                
                if (downloadedTexture.TextureDef.PackingMethod == PackingMethod.glTF2 && targetPackingMethod == PackingMethod.Standard)
                {
                        Texture2D originalTexture = downloadedTexture.Texture;
                        Texture2D transformedTexture = new Texture2D(originalTexture.width, originalTexture.height);

                        for (int y = 0; y < originalTexture.height; y++)
                        {
                                for (int x = 0; x < originalTexture.width; x++)
                                {
                                        Color originalColor = originalTexture.GetPixel(x, y);
                                        Color transformedColor = new Color(originalColor.r, originalColor.g, originalColor.b, originalColor.g);
                                        transformedTexture.SetPixel(x, y, transformedColor);
                                }
                        }
                        transformedTexture.Apply();

                        string path = downloadedTexture.TextureDef.Path + "_Transformed" + downloadedTexture.TextureDef.Extension;
                        if (!FileIOService.TryCreateFile(path, transformedTexture.EncodeToPNG(), out string error))
                        {
                                Debug.LogError($"An error occured while saving transformed texture: {error}");
                        }
                        return transformedTexture;
                }
                return downloadedTexture.Texture;
        }
}