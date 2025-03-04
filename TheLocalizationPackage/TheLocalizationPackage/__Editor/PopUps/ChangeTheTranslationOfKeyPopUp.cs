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

namespace LazyPlatypus.TheLocalizationPackage.Editor
{
    public class ChangeTheTranslationOfKeyPopUp : EditorWindow
    {
        LanguageKeySO MyKey;
        LanguageCreatorHandler mLanguages;
        List<LanguageKeyValueSO> mLanguageKeys;
        List<ChangeTranslationUnit> mTranslationUnits;
        string mPath;
        
        public static void Show(LanguageKeySO _key, LanguageCreatorHandler _languages, string _path)
        {
            var window = CreateInstance<ChangeTheTranslationOfKeyPopUp>();

            window.mLanguages = _languages;
            window.MyKey = _key;
            window.mPath = _path;
            window.mTranslationUnits = new List<ChangeTranslationUnit>();

            for (int i = 0; i < window.mLanguages.CreatedLanguages.Count; i++)
            {
                var language = window.mLanguages.CreatedLanguages[i];
                var languagePath = Path.Combine(window.mPath, language.ShortHand.ToLower());
                var keys = FolderStructures.FindAssets<LanguageKeyValueSO>(languagePath, "t:LanguageKeyValueSO");
                LanguageKeyValueSO foundTranslationkey = null;
                for (int j = 0; j < keys.Count; j++)
                {
                    if (keys[j].TheKey == window.MyKey)
                    {
                        foundTranslationkey = keys[j];
                        break;
                    }
                }
                window.mTranslationUnits.Add(new ChangeTranslationUnit(language.EnglishName, foundTranslationkey));
            }
            
            window.titleContent = new GUIContent("Add Translations To Key");
            window.minSize = new Vector2(350, 250);
            window.ShowUtility();
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField("Current key is " + MyKey, EditorStyles.boldLabel);
            EditorGUILayout.Space();
            for (int i = 0; i < mTranslationUnits.Count; i++)
            {
                mTranslationUnits[i].MyOnGUI();
            }
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            
            if (GUILayout.Button("Close", GUILayout.Width(100)))
                Close();
        }
    }
}

#endif