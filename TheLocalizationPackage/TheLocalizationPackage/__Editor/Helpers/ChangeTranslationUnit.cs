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

using UnityEditor;
using UnityEngine;

namespace LazyPlatypus.TheLocalizationPackage.Editor
{
    public class ChangeTranslationUnit
    {
        LanguageKeyValueSO mKey;
        string mLanguageKey;
        string mTranslation;
        
        public ChangeTranslationUnit(string _languageKey, LanguageKeyValueSO _key)
        {
            mKey = _key;
            mLanguageKey = _languageKey;
            mTranslation = _key.TheValue;
        }

        public void MyOnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(mLanguageKey);
            mTranslation = EditorGUILayout.TextField(mTranslation);
            if (GUILayout.Button("Apply", GUILayout.Width(100)))
            {
                mKey.TheValue = mTranslation;
                EditorUtility.SetDirty(mKey);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}

#endif