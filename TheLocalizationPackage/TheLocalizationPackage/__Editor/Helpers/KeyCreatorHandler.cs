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
using UnityEditor;
using UnityEngine;

namespace LazyPlatypus.TheLocalizationPackage.Editor
{
    public class KeyCreatorHandler
    {
        string mPath;
        LanguageKeyHandler mKeyHandler;
        LanguageCreatorHandler mLanguagesToLoad;
        LanguagePartitionHandler mPartitions;

        bool mIsCreatingKey = false;
        List<LanguagePartitionSO> mLoadedPartitions;
        LanguagePartitionSO mSelectedPartition;
        
        public KeyCreatorHandler(string _path, LanguageKeyHandler _keyHandler, LanguageCreatorHandler _languagesToLoad, LanguagePartitionHandler _partitions)
        {
            mPath = _path;
            mKeyHandler = _keyHandler;
            mLanguagesToLoad = _languagesToLoad;
            mPartitions = _partitions;
        }

        public void MyOnGUI()
        {
            if (mPartitions.RetrievePartitions().Count > 0)
            {
                if (mIsCreatingKey)
                {
                    EditorGUILayout.LabelField("Select partition:", EditorStyles.boldLabel);
                    for (int i = 0; i < mLoadedPartitions.Count; i++)
                    {
                        if (GUILayout.Button("Create Key For " + mLoadedPartitions[i].KEY))
                        {
                            mSelectedPartition = mLoadedPartitions[i];
                            AssociateTMPTextWithLanguagesPopUp.Show(mLoadedPartitions[i], mKeyHandler, mLanguagesToLoad, mPath, null, NewKeyHasBeenAdded, true);
                            mIsCreatingKey = false;
                        }
                    }
                }
                else
                {
                    if (GUILayout.Button("Create New Translation Key"))
                    {
                        mIsCreatingKey = true;
                        mLoadedPartitions = mPartitions.RetrievePartitions();
                    }
                }
            }
        }

        void NewKeyHasBeenAdded(LanguageKeySO _key)
        {
            for (int i = 0; i < mKeyHandler.KeyCollections.Count; i++)
            {
                if (mKeyHandler.KeyCollections[i].Partition == mSelectedPartition)
                {
                   mKeyHandler.KeyCollections[i].AllKeys.Add(_key); 
                   return;
                }
            }
            
        }
    }
}
#endif