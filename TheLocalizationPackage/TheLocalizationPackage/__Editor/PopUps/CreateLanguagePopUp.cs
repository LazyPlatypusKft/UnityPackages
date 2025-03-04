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
    public class CreateLanguagePopUp: EditorWindow
    {
        int mPreferredIndex;
        string mEnglishName = string.Empty;
        string mNativeName = string.Empty;
        string mShortHand = string.Empty;
        string mPath;
        Action<LanguageSO> mCallback;
        List<LanguageSO> mAlreadyCreatedLanguages;

        public static void Show(string _path, List<LanguageSO> languages, Action<LanguageSO> _onCreation)
        {
            var window = CreateInstance<CreateLanguagePopUp>();
            window.mAlreadyCreatedLanguages = languages;
            window.mPath = _path;
            window.mCallback = _onCreation;
            var preferredIndex = 0;
            for (int i = 0; i < languages.Count; i++)
                if (languages[i].PreferredIndex > preferredIndex)
                    preferredIndex = languages[i].PreferredIndex + 1;
            window.mPreferredIndex = preferredIndex;
            window.titleContent = new GUIContent("Create Language");
            window.minSize = new Vector2(350, 250);
            window.maxSize = new Vector2(350, 250);
            window.ShowUtility();
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField("Create New Language", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("If the English name is somehow consists of multiple words, put underscores instead of the spaces.", MessageType.Info);
            mPreferredIndex = EditorGUILayout.IntField("Preferred Index:", mPreferredIndex);
            mEnglishName = EditorGUILayout.TextField("English Name:", mEnglishName);
            mNativeName = EditorGUILayout.TextField("Native Name:", mNativeName);
            
            EditorGUILayout.HelpBox("Use lowercase only, preferably only 3 characters. Will only be used during development!", MessageType.Info);
            mShortHand = EditorGUILayout.TextField("Shorthand Name:", mShortHand);

            EditorGUILayout.Space();

            if (GUILayout.Button("Create", GUILayout.Width(100)))
            {
                if (string.IsNullOrEmpty(mEnglishName) || string.IsNullOrEmpty(mNativeName) || string.IsNullOrEmpty(mShortHand))
                {
                    EditorUtility.DisplayDialog("Error", "English Name and Native Name cannot be empty.", "OK");
                    return;
                }

                for (int i = 0; i < mAlreadyCreatedLanguages.Count; i++)
                {
                    if (mAlreadyCreatedLanguages[i].ShortHand == mShortHand)
                    {
                        EditorUtility.DisplayDialog("Error", "Shorthands have to be unique.", "OK");
                        return;
                    }
                }

                var shorthandWithCapital = mShortHand.ToLower();
                shorthandWithCapital = char.ToUpper(shorthandWithCapital[0]) + shorthandWithCapital.Substring(1);
                var assetPath = Path.Combine(mPath, mShortHand);
                
                var newLanguage = FolderStructures.CreateAsset<LanguageSO>(assetPath, $"{shorthandWithCapital}.asset", FillWithValues);
                mCallback?.Invoke(newLanguage);
                Close();
            }
        }

        void FillWithValues(LanguageSO _newLanguageAsset)
        {
            _newLanguageAsset.PreferredIndex = mPreferredIndex;
            _newLanguageAsset.EnglishName = mEnglishName;
            _newLanguageAsset.NativeName = mNativeName;
            _newLanguageAsset.ShortHand = mShortHand.ToLower();
            
            mAlreadyCreatedLanguages.Add(_newLanguageAsset);
        }
    }
}
#endif