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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace LazyPlatypus.TheLocalizationPackage.Editor
{
    public class LanguageImporterPopUp : EditorWindow
    {
        List<LanguageSO> mLanguages;
        string mSelectedLanguage;
        string mImportFilePath = "Assets/Imports/language.json";
        string mLanguagesPath;

        public static void Show(string path, List<LanguageSO> languages)
        {
            var window = GetWindow<LanguageImporterPopUp>("Import JSON");
            window.mLanguages = languages;
            window.mLanguagesPath = path;
            window.mSelectedLanguage = languages.Count > 0 ? languages[0].EnglishName : string.Empty;
            window.minSize = new Vector2(400, 200);
        }

        private void OnGUI()
        {
            GUILayout.Label("Import JSON", EditorStyles.boldLabel);

            // JSON file path input
            EditorGUILayout.LabelField("JSON File Path:");
            mImportFilePath = EditorGUILayout.TextField(mImportFilePath);

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

            EditorGUILayout.Space();

            // Buttons
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Import", GUILayout.Height(30)))
            {
                ImportJson();
            }
            if (GUILayout.Button("Cancel", GUILayout.Height(30)))
            {
                Close();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void ImportJson()
        {
            if (!File.Exists(mImportFilePath))
            {
                Debug.LogError($"JSON file not found at: {mImportFilePath}");
                return;
            }

            var selectedLanguageShortForm = string.Empty;
            for (int i = 0; i < mLanguages.Count; i++)
            {
                if (mLanguages[i].EnglishName == mSelectedLanguage)
                {
                    selectedLanguageShortForm = mLanguages[i].ShortHand;
                    break;
                }
            }

            if (string.IsNullOrEmpty(selectedLanguageShortForm))
            {
                Debug.LogError("Invalid language selection.");
                return;
            }

            string languageFolderPath = Path.Combine(mLanguagesPath, selectedLanguageShortForm.ToLower());
            if (!AssetDatabase.IsValidFolder(languageFolderPath))
            {
                Debug.LogError($"Language folder does not exist: {languageFolderPath}");
                return;
            }

            // Read and parse JSON
            string jsonContent = File.ReadAllText(mImportFilePath);
            var jsonObject = DeserializeJsonToDictionary(jsonContent);

            // Overwrite LanguageKeyValueSOs
            OverwriteLanguageKeyValueSOs(jsonObject, languageFolderPath);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("Import completed successfully.");
            Close();
        }
        
        private Dictionary<string, object> DeserializeJsonToDictionary(string jsonContent)
        {
            var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonContent);
            return ConvertJObjectToDictionary(jsonObject);
        }

        private Dictionary<string, object> ConvertJObjectToDictionary(Dictionary<string, object> input)
        {
            var result = new Dictionary<string, object>();

            foreach (var kvp in input)
            {
                if (kvp.Value is JObject nestedObject)
                {
                    // Recursively convert JObject to Dictionary
                    result[kvp.Key] = ConvertJObjectToDictionary(nestedObject.ToObject<Dictionary<string, object>>());
                }
                else if (kvp.Value is JArray array)
                {
                    // Handle JArray if needed (optional)
                    result[kvp.Key] = array.ToObject<List<object>>();
                }
                else
                {
                    result[kvp.Key] = kvp.Value;
                }
            }

            return result;
        }

        private void OverwriteLanguageKeyValueSOs(Dictionary<string, object> jsonObject, string folderPath)
        {
            foreach (var kvp in jsonObject)
            {
                string key = kvp.Key;
                object value = kvp.Value;

                if (value is Dictionary<string, object> nestedObject)
                {
                    // Handle nested folders
                    string subfolderPath = Path.Combine(folderPath, key);
                    if (AssetDatabase.IsValidFolder(subfolderPath))
                    {
                        OverwriteLanguageKeyValueSOs(nestedObject, subfolderPath);
                    }
                    else
                    {
                        Debug.LogWarning($"Subfolder not found: {subfolderPath}");
                    }
                }
                else if (value is string stringValue)
                {
                    // Find and overwrite LanguageKeyValueSO
                    string[] guids = AssetDatabase.FindAssets("t:LanguageKeyValueSO", new[] { folderPath });
                    foreach (string guid in guids)
                    {
                        string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                        LanguageKeyValueSO keyValue = AssetDatabase.LoadAssetAtPath<LanguageKeyValueSO>(assetPath);

                        if (keyValue != null && keyValue.TheKey != null && keyValue.TheKey.KEY == key)
                        {
                            keyValue.TheValue = stringValue;
                            EditorUtility.SetDirty(keyValue);
                            Debug.Log($"Updated {keyValue.TheKey.KEY} with value: {stringValue}");
                            break;
                        }
                    }
                }
            }
        }
    }
}
#endif