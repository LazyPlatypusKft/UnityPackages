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

using System;
using System.Collections.Generic;
using LazyPlatypus.TheFolderSupportPackage;
using UnityEditor;
using UnityEngine;

namespace LazyPlatypus.TheLocalizationPackage.Editor
{
    [Serializable]
    public class LanguageKeyHandler
    {
        public List<LanguageKeyCollection> KeyCollections;
        public LanguagePartitionHandler mPartitionHandler;
        
        string mPath;
        
        public LanguageKeyHandler(string _path, LanguagePartitionHandler _handler)
        {
            mPath = _path;
            mPartitionHandler = _handler;
            mPartitionHandler.OnPartitionCreated += HandlePartitionCreated;
            mPartitionHandler.OnPartitionModified += HandlePartitionModified;
            mPartitionHandler.OnPartitionDeleted += HandlePartitionDeleted;

            var _partitions = mPartitionHandler.RetrievePartitions();
            var keysFolderPath = System.IO.Path.Combine(mPath, LanguageFolderStructures.KEYS_FOLDER);

            var existingFolders = AssetDatabase.GetSubFolders(keysFolderPath);
            var existingFolderNames = new HashSet<string>();
            foreach (string folder in existingFolders)
                existingFolderNames.Add(System.IO.Path.GetFileName(folder).ToLower());

            KeyCollections = new List<LanguageKeyCollection>();
            foreach (var partition in _partitions)
            {
                var folderName = partition.KEY.ToLower();
                var fullFolderPath = System.IO.Path.Combine(keysFolderPath, folderName);

                if (!AssetDatabase.IsValidFolder(fullFolderPath))
                    FolderStructures.EnsureFolderExists(fullFolderPath);
                else
                    existingFolderNames.Remove(folderName);
                KeyCollections.Add(new LanguageKeyCollection(partition, fullFolderPath));
            }

            if (existingFolderNames.Count > 0)
            {
                var extraFolders = string.Join(", ", existingFolderNames);
                Debug.LogWarning($"There are additional folder(s) in the _keys folder {extraFolders}. Remove them if they are not deemed necessary!");
            }
        }

        ~LanguageKeyHandler()
        {
            if (mPartitionHandler != null)
            {
                mPartitionHandler.OnPartitionCreated -= HandlePartitionCreated;
                mPartitionHandler.OnPartitionModified -= HandlePartitionModified;
                mPartitionHandler.OnPartitionDeleted -= HandlePartitionDeleted;
            }
        }

        void HandlePartitionCreated(LanguagePartitionSO _partition)
        {
            var folderName = _partition.KEY.ToLower();
            var fullFolderPath = System.IO.Path.Combine(mPath, LanguageFolderStructures.KEYS_FOLDER, folderName);
            FolderStructures.EnsureFolderExists(fullFolderPath);
            KeyCollections.Add(new LanguageKeyCollection(_partition));
        }

        void HandlePartitionModified(string _oldName, LanguagePartitionSO _newPartition)
        {
            var oldFolderName = _oldName.ToLower();
            var newFolderName = _newPartition.KEY.ToLower();
            var oldFolderPath = System.IO.Path.Combine(mPath, LanguageFolderStructures.KEYS_FOLDER, oldFolderName);
            var newFolderPath = System.IO.Path.Combine(mPath, LanguageFolderStructures.KEYS_FOLDER, newFolderName);

            if (AssetDatabase.IsValidFolder(oldFolderPath))
                AssetDatabase.MoveAsset(oldFolderPath, newFolderPath);
            
            foreach (var keyCollection in KeyCollections)
            {
                if (keyCollection.Partition.KEY.Equals(_oldName, StringComparison.OrdinalIgnoreCase))
                {
                    keyCollection.Partition = _newPartition;
                    break;
                }
            }
        }

        void HandlePartitionDeleted(LanguagePartitionSO _partition)
        {
            var folderName = _partition.KEY.ToLower();
            var folderPath = System.IO.Path.Combine(mPath, LanguageFolderStructures.KEYS_FOLDER, folderName);
            
            if (AssetDatabase.IsValidFolder(folderPath))
                AssetDatabase.DeleteAsset(folderPath);

            for (int i = 0; i < KeyCollections.Count; i++)
            {
                if (KeyCollections[i].Partition.KEY == _partition.KEY)
                {
                    KeyCollections.RemoveAt(i);
                    break;
                }
            }
        }
    }
}
#endif