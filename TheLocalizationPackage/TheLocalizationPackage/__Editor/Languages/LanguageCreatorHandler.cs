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
using UnityEditor;
using UnityEngine;

namespace LazyPlatypus.TheLocalizationPackage.Editor
{
    [Serializable]
    public class LanguageCreatorHandler
    {
        public event Action<LanguageSO> OnLanguageCreated;
        
        [SerializeField] public List<LanguageSO> CreatedLanguages;

        string mPath;
        bool mOpened;
        
        public LanguageCreatorHandler(string _path, List<string> _folders)
        {
            mPath = _path;
            CreatedLanguages = new List<LanguageSO>();
            for (int i = 0; i < _folders.Count; i++)
            {
                var foundLanguage = LanguageFolderStructures.LoadLanguageAt(_path, _folders[i]);
                if (foundLanguage != null)
                    CreatedLanguages.Add(foundLanguage);
            }
        }
        
        public virtual void MyOnGUI()
        {
            if (!mOpened)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Language Editor:", EditorStyles.boldLabel);
                if (GUILayout.Button("Open", GUILayout.Width(100)))
                    mOpened = true;
                EditorGUILayout.EndHorizontal();
                return;
            }
            
            EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Supported Languages:", EditorStyles.boldLabel);

            if (CreatedLanguages.Count > 0)
            {
                var languageNames = new List<string>();
                foreach (var language in CreatedLanguages)
                    languageNames.Add(language.EnglishName);
                EditorGUILayout.LabelField(string.Join(", ", languageNames), EditorStyles.wordWrappedLabel, GUILayout.ExpandWidth(true));

            }
            else
                EditorGUILayout.LabelField("No languages loaded.");
            EditorGUILayout.Space();
            
            if (GUILayout.Button("Create Language", GUILayout.Width(150)))
                CreateLanguagePopUp.Show(mPath, CreatedLanguages, CreateNewLanguage);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Language Export / Import functionality: ");
            if (GUILayout.Button("Export Language", GUILayout.Width(150)))
                LanguageExporterPopUp.Show(mPath, CreatedLanguages);
            if (GUILayout.Button("Import Language", GUILayout.Width(150)))
                LanguageImporterPopUp.Show(mPath, CreatedLanguages);
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("Close", GUILayout.Width(100)))
                mOpened = false;
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
        }

        void CreateNewLanguage(LanguageSO _newLanguage)
        {
            OnLanguageCreated?.Invoke(_newLanguage);
        }
    }
}
#endif