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
    public class LanguagePartitionHandler
    {
        public event Action<LanguagePartitionSO> OnPartitionCreated;
        public event Action<string, LanguagePartitionSO> OnPartitionModified;
        public event Action<LanguagePartitionSO> OnPartitionDeleted;
        
        [SerializeField] List<LanguagePartitionSO> Partitions;
        
        bool mIsEditorOpen = false;
        string mNewPartitionName = "";
        string mLocalPath;
        LanguagePartitionSO mSelectedPartition = null;

        public LanguagePartitionHandler(string _localPath, List<LanguagePartitionSO> _partitions)
        {
            Partitions = _partitions;
            mLocalPath = _localPath;
        }

        public virtual void MyOnGUI()
        { 
            EditorGUILayout.Space();
            
            if (!mIsEditorOpen)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Language Partition Editor:", EditorStyles.boldLabel);
                if (GUILayout.Button("Open", GUILayout.Width(100)))
                    mIsEditorOpen = true;
                EditorGUILayout.EndHorizontal();
                return;
            }
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);

            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Language Partition Editor:", EditorStyles.boldLabel);

            GUILayout.Label("Partitions:", EditorStyles.label);
            foreach (var partition in Partitions)
                if (GUILayout.Button(partition.KEY))
                    OpenPartitionPopup(partition);
                
            EditorGUILayout.Space();
            if (GUILayout.Button("Create New Partition", GUILayout.Width(200)))
                OpenPartitionPopup(null);
            if (GUILayout.Button("Close", GUILayout.Width(100)))
                mIsEditorOpen = false;

            EditorGUILayout.EndVertical();
            EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
        }

        public List<LanguagePartitionSO> RetrievePartitions()
        {
            return Partitions;
        }

        void OpenPartitionPopup(LanguagePartitionSO partition)
        {
            mSelectedPartition = partition;
            mNewPartitionName = partition != null ? partition.KEY : string.Empty;

            PartitionCreatePopUp.Show(mNewPartitionName, partition == null, (actionType, newName) =>
            {
                switch (actionType)
                {
                    case 0:
                        CreateNewPartition(newName);
                        break;
                    case 1:
                        RenamePartition(partition, newName);
                        break;
                    case 2:
                        FolderStructures.DeletingGenericAsset(mSelectedPartition, () => RefreshPartitionsList(mSelectedPartition));
                        break;
                }
            });
        }

        void RefreshPartitionsList(LanguagePartitionSO _partition)
        {
            OnPartitionDeleted?.Invoke(_partition);
            Partitions.Remove(_partition);
        }

        void CreateNewPartition(string name)
        {
            var newPartition = FolderStructures.CreateAsset<LanguagePartitionSO>(Path.Combine(mLocalPath, LanguageFolderStructures.PARTITIONS_FOLDER), $"{name}Partition.asset", newPartition => newPartition.KEY = name.ToUpper());
            Partitions.Add(newPartition);
            OnPartitionCreated?.Invoke(newPartition);
        }
        
        void RenamePartition(LanguagePartitionSO _partition, string _newName)
        {
            var oldName = _partition.KEY;
            FolderStructures.RenameAsset(_partition, _newName, theRenamedPartition => theRenamedPartition.KEY = _newName.ToUpper());
            OnPartitionModified?.Invoke(oldName, _partition);
        }
    }
}
#endif