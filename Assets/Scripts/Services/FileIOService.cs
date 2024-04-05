using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class FileIOService
{
        private static List<string> _createdFilePaths = new List<string>();
        
        public static void CreateFile(string directory, string fileName, byte[] byteArray)
        {
                string path = $"{directory}/{fileName}";
                
                try
                {
                        if (!File.Exists(directory))
                        {
                                Directory.CreateDirectory(directory);
                        }
                        
                        File.WriteAllBytes(path, byteArray);
                }
                catch (Exception e)
                {
                        Debug.LogError($"File creation for texture '{fileName}' failed: {e.Message}.");
                        return;
                }

                _createdFilePaths.Add(path);
        }

        public static bool TryLoadImage(string path, out Texture2D texture)
        {
                texture = new Texture2D(2, 2);
                if (File.Exists(path))
                {
                        try
                        {
                                byte[] bytes = File.ReadAllBytes(path);
                                if (texture.LoadImage(bytes))
                                {
                                        _createdFilePaths.Add(path);
                                        return true;
                                }
                        }
                        catch (Exception e)
                        {
                                Debug.LogError($"Failed to load image at '{path}': {e.Message}.");
                        }
                }
                return false;
        }
        
        public static void DeleteAllCreatedFiles()
        {
                foreach (string path in _createdFilePaths)
                {
                        try
                        {
                                File.Delete(path);
                                File.Delete(path + ".meta"); // No exception thrown if file doesn't exist, no additional try catch needed
                        }
                        catch (Exception e)
                        {
                                Debug.LogError($"File deletion failed: {e.Message} \nPath: {path}");
                        }
                }
                _createdFilePaths.Clear();
        }
}