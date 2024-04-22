using UnityEngine;

public static class TexturePackingService
{
        public static Texture2D ComputeTexture(DownloadedTexture downloadedTexture, PackingMethod targetPackingMethod, string directory, string fileNamePrefix)
        {
                if (downloadedTexture.TextureDef.PackingMethod == targetPackingMethod)
                {
                        return downloadedTexture.Texture;
                }
                
                return TransformTexture(downloadedTexture, targetPackingMethod, directory, fileNamePrefix);
        }

        private static Texture2D TransformTexture(DownloadedTexture downloadedTexture, PackingMethod targetPackingMethod, string directory, string fileNamePrefix)
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
                                        Color transformedColor = new Color(originalColor.b, 0, 0, -originalColor.g);
                                        transformedTexture.SetPixel(x, y, transformedColor);
                                }
                        }
                        transformedTexture.Apply();

                        string fileName = fileNamePrefix + downloadedTexture.TextureDef.Type + "_Transformed" + downloadedTexture.TextureDef.Extension;
                        FileIOService.CreateFile(directory, fileName, transformedTexture.EncodeToPNG());
                                
                        return transformedTexture;
                }
                return downloadedTexture.Texture;
        }
}