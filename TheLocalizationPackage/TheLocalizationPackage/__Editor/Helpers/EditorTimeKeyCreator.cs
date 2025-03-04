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
using System.Linq;
using LazyPlatypus.TheFolderSupportPackage;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace LazyPlatypus.TheLocalizationPackage.Editor
{
    [Serializable]
    public class EditorTimeKeyCreator
    {
        LanguageCreatorHandler mLanguagesToLoad;
        LanguageKeyHandler mKeyHandler;
        LanguagePartitionHandler mPartitionHandler;
        string mPath;
        GameObject mLocalizationRoot;
        int mHierarchyChange;
        string selectedLanguageEnglishName;
        Vector2 scrollPosition;
        TMP_Text mOutOfComp;

        LocalizationHelper mTheHelper;
        List<TMP_Text> mAllFoundTexts = new List<TMP_Text>();
        List<KeyCreatorUnit> mTexts = new List<KeyCreatorUnit>();
        List<KeyCreatorUnit> mOutOfComponentTexts = new List<KeyCreatorUnit>();
        
        public void OnEnableModule()
        {
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
            Undo.postprocessModifications += OnUndoPostprocess;
        }

        public void AddMainResources(string _path, LanguageKeyHandler _keyHandler, LanguageCreatorHandler _languagesToLoad, LanguagePartitionHandler _partitions)
        {
            mPath = _path;
            mKeyHandler = _keyHandler;
            mLanguagesToLoad = _languagesToLoad;
            mPartitionHandler = _partitions;
            selectedLanguageEnglishName = "English";
        }

        public void OnDisableModule()
        {
            EditorApplication.hierarchyChanged -= OnHierarchyChanged;
            Undo.postprocessModifications -= OnUndoPostprocess;
        }
        
        public void MyOnGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUI.BeginChangeCheck();
            mLocalizationRoot = (GameObject)EditorGUILayout.ObjectField("Localization Root Object", mLocalizationRoot, typeof(GameObject), true);
            if (EditorGUI.EndChangeCheck())
                LocalizationRootChanged();

            if (mLocalizationRoot != null)
            {
                EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
                EditorGUILayout.Space();
                if (GUILayout.Button("Refresh"))
                    GatherAllTextObjects();

                ShowLocalizationOnGUI();
            }
        }

        void ShowLocalizationOnGUI()
        {
            if (mTheHelper == null)
            {
                EditorGUILayout.HelpBox("Localization Helper is needed to translate the texts runtime. Please attach one manually, or just click the button below!", MessageType.Info);
                if (GUILayout.Button("Attach Localization Component", GUILayout.Width(200)))
                    CreateNewHelperComponentOnRoot();
            }
            else if (mTheHelper.Tipe == null)
            {
                if (GUILayout.Button("Change Partition"))
                {
                    mTheHelper.Tipe = null;
                    return;
                }
                ShowPartitionLocalization();
            }
            else
            {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("You can display any languages on the given UI here", MessageType.Info);
                
                EditorGUILayout.LabelField("Select Language:");
                if (mLanguagesToLoad != null && mLanguagesToLoad.CreatedLanguages != null && mLanguagesToLoad.CreatedLanguages.Count > 0)
                {
                    var languageNames = mLanguagesToLoad.CreatedLanguages.ConvertAll(lang => lang.EnglishName);
                    int selectedIndex = mLanguagesToLoad.CreatedLanguages.FindIndex(lang => lang.EnglishName == selectedLanguageEnglishName);
                    EditorGUI.BeginChangeCheck();
                    selectedIndex = EditorGUILayout.Popup(selectedIndex, languageNames.ToArray());
                    selectedLanguageEnglishName = mLanguagesToLoad.CreatedLanguages[selectedIndex].EnglishName;
                    if (EditorGUI.EndChangeCheck())
                        DisplayTexts();
                }
                else
                {
                    EditorGUILayout.LabelField("No languages available.");
                    return;
                }
                
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
                EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
                
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Current Name", EditorStyles.boldLabel);
                GUILayout.Label("Current Input Text", EditorStyles.boldLabel);
                GUILayout.Label("Actions", EditorStyles.boldLabel);
                EditorGUILayout.EndHorizontal();
                
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(300));
                for (int i = 0; i < mTexts.Count; i++)
                    mTexts[i].OnMyGUI();
                EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
                GUILayout.Label("Not under component:", EditorStyles.boldLabel);
                for (int i = 0; i < mOutOfComponentTexts.Count; i++)
                    mOutOfComponentTexts[i].OnMyGUI();
                EditorGUILayout.EndScrollView();
                
                EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Add out of Component Text Asset: ");
                mOutOfComp = (TMP_Text)EditorGUILayout.ObjectField("Text Component:", mOutOfComp, typeof(TMP_Text), true);
                if (GUILayout.Button("Add", GUILayout.Width(100)))
                {
                    if (mOutOfComp == null)
                        return;
                    
                    var newKeyUnit = new KeyCreatorUnit(mOutOfComp);
                    mOutOfComponentTexts.Add(newKeyUnit);
                    newKeyUnit.Setup(mTheHelper, mKeyHandler, mLanguagesToLoad, mPath, mPartitionHandler);
                    
                    mOutOfComp = null;
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        public void DisplayTexts()
        {
            LanguageSO selectedLanguage = null;
            for (int i = 0; i < mLanguagesToLoad.CreatedLanguages.Count; i++)
            {
                if (mLanguagesToLoad.CreatedLanguages[i].EnglishName == selectedLanguageEnglishName)
                {
                    selectedLanguage = mLanguagesToLoad.CreatedLanguages[i];
                    break;
                }
            }
            
            if (selectedLanguage == null)
                return;
            
            var languageFolderPath = Path.Combine(mPath, selectedLanguage.ShortHand.ToLower());
            var assets = FolderStructures.FindAssets<LanguageKeyValueSO>(languageFolderPath, "t:LanguageKeyValueSO");
            for (int i = 0; i < mTheHelper.Keys.Count; i++)
            {
                for (int j = 0; j < assets.Count; j++)
                {
                    if (assets[j].TheKey == mTheHelper.Keys[i])
                    {
                        mTheHelper.Texts[i].text = assets[j].TheValue;
                        break;
                    }
                }
            }
        }

        void ShowPartitionLocalization()
        {
            var allPartitions = mKeyHandler.mPartitionHandler.RetrievePartitions();

            if (allPartitions.Count == 0)
            {
                EditorGUILayout.HelpBox("You don't have any localization Partitions, create one ", MessageType.Error);
            }
            else
            {
                EditorGUILayout.HelpBox("You have to set a partition for the localization!", MessageType.Info);
                for (int i = 0; i < allPartitions.Count; i++)
                {
                    var index = i;
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label(allPartitions[i].KEY);
                    if (GUILayout.Button("Select", GUILayout.Width(100)))
                        SelectThisPartition(index);
                    EditorGUILayout.EndHorizontal();
                }
            }
        }

        void SelectThisPartition(int index)
        {
            var partitions = mKeyHandler.mPartitionHandler.RetrievePartitions();
            mTheHelper.Tipe = partitions[index];
            EditorUtility.SetDirty(mTheHelper);
            
            for (int i = 0; i < mTexts.Count; i++)
                mTexts[i].Setup(partitions[index]);
        }

        void CreateNewHelperComponentOnRoot()
        {
            mTheHelper = mLocalizationRoot.AddComponent<LocalizationHelper>();
            mTheHelper.Keys = new List<LanguageKeySO>();
            mTheHelper.Texts = new List<TMP_Text>();
        }

        void CheckForHelper()
        {
            mTheHelper = mLocalizationRoot.GetComponent<LocalizationHelper>();
        }
        
        void LocalizationRootChanged()
        {
            if (mLocalizationRoot == null)
                return;
            GatherAllTextObjects();
            mTheHelper = mLocalizationRoot.GetComponent<LocalizationHelper>();
        }

        void OnHierarchyChanged()
        {
            if (mLocalizationRoot == null)
                return;

            var currentChangeID = mLocalizationRoot.transform.hierarchyCapacity; // Track hierarchy change
            if (currentChangeID != mHierarchyChange)
            {
                mHierarchyChange = currentChangeID;
                GatherAllTextObjects();
            }
        }

        UndoPropertyModification[] OnUndoPostprocess(UndoPropertyModification[] modifications)
        {
            if (mLocalizationRoot == null)
                return modifications;

            foreach (var mod in modifications)
            {
                if (mod.currentValue.target is Transform || mod.currentValue.target is TMP_Text)
                {
                    GatherAllTextObjects();
                    break;
                }
            }
            return modifications;
        }

        void GatherAllTextObjects()
        {
            mTexts = new List<KeyCreatorUnit>();
            mOutOfComponentTexts = new List<KeyCreatorUnit>();
            
            if (mLocalizationRoot == null)
                return;
            
            CheckForHelper();
            
            var allTextObjects = new List<TMP_Text>();
            var texts = mLocalizationRoot.GetComponentsInChildren<TMP_Text>(true);
            allTextObjects.AddRange(texts);
            mAllFoundTexts = texts.ToList();

            mTexts = new List<KeyCreatorUnit>();
            mOutOfComponentTexts = new List<KeyCreatorUnit>();
            for (int i = 0; i < mAllFoundTexts.Count; i++)
                mTexts.Add(new KeyCreatorUnit(mAllFoundTexts[i]));

            if (mTheHelper != null)
            {
                for (int i = 0; i < mTexts.Count; i++)
                    mTexts[i].Setup(mTheHelper, mKeyHandler, mLanguagesToLoad, mPath, mPartitionHandler);
                
                if (mTheHelper.Texts == null)
                    return;
                
                for (int i = 0; i < mTheHelper.Texts.Count; i++)
                {
                    var hasIt = false;
                    for (int j = 0; j < mTexts.Count; j++)
                    {
                        if (mTexts[j].mComponent == mTheHelper.Texts[i])
                        {
                            hasIt = true;
                            continue;
                        }
                    }
                    if (!hasIt)
                        mOutOfComponentTexts.Add(new KeyCreatorUnit(mTheHelper.Texts[i]));
                }
                for (int i = 0; i < mOutOfComponentTexts.Count; i++)
                    mOutOfComponentTexts[i].Setup(mTheHelper, mKeyHandler, mLanguagesToLoad, mPath, mPartitionHandler);
            }
        }
    }
}
#endif