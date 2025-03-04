/**************************************************************************
 *  Lazy Platypus Kft. - Game Development Studio
 *
 *  Copyright © 2025 Lazy Platypus Kft. All rights reserved.
 *
 *  This script is part of the Lazy Platypus Kft. game framework and is
 *  protected under applicable copyright laws. Unauthorized use,
 *  distribution, or modification of this file is strictly prohibited.
 *
 *  MIT, can be included in commercial products and modified
 *  If you feel like, you can support on our website:
 *  https://www.lazyplatypus.com
 *
 *  ───────────────────────────────────────────────────────────────
 *  "Use this and bake nice things!" - Lazy Platypus Kft.
 *  ───────────────────────────────────────────────────────────────
 **************************************************************************/

#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using LazyPlatypus.TheFolderSupportPackage;
using UnityEditor;

namespace LazyPlatypus.TheLocalizationPackage.Editor
{
    public static class LanguageFolderStructures
    {
        public const string KEYS_FOLDER = "_keys";
        public const string PARTITIONS_FOLDER = "_partitions";
        
        public static void EnsureMandatoryFolderExists(string basePath)
        {
            var keysFolderPath = Path.Combine(basePath, KEYS_FOLDER);
            CreateFolder(keysFolderPath);
            
            var partitionsFolderPath = Path.Combine(basePath, PARTITIONS_FOLDER);
            CreateFolder(partitionsFolderPath);
        }

        static void CreateFolder(string _path)
        {
            if (!AssetDatabase.IsValidFolder(_path))
                FolderStructures.EnsureFolderExists(_path);
        }
        
        public static List<LanguagePartitionSO> LoadLanguagePartitions(string _basePath)
        {
            var partitionsFolderPath = Path.Combine(_basePath, "_partitions");
            var partitions = new List<LanguagePartitionSO>();
            if (AssetDatabase.IsValidFolder(partitionsFolderPath))
            {
                string[] guids = AssetDatabase.FindAssets("t:LanguagePartitionSO", new[] { partitionsFolderPath });
                foreach (string guid in guids)
                {
                    var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    var partition = AssetDatabase.LoadAssetAtPath<LanguagePartitionSO>(assetPath);
                    if (partition != null)
                        partitions.Add(partition);
                }
            }
            return partitions;
        }
        
        public static List<string> ParseLanguageFolders(string basePath)
        {
            var folderNames = new List<string>();
            if (AssetDatabase.IsValidFolder(basePath))
            {
                var subfolders = AssetDatabase.GetSubFolders(basePath);
                foreach (string folder in subfolders)
                {
                    var folderName = Path.GetFileName(folder);
                    if (folderName != KEYS_FOLDER && folderName != PARTITIONS_FOLDER)
                        folderNames.Add(folderName);
                }
            }
            return folderNames;
        }

        public static LanguageSO LoadLanguageAt(string _basePath, string _subfolder)
        {
            var fullPath = Path.Combine(_basePath, _subfolder);

            if (!AssetDatabase.IsValidFolder(fullPath))
                return null;

            var guids = AssetDatabase.FindAssets("t:LanguageSO", new[] { fullPath });
            if (guids.Length == 0)
                return null;

            var assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            var language = AssetDatabase.LoadAssetAtPath<LanguageSO>(assetPath);
            return language;
        }
    }
}
#endif