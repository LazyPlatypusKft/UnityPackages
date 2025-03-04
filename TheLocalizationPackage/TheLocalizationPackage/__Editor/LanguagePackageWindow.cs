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

using System.IO;
using System.Collections.Generic;
using LazyPlatypus.TheFolderSupportPackage;
using UnityEditor;
using UnityEngine;

namespace LazyPlatypus.TheLocalizationPackage.Editor
{
    public class LanguagePackageWindow : EditorWindow
    {
        static FolderConfigurationSO FolderConfig;
        static LanguagePartitionHandler PartitionHandler;
        static LanguageCreatorHandler Languages;
        static LanguageKeyHandler KeyHandler;
        static LanguageFolderHandler LanguageHandler;
        static EditorTimeKeyCreator KeyCreatorAndAssotiator;
        static KeyCreatorHandler KeyCreatorWindow;
        static KeyEditorHandler KeyEditorWindow;
        
        const string LINKED_ASSET_ID = "LP_LanguagePackageWindow";
        
        [MenuItem("LazyPlatypus/Localization Helper")]
        public static void ShowWindow()
        {
            // Show the window
            GetWindow<LanguagePackageWindow>("Localization Helper");
            RetrieveData();
        }

        void OnEnable()
        {
            KeyCreatorAndAssotiator = new EditorTimeKeyCreator();
            KeyCreatorAndAssotiator.OnEnableModule();
        }

        void OnDisable()
        {
            KeyCreatorAndAssotiator.OnDisableModule();
        }

        void OnGUI()
        {
            if (FolderConfig == null)
                RetrieveData();
            
            FolderConfig.MyOnGUI();
            PartitionHandler.MyOnGUI();
            Languages.MyOnGUI();
            KeyCreatorAndAssotiator.MyOnGUI();
            KeyCreatorWindow.MyOnGUI();
            KeyEditorWindow.MyOnGUI();
            
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (GUILayout.Button("Clean up assets", GUILayout.Width(200)))
                CleanUp();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Open Asset Migration Pop Up", GUILayout.Width(300)))
                AssetMigrationPopUp.Show();
            if (GUILayout.Button("Open ScriptableObject Translator", GUILayout.Width(300)))
                ScriptableObjectTranslatorPopUp.Show(PartitionHandler, KeyHandler, Languages, FolderConfig.GetPath());
            GUILayout.EndHorizontal();
        }

        void CleanUp()
        {
            // Remove all unused keys (compares to the English language)
            var theEnglishPath = Path.Combine(FolderConfig.GetPath(), "en");
            var englishKeyPaths = FolderStructures.FindAssets<LanguageKeyValueSO>(theEnglishPath, "t:LanguageKeyValueSO");
            var genericKeysPath = Path.Combine(FolderConfig.GetPath(), "_keys");
            var allKeysExisting = FolderStructures.FindAssets<LanguageKeySO>(genericKeysPath, "t:LanguageKeySO");
            var usedLanguageKeyDict = new List<LanguageKeySO>();
            foreach (var keyValue in englishKeyPaths)
                usedLanguageKeyDict.Add(keyValue.TheKey);
            for (int i = allKeysExisting.Count - 1; i >= 0; i--)
                if (!usedLanguageKeyDict.Contains(allKeysExisting[i]))
                    FolderStructures.DeletingGenericAsset(allKeysExisting[i], null);

            for (int i = 0; i < Languages.CreatedLanguages.Count; i++)
            {
                var selectedLanguageShortForm = Languages.CreatedLanguages[i].ShortHand;
                
                var languagesPath = Path.Combine(FolderConfig.GetPath(), selectedLanguageShortForm.ToLower());
                var allKeys = FolderStructures.FindAssets<LanguageKeyValueSO>(languagesPath, "t:LanguageKeyValueSO");
                // Delete all LanguageKeyValue which does not have a key
                for (int j = allKeys.Count - 1; j >= 0; j--)
                    if (allKeys[j].TheKey == null)
                        FolderStructures.DeletingGenericAsset(allKeys[j], null);
                
                var partitionPath = Path.Combine(languagesPath, "_partitions");
                var allDictionaries = FolderStructures.FindAssets<LoadableLanguageDictionarySO>(partitionPath, "t:LoadableLanguageDictionarySO");
                // Remove all empty references (either by the deleted keys above, or in general, user deleted content)
                for (int j = 0; j < allDictionaries.Count; j++)
                {
                    for (int k = allDictionaries[j].TheDictionary.Count - 1; k >= 0; k--)
                        if (allDictionaries[j].TheDictionary[k] == null)
                            allDictionaries[j].TheDictionary.RemoveAt(k);
                    EditorUtility.SetDirty(allDictionaries[j]);
                    AssetDatabase.Refresh();
                    AssetDatabase.SaveAssets();
                }
            }
        }

        static void RetrieveData()
        {
            FolderConfig = FolderStructures.RetrieveConfigurationAsset<FolderConfigurationSO>(LINKED_ASSET_ID);
            LanguageFolderStructures.EnsureMandatoryFolderExists(FolderConfig.GetPath());
            
            PartitionHandler = new LanguagePartitionHandler(FolderConfig.GetPath(), LanguageFolderStructures.LoadLanguagePartitions(FolderConfig.GetPath()));
            KeyHandler = new LanguageKeyHandler(FolderConfig.GetPath(), PartitionHandler);
            
            var languageFolders = LanguageFolderStructures.ParseLanguageFolders(FolderConfig.GetPath());
            Languages = new LanguageCreatorHandler(FolderConfig.GetPath(), languageFolders);
            LanguageHandler = new LanguageFolderHandler(FolderConfig.GetPath(), Languages, PartitionHandler, KeyHandler, LanguageFolderStructures.ParseLanguageFolders(FolderConfig.GetPath()));

            KeyCreatorAndAssotiator.AddMainResources(FolderConfig.GetPath(), KeyHandler, Languages, PartitionHandler);
            KeyCreatorWindow = new KeyCreatorHandler(FolderConfig.GetPath(), KeyHandler, Languages, PartitionHandler);
            KeyEditorWindow = new KeyEditorHandler(FolderConfig.GetPath(), KeyHandler, Languages, PartitionHandler);
        }
    }
}
#endif