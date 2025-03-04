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
using UnityEngine;
using Newtonsoft.Json;

namespace LazyPlatypus.TheLocalizationPackage.Editor
{
    public class LanguageExporterPopUp : EditorWindow
    {
        List<LanguageSO> mLanguages;
        string mSelectedLanguage;
        string mExportFolder = "Assets/Exports";
        string mLanguagesPath;

        public static void Show(string _path, List<LanguageSO> _languages)
        {
            var window = GetWindow<LanguageExporterPopUp>("Export JSON");
            window.mLanguages = _languages;
            window.mLanguagesPath = _path;
            window.mSelectedLanguage = _languages.Count > 0 ? _languages[0].EnglishName : string.Empty;
            window.minSize = new Vector2(400, 200);
        }

        void OnGUI()
        {
            GUILayout.Label("Export JSON", EditorStyles.boldLabel);

            // Language selection dropdown
            EditorGUILayout.LabelField("Select Language:");
            if (mLanguages != null && mLanguages.Count > 0)
            {
                var languageNames = mLanguages.ConvertAll(lang => lang.EnglishName);
                int selectedIndex = mLanguages.FindIndex(lang => lang.EnglishName == mSelectedLanguage);
                selectedIndex = EditorGUILayout.Popup(selectedIndex, languageNames.ToArray());
                mSelectedLanguage = mLanguages[selectedIndex].EnglishName;
            }
            else
            {
                EditorGUILayout.LabelField("No languages available.");
                return;
            }

            // Export folder input
            EditorGUILayout.LabelField("Export Folder (relative to Assets):");
            mExportFolder = EditorGUILayout.TextField(mExportFolder);

            EditorGUILayout.Space();

            // Buttons
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Export", GUILayout.Height(30)))
                ExportJson();
            if (GUILayout.Button("Cancel", GUILayout.Height(30)))
                Close();
            
            EditorGUILayout.EndHorizontal();
        }

        void ExportJson()
        {
            if (string.IsNullOrEmpty(mSelectedLanguage) || string.IsNullOrEmpty(mExportFolder))
            {
                Debug.LogError("Invalid input. Please select a language and specify an export folder.");
                return;
            }

            var selectedLanguageShortForm = "";
            for (int i = 0; i < mLanguages.Count; i++)
            {
                if (mLanguages[i].EnglishName == mSelectedLanguage)
                {
                    selectedLanguageShortForm = mLanguages[i].ShortHand;
                    break;
                }
            }
            
            var languageFolderPath = Path.Combine(mLanguagesPath, selectedLanguageShortForm.ToLower());
            var absoluteExportPath = Path.Combine(Application.dataPath, mExportFolder.Substring("Assets/".Length));
            if (!Directory.Exists(absoluteExportPath))
                Directory.CreateDirectory(absoluteExportPath);

            // Build JSON from the language folder
            var rootObject = BuildJsonFromFolder(languageFolderPath);
            var json = JsonConvert.SerializeObject(rootObject, Formatting.Indented);

            // Save JSON to file
            var exportFilePath = Path.Combine(absoluteExportPath, $"{selectedLanguageShortForm}.json");
            File.WriteAllText(exportFilePath, json);

            Debug.Log($"Exported JSON to {exportFilePath}");
            AssetDatabase.Refresh();
            Close();
        }

        Dictionary<string, object> BuildJsonFromFolder(string folderPath)
        {
            var result = new Dictionary<string, object>();
            var subfolders = AssetDatabase.GetSubFolders(folderPath);
            foreach (string subfolder in subfolders)
            {
                var folderName = Path.GetFileName(subfolder);
                result[folderName] = BuildJsonFromFolder(subfolder);
            }
            var assets = FolderStructures.FindAssets<LanguageKeyValueSO>(folderPath, "t:LanguageKeyValueSO", false);
            foreach (var keyAsset in assets)
                result[keyAsset.TheKey.KEY] = keyAsset.TheValue;
            return result;
        }
    }
}
#endif