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

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

namespace LazyPlatypus.TheLocalizationPackage.Editor
{
    public class LanguageKeysToModify
    {
        public ScriptableObject TheMainObject;
        public string FieldName;
        public LanguageKeySO Value;

        LanguagePartitionHandler mPartitionHandler;
        LanguageKeyHandler mKeyHandler;
        LanguageCreatorHandler mLanguages;
        string mPath;

        int mSelectedPartitionIndex = 0;
        LanguagePartitionSO mTipe;
        List<LanguagePartitionSO> mAllPartitions;
        string[] mAllPartitionsOptions;
        
        public LanguageKeysToModify(ScriptableObject _theMainObject, string _fieldName, LanguageKeySO _value, LanguagePartitionHandler _partitions, LanguageKeyHandler _keyHandler, LanguageCreatorHandler _languagesToLoad, string _path)
        {
            TheMainObject = _theMainObject;
            FieldName = _fieldName;
            Value = _value;
            
            mPartitionHandler = _partitions;
            mKeyHandler = _keyHandler;
            mLanguages = _languagesToLoad;
            mPath = _path;
            
            mAllPartitions = mPartitionHandler.RetrievePartitions();
            mAllPartitionsOptions = new string[mAllPartitions.Count];
            for (int i = 0; i < mAllPartitions.Count; i++)
                mAllPartitionsOptions[i] = mAllPartitions[i].KEY;
            UpdateTipe();
        }

        public void ShowMyGUI()
        {
            if (mTipe == null)
            {
                EditorGUILayout.HelpBox("Select a Partition before continuing", MessageType.Info);
                EditorGUI.BeginChangeCheck();
                mSelectedPartitionIndex = EditorGUILayout.Popup(mSelectedPartitionIndex, mAllPartitionsOptions);
                if (EditorGUI.EndChangeCheck())
                    UpdateTipe();
                return;
            }
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(FieldName);
            if (Value == null)
            {
                if (GUILayout.Button("Create Key", GUILayout.Width(100)))
                    AssociateTMPTextWithLanguagesPopUp.Show(mTipe, mKeyHandler, mLanguages, mPath, null, AddKeyToScriptableObject);
            }
            else
            {
                if (GUILayout.Button("Change Key", GUILayout.Width(100)))
                    AssociateTMPTextWithLanguagesPopUp.Show(mTipe, mKeyHandler, mLanguages, mPath, null, AddKeyToScriptableObject);
                if (GUILayout.Button("Change Translation", GUILayout.Width(150)))
                    ChangeTheTranslationOfKeyPopUp.Show(Value, mLanguages, mPath);
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            EditorGUI.BeginChangeCheck();
            mSelectedPartitionIndex = EditorGUILayout.Popup(mSelectedPartitionIndex, mAllPartitionsOptions);
            if (EditorGUI.EndChangeCheck())
                UpdateTipe();
            EditorGUILayout.Space();
        }

        void AddKeyToScriptableObject(LanguageKeySO _newValue)
        {
            Value = _newValue;
            
            Type type = TheMainObject.GetType();
            FieldInfo field = type.GetField(FieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (field != null && field.FieldType == typeof(LanguageKeySO))
            {
                field.SetValue(TheMainObject, _newValue);
                EditorUtility.SetDirty(TheMainObject);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        void UpdateTipe()
        {
            mTipe = mAllPartitions[mSelectedPartitionIndex];
        }
    }
}

#endif