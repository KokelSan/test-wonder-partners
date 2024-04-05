using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class FileIOService
{
        private static List<string> _createdFilePaths = new List<string>();
        
        public static bool TryCreateFile(string path, byte[] byteArray, out string errorMsg)
        {
                try
                {
                        File.WriteAllBytes(path, byteArray);
                }
                catch (Exception e)
                {
                        errorMsg = e.Message;
                        return false;
                }

                errorMsg = string.Empty;
                _createdFilePaths.Add(path);
                return true;
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