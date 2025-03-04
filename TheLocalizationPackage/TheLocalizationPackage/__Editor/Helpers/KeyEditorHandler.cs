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
    public class KeyEditorHandler
    {
        string mPath;
        LanguageKeyHandler mKeyHandler;
        LanguageCreatorHandler mLanguagesToLoad;
        LanguagePartitionHandler mPartitions;

        bool mIsCreatingKey = false;
        bool mIsEditingKey = false;
        List<LanguagePartitionSO> mLoadedPartitions;
        LanguagePartitionSO mSelectedPartition;
        List<LanguageKeySO> mKeys;
        Vector3 mScrollPosition;
        Vector3 mScrollPosition2;
        
        public KeyEditorHandler(string _path, LanguageKeyHandler _keyHandler, LanguageCreatorHandler _languagesToLoad, LanguagePartitionHandler _partitions)
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
                if (mIsEditingKey)
                {
                    EditorGUILayout.LabelField("Select key to edit:", EditorStyles.boldLabel);
                    mScrollPosition = EditorGUILayout.BeginScrollView(mScrollPosition, GUILayout.Height(300));
                    for (int i = 0; i < mKeys.Count; i++)
                    {
                        if (GUILayout.Button("Edit " + mKeys[i].KEY))
                        {
                            ChangeTheTranslationOfKeyPopUp.Show(mKeys[i], mLanguagesToLoad, mPath);
                            mIsEditingKey = false;
                            break;
                        }
                    }
                    EditorGUILayout.EndScrollView();
                }
                else
                {
                    if (mIsCreatingKey)
                    {
                        EditorGUILayout.LabelField("Select partition:", EditorStyles.boldLabel);
                        mScrollPosition2 = EditorGUILayout.BeginScrollView(mScrollPosition2, GUILayout.Height(300));
                        for (int i = 0; i < mLoadedPartitions.Count; i++)
                        {
                            if (GUILayout.Button("Edit Key For " + mLoadedPartitions[i].KEY))
                            {
                                for (int j = 0; j < mKeyHandler.KeyCollections.Count; j++)
                                {
                                    if (mKeyHandler.KeyCollections[i].Partition == mLoadedPartitions[i])
                                    {
                                        mSelectedPartition = mLoadedPartitions[i];
                                        mKeys = mKeyHandler.KeyCollections[i].AllKeys;
                                        mIsCreatingKey = false;
                                        mIsEditingKey = true;
                                    }
                                }
                            }
                        }
                        EditorGUILayout.EndScrollView();
                    }
                    else
                    {
                        if (GUILayout.Button("Edit Translation Key"))
                        {
                            mIsCreatingKey = true;
                            mLoadedPartitions = mPartitions.RetrievePartitions();
                        }
                    }
                }
            }

            if (mIsCreatingKey || mIsEditingKey)
            {
                if (GUILayout.Button("Cancel"))
                {
                    mIsCreatingKey = false;
                    mIsEditingKey = false;
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