/**************************************************************************
 *  Lazy Platypus Kft. - Game Development Studio
 *  
 *  Copyright © 2025 Lazy Platypus Kft. All rights reserved.
 *  
 *  This script is part of the Lazy Platypus Kft. game framework and is 
 *  protected under applicable copyright laws. Unauthorized use, 
 *  distribution, or modification of this file is strictly prohibited.
 *  
 *  For licensing inquiries, please contact: info@lazyplatypus.com
 *  
 *  ───────────────────────────────────────────────────────────────
 *  "Games with Heart, Crafted with Code." - Lazy Platypus Kft.
 *  ───────────────────────────────────────────────────────────────
 **************************************************************************/

#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LazyPlatypus.TheFolderSupportPackage
{
    public class FolderStructures
    {        
        const string CONFIGURATIONS_FOLDER = "Assets/LazyPlatypus/Configurations/";

        // Ensures the given path exists; creates it if it doesn’t
        public static void EnsureFolderExists(string _path)
        {
            if (!AssetDatabase.IsValidFolder(_path))
            {
                string parent = Path.GetDirectoryName(_path);
                string folderName = Path.GetFileName(_path);

                if (parent != null)
                {
                    EnsureFolderExists(parent);
                    AssetDatabase.CreateFolder(parent, folderName);
                }
            }
        }

        // Converts a full path to a relative path under Assets
        public static string GetRelativeAssetsPath(string _path)
        {
            if (_path.StartsWith(Application.dataPath))
            {
                return "Assets" + _path.Substring(Application.dataPath.Length).Replace("\\", "/");
            }
            return _path;
        }

        // Detects the relative display name of a folder
        public static string GetDisplayName(string _path)
        {
            if (_path == "Assets") return "*";
            return Path.GetFileName(_path);
        }
        
        // Retrieves or creates a FolderConfigurationSO with the specified linkedAssetId
        public static T RetrieveConfigurationAsset<T>(string _linkedAssetId) where T : FolderConfigurationSO
        {
            // Search for existing FolderConfigurationSO
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}", new[] { CONFIGURATIONS_FOLDER });
            
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                T asset = AssetDatabase.LoadAssetAtPath<T>(path);
                if (asset != null && asset.linkedAssetId == _linkedAssetId)
                    return asset;
            }

            // Create new FolderConfigurationSO if not found
            var assetPath = Path.Combine(CONFIGURATIONS_FOLDER, $"{_linkedAssetId}_FolderConfiguration.asset");
            if (AssetDatabase.IsValidFolder(assetPath))
                assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);

            // Usually fails, but honestly it creates the folder so who cares, without it, the CreateAsset would fail
            EnsureFolderExists(CONFIGURATIONS_FOLDER);
            
            T newAsset = ScriptableObject.CreateInstance<T>();
            newAsset.linkedAssetId = _linkedAssetId;
            AssetDatabase.CreateAsset(newAsset, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return newAsset;
        }

        public static T CreateAsset<T>(string _path, string _fileName, Action<T> _assetSetup = null) where T : ScriptableObject
        {
            var newAssetOfScriptableObject = ScriptableObject.CreateInstance<T>();
            _assetSetup?.Invoke(newAssetOfScriptableObject);
            var fullPath = Path.Combine(_path, _fileName);
            if (!AssetDatabase.IsValidFolder(_path))
                EnsureFolderExists(_path);
            else
                fullPath = AssetDatabase.GenerateUniqueAssetPath(fullPath);
            
            AssetDatabase.CreateAsset(newAssetOfScriptableObject, fullPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            return newAssetOfScriptableObject;
        }

        public static void RenameAsset<T>(T _asset, string _newName, Action<T> _specialSetup = null) where T : ScriptableObject
        {
            var oldPath = AssetDatabase.GetAssetPath(_asset);
            var newPath = Path.Combine(Path.GetDirectoryName(oldPath), $"{_newName}Partition.asset");
            newPath = AssetDatabase.GenerateUniqueAssetPath(newPath);

            AssetDatabase.RenameAsset(oldPath, Path.GetFileNameWithoutExtension(newPath));
            
            if (_specialSetup != null)
                _specialSetup?.Invoke(_asset);
            
            EditorUtility.SetDirty(_asset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void DeletingGenericAsset<T>(T _asset, Action _specialTeardown = null) where T : ScriptableObject
        {
            if (_asset == null)
                return;
            
            var assetPath = AssetDatabase.GetAssetPath(_asset);
            if (string.IsNullOrEmpty(assetPath))
                return;
            
            var deleteConfirmed = EditorUtility.DisplayDialog(
                "Deleting Asset",
                $"Are you sure you want to delete '{_asset.name}'?",
                "Delete",
                "Cancel"
            );

            if (deleteConfirmed)
            {
                _specialTeardown?.Invoke();
                AssetDatabase.DeleteAsset(assetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        public static List<T> FindAssets<T>(string _folder, string _type, bool _recursive = true) where T : ScriptableObject
        {
            var foundAsset = new List<T>();
            var guids = AssetDatabase.FindAssets(_type, new[] { _folder });

            if (_recursive)
            {
                foreach (string guid in guids)
                {
                    var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    T keyAsset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                    if (keyAsset != null)
                        foundAsset.Add(keyAsset);
                }
            }
            else
            {
                foreach (string guid in guids)
                {
                    var assetPath = AssetDatabase.GUIDToAssetPath(guid);

                    var unifiedPathsAssetPath = Path.GetDirectoryName(assetPath).Replace("\\", "/");
                    var filteredUnifiedPathAsset = "";
                    var splittedPath = unifiedPathsAssetPath.Split('/');
                    for (int i = 0; i < splittedPath.Length; i++)
                    {
                        if (i == splittedPath.Length - 1)
                            break;
                        if (i != 0)
                            filteredUnifiedPathAsset += "/" + splittedPath[i];
                        else
                            filteredUnifiedPathAsset += splittedPath[i];
                    }
                    
                    var unifiedPathsFolderPath = Path.GetDirectoryName(_folder).Replace("\\", "/");
                    if (filteredUnifiedPathAsset == unifiedPathsFolderPath)
                    {
                        T keyAsset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                        if (keyAsset != null)
                            foundAsset.Add(keyAsset);
                    }
                }
            }
            return foundAsset;
        }
    }
}
#endif