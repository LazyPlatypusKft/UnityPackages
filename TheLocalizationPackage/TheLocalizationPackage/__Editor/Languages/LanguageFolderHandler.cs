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
using System.IO;
using LazyPlatypus.TheFolderSupportPackage;
using UnityEditor;
using UnityEngine;

namespace LazyPlatypus.TheLocalizationPackage.Editor
{
    [Serializable]
    public class LanguageFolderHandler
    {
        [SerializeField] List<LanguageFolder> Languages = new List<LanguageFolder>();

        string mPath;
        List<LanguagePartitionSO> mPartitions;
        LanguageCreatorHandler mLanguageCreator;
        LanguagePartitionHandler mPartitionCreator;
        LanguageKeyHandler mKeyHandler;
        
        public LanguageFolderHandler(string _path, LanguageCreatorHandler _languages, LanguagePartitionHandler _partitionHandler, LanguageKeyHandler _keys, List<string> _dictionaries)
        {
            mPath = _path;
            
            mPartitionCreator = _partitionHandler;
            mKeyHandler = _keys;
            mPartitions = _partitionHandler.RetrievePartitions();
            mLanguageCreator = _languages;
            
            Languages = new List<LanguageFolder>();
            for (int i = 0; i < _dictionaries.Count; i++)
                Languages.Add(new LanguageFolder(_dictionaries[i]));
            
            mLanguageCreator.OnLanguageCreated += OnLanguageCreated;
            mPartitionCreator.OnPartitionCreated += OnPartitionCreated;
            mPartitionCreator.OnPartitionDeleted += OnPartitionDeleted;
            mPartitionCreator.OnPartitionModified += OnPartitionModified;
        }

        ~LanguageFolderHandler()
        {
            if (mLanguageCreator != null)
                mLanguageCreator.OnLanguageCreated -= OnLanguageCreated;
            if (mPartitionCreator != null)
            {
                mPartitionCreator.OnPartitionCreated -= OnPartitionCreated;
                mPartitionCreator.OnPartitionDeleted -= OnPartitionDeleted;
                mPartitionCreator.OnPartitionModified -= OnPartitionModified;
            }
        }

        void OnLanguageCreated(LanguageSO _newLanguage)
        {
            Languages.Add(new LanguageFolder(_newLanguage.ShortHand));

            var partitionPath = Path.Combine(mPath, _newLanguage.ShortHand, "_partitions");
            FolderStructures.EnsureFolderExists(partitionPath);
            
            foreach (var partition in mPartitions)
            {
                var localizedAsset = FolderStructures.CreateAsset<LocalizedLanguagePartitionSO>(partitionPath, $"{partition.KEY.ToLower()}_{_newLanguage.ShortHand}_part.asset", 
                    newLLPSO => newLLPSO.PartitionKey = partition);
                var dictionaryAsset = FolderStructures.CreateAsset<LoadableLanguageDictionarySO>(partitionPath, $"{partition.KEY.ToLower()}_{_newLanguage.ShortHand}.asset", 
                    newDict => newDict.TheType = partition);
                dictionaryAsset.TheDictionary = new List<LanguageKeyValueSO>();

                LanguageKeyCollection mCurrentCollection = null; 
                for (int i = 0; i < mKeyHandler.KeyCollections.Count; i++)
                {
                    if (mKeyHandler.KeyCollections[i].Partition.KEY == partition.KEY)
                    {
                        mCurrentCollection = mKeyHandler.KeyCollections[i];
                        break;
                    }
                }
                
                foreach (var key in mCurrentCollection.AllKeys)
                {
                    var originalPath = AssetDatabase.GetAssetPath(key);
                    var relativePathOfKey = originalPath.Substring(originalPath.IndexOf(LanguageFolderStructures.KEYS_FOLDER) + LanguageFolderStructures.KEYS_FOLDER.Length + 1);
                    var relativePathSplitted = relativePathOfKey.Split('/');
                    var relativePath = "";
                    var theKeyFileName = "";
                    for (int i = 0; i < relativePathSplitted.Length; i++)
                    {
                        if (i == 0)
                            relativePath += relativePathSplitted[i];
                        else if (i == relativePathSplitted.Length - 1)
                            theKeyFileName = relativePathSplitted[relativePathSplitted.Length - 1];
                        else 
                            relativePath += "/" + relativePathSplitted[i];
                    }

                    var shortKeyValue = partition.KEY.ToLower().Substring(0, Math.Min(partition.KEY.Length, 4));
                    var newAssetFolder = Path.Combine(mPath, _newLanguage.ShortHand, relativePath);
                    FolderStructures.EnsureFolderExists(newAssetFolder);
                    var newAssetName = $"{_newLanguage.ShortHand.ToLower()}_{shortKeyValue}_{key.name.ToLower()}.asset";
                    var keyValueAsset = FolderStructures.CreateAsset<LanguageKeyValueSO>(
                        newAssetFolder,
                            newAssetName,
                        asset =>
                        {
                            asset.TheKey = key;
                            var fileName = "en_" + shortKeyValue + "_" + theKeyFileName;
                            var englishPath = Path.Combine(mPath,  "en", relativePath);
                            var englishKeyValue = AssetDatabase.LoadAssetAtPath<LanguageKeyValueSO>(Path.Combine(englishPath, fileName));
                            if (englishKeyValue == null)
                            {
                                Debug.Log($"English value of {Path.Combine(englishPath, fileName)} could not be found. Sub parts: {partition.KEY.ToLower()}, {theKeyFileName}, {relativePath}");
                            }
                            asset.TheValue = englishKeyValue != null ? englishKeyValue.TheValue : string.Empty;
                        }
                    );
                    dictionaryAsset.TheDictionary.Add(keyValueAsset);
                }
                
                EditorUtility.SetDirty(dictionaryAsset);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
        
        void OnPartitionCreated(LanguagePartitionSO obj)
        {
            
        }
        
        void OnPartitionModified(string arg1, LanguagePartitionSO arg2)
        {
            
        }
        
        void OnPartitionDeleted(LanguagePartitionSO obj)
        {
            
        }
    }
}
#endif