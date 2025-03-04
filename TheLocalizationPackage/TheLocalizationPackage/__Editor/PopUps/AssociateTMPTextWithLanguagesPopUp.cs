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
    public class AssociateTMPTextWithLanguagesPopUp : EditorWindow
    {
        KeyCreatorUnit mUnit; 
        LanguageKeyHandler mKeyHandler;
        LanguagePartitionSO mTipe;
        LanguageCreatorHandler mLanguages;
        Vector2 scrollPosition;
        Vector2 scrollPosition2;
        List<LanguageKeySO> mFilteredKeys;
        int mLanguageIndex = 0;
        string mPath;
        string mTranslationEnglish;
        string mOrganizationFolder;
        string mNewKey;
        string mPrecursor;
        bool mWithoutPrev;
        Dictionary<string, LanguageKeyValueSO> mTranslatedLanguages;
        Action mCancelCallback;
        Action<LanguageKeySO> mSelectedKeyCallback;

        public static void Show(LanguagePartitionSO _tipe, LanguageKeyHandler _handler, LanguageCreatorHandler _languages, string _path, Action _cancel, Action<LanguageKeySO> _selectedKey, bool _withoutPrev = false)
        {
            var window = CreateInstance<AssociateTMPTextWithLanguagesPopUp>();
            window.mLanguageIndex = 0;
            window.mKeyHandler = _handler;
            window.mTipe = _tipe;
            window.mPath = _path;
            window.mLanguages = _languages;
            window.mCancelCallback = _cancel;
            window.mSelectedKeyCallback = _selectedKey;
            window.mWithoutPrev = _withoutPrev;
            window.mTranslatedLanguages = new Dictionary<string, LanguageKeyValueSO>();
            window.mPrecursor = (_tipe.KEY.Length > 4 ? _tipe.KEY.ToLower().Substring(0, 4) : _tipe.KEY.ToLower()).ToUpper();
            
            for (int i = 0; i < _handler.KeyCollections.Count; i++)
            {
                if (_handler.KeyCollections[i].Partition == _tipe)
                {
                    window.mFilteredKeys = _handler.KeyCollections[i].AllKeys;
                    break;
                }
            }

            if (_languages.CreatedLanguages.Count > 0)
            {
                for (int i = 0; i < _languages.CreatedLanguages.Count; i++)
                {
                    if (_languages.CreatedLanguages[i].ShortHand == "en")
                    {
                        window.mLanguageIndex = i;
                        window.RetrieveLanguageKeys(_languages.CreatedLanguages[window.mLanguageIndex]);
                        break;
                    }
                }
            }
            window.titleContent = new GUIContent("Match or Create Language Key");
            window.minSize = new Vector2(350, 250);
            window.ShowUtility();
        }

        void RetrieveLanguageKeys(LanguageSO languagesCreatedLanguage)
        {
            var selectedLanguageShortForm = languagesCreatedLanguage.ShortHand;
            if (string.IsNullOrEmpty(selectedLanguageShortForm))
            {
                Debug.LogError("Invalid language selection.");
                return;
            }
            string languageFolderPath = Path.Combine(mPath, selectedLanguageShortForm.ToLower());
            if (!AssetDatabase.IsValidFolder(languageFolderPath))
            {
                Debug.LogError($"Language folder does not exist: {languageFolderPath}");
                return;
            }
            mTranslatedLanguages = new Dictionary<string, LanguageKeyValueSO>();
            var allKeys = FolderStructures.FindAssets<LanguageKeyValueSO>(languageFolderPath, "t:LanguageKeyValueSO", true);
            for (int i = 0; i < allKeys.Count; i++)
                mTranslatedLanguages[allKeys[i].TheKey.KEY] = allKeys[i];
        }

        void OnGUI()
        {
            if (!mWithoutPrev)
            {
                EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
                EditorGUILayout.LabelField("Selected Language is " + mLanguages.CreatedLanguages[mLanguageIndex].EnglishName, EditorStyles.boldLabel);
                EditorGUILayout.Space();
                // TODO: Select language, Load language and then show the keys with the translations
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));
                for (int i = 0; i < mLanguages.CreatedLanguages.Count; i++)
                {
                    var index = i;
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label(mLanguages.CreatedLanguages[i].EnglishName);
                    if (GUILayout.Button("Select", GUILayout.Width(100)))
                        SelectThisKey(index);
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();
                EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Select already create key:", EditorStyles.boldLabel);
                EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
                scrollPosition2 = EditorGUILayout.BeginScrollView(scrollPosition2, GUILayout.Height(200));
                for (int i = 0; i < mFilteredKeys.Count; i++)
                {
                    var index = i;
                    EditorGUILayout.BeginHorizontal();
                    try
                    {
                        GUILayout.Label(mFilteredKeys[i].KEY);
                        GUILayout.Label(mTranslatedLanguages[mFilteredKeys[i].KEY].TheValue);
                        if (GUILayout.Button("Select", GUILayout.Width(100)))
                            SelectThisKey(index);
                    }
                    catch (Exception error)
                    {
                        Debug.LogError(error);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();
                EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
                EditorGUILayout.Space();
            }
            EditorGUILayout.LabelField("Create new key:", EditorStyles.boldLabel);
          
            EditorGUILayout.HelpBox("Please name your key based on its folder as well! So if you were to place it in Menu/ToolBar, then name it MENU_TOOLBAR_...", MessageType.Warning);
            EditorGUILayout.HelpBox("Right naming: MENU_TOOLBAR_MAIN_LABEL_GO_AWAY, an asset in Menu/ToolBar", MessageType.Info);
            EditorGUI.BeginChangeCheck();
            mOrganizationFolder = EditorGUILayout.TextField("Folder:", mOrganizationFolder);
            if (EditorGUI.EndChangeCheck())
                mPrecursor = ((mTipe.KEY.Length > 4 ? mTipe.KEY.ToLower().Substring(0, 4) : mTipe.KEY.ToLower()) + "_" + ToLowerOrgCase(mOrganizationFolder)).ToUpper();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(mPrecursor, EditorStyles.boldLabel);
            mNewKey = EditorGUILayout.TextField("Key:", mNewKey);
            EditorGUILayout.EndHorizontal();
            mTranslationEnglish = EditorGUILayout.TextField("Basic English Translation:", mTranslationEnglish);
            EditorGUILayout.HelpBox("Right naming: Menu/ToolBar", MessageType.Info);
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Create", GUILayout.Width(100)))
            {
                var indexOfKey = -1;
                var key = CreateNewKeyForAllLanguages();
                for (int i = 0; i < mKeyHandler.KeyCollections.Count; i++)
                    if (mKeyHandler.KeyCollections[i].Partition == mTipe)
                        mKeyHandler.KeyCollections[i].AllKeys.Add(key);
                SelectThisKey(key);
            }
            if (GUILayout.Button("Cancel", GUILayout.Width(100)))
            {
                mCancelCallback?.Invoke();
                Close();
            }
            EditorGUILayout.EndHorizontal();
        }

        string ToLowerOrgCase(string _orgFolder)
        {
            if (_orgFolder.Length == 0)
                return "";

            var splitted = _orgFolder.Split('/');
            var merged = "";
            for (int i = 0; i < splitted.Length; i++)
                merged += splitted[i] + "_";
            return merged;
        }

        LanguageKeySO CreateNewKeyForAllLanguages()
        {
            var toLower = mNewKey.ToLower();
            var keyPath = Path.Combine(mPath, "_keys", mTipe.KEY.ToLower());
            if (!string.IsNullOrEmpty(mOrganizationFolder))
                keyPath = Path.Combine(keyPath, mOrganizationFolder);
            var shortenedPartition = mTipe.KEY.Length <= 4 ? mTipe.KEY.ToLower() : mTipe.KEY.ToLower().Substring(0, 4);
            FolderStructures.EnsureFolderExists(keyPath);
            var theKey = FolderStructures.CreateAsset<LanguageKeySO>(keyPath,$"{mPrecursor.ToLower()}{toLower}.asset", FillInKeyData);
            for (int i = 0; i < mLanguages.CreatedLanguages.Count; i++)
            {
                var selectedLanguageShortForm = mLanguages.CreatedLanguages[i].ShortHand;
                var languageFolderPath = Path.Combine(mPath, selectedLanguageShortForm.ToLower(), mTipe.KEY.ToLower());
                if (!string.IsNullOrEmpty(mOrganizationFolder))
                    languageFolderPath = Path.Combine(languageFolderPath, mOrganizationFolder);
                FolderStructures.EnsureFolderExists(languageFolderPath);
                var key = FolderStructures.CreateAsset<LanguageKeyValueSO>(languageFolderPath, $"{selectedLanguageShortForm.ToLower()}_{shortenedPartition}_{mPrecursor.ToLower()}{toLower}.asset", (data) => FillLanguageKeyData(data, theKey));
                
                var partitionPath = Path.Combine(mPath, selectedLanguageShortForm.ToLower(), "_partitions");
                var allKeys = FolderStructures.FindAssets<LoadableLanguageDictionarySO>(partitionPath, "t:LoadableLanguageDictionarySO", false);
                for (int j = 0; j < allKeys.Count; j++)
                {
                    if (allKeys[j].TheType == mTipe)
                    {
                        allKeys[j].TheDictionary.Add(key);
                        EditorUtility.SetDirty(allKeys[j]);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                        break;
                    }
                }
            }
            return theKey;
        }

        void FillInKeyData(LanguageKeySO _key)
        {
            _key.KEY = mPrecursor.ToUpper() + mNewKey.ToUpper();
        }

        void FillLanguageKeyData(LanguageKeyValueSO _language, LanguageKeySO _key)
        {
            _language.TheValue = mTranslationEnglish;
            _language.TheKey = _key;
        }

        void SelectThisKey(int _index)
        {
            for (int i = 0; i < mKeyHandler.KeyCollections.Count; i++)
            {
                if (mKeyHandler.KeyCollections[i].Partition == mTipe)
                {
                    SelectThisKey(mKeyHandler.KeyCollections[i].AllKeys[_index]);
                    break;
                }
            }
        }

        void SelectThisKey(LanguageKeySO _index)
        {
            mSelectedKeyCallback?.Invoke(_index);
            Close();
        }
    }
}

#endif