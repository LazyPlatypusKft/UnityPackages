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

using TMPro;
using UnityEditor;
using UnityEngine;

namespace LazyPlatypus.TheLocalizationPackage.Editor
{
    public class KeyCreatorUnit
    {
        public bool UsingAKey;
        public TMP_Text mComponent;
        public LanguagePartitionSO Tipe;
        public LanguageKeySO MyKey;
        
        LanguagePartitionHandler mPartitionHandler;
        LanguageKeyHandler mKeyHandler;
        LanguageCreatorHandler mLanguages;
        LocalizationHelper mTheHelper;
        string mPath;
        string mText;
        int mIndexOfMine = 0;
        
        public KeyCreatorUnit(TMP_Text _key)
        {
            mIndexOfMine = -1;
            mComponent = _key;
            mText = _key.text;
        }

        public void OnMyGUI()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(mComponent.gameObject.name, EditorStyles.boldLabel);
            GUILayout.Label(mText);
            if (UsingAKey)
            {
                if (GUILayout.Button("Change Key", GUILayout.Width(100)))
                    AssociateTMPTextWithLanguagesPopUp.Show(Tipe, mKeyHandler, mLanguages, mPath, null, AddKeyToUnit);
                if (GUILayout.Button("Change Translation", GUILayout.Width(150)))
                    ChangeTheTranslationOfKeyPopUp.Show(MyKey, mLanguages, mPath);
            }
            else
            {
                if (GUILayout.Button("Add Key", GUILayout.Width(200)))
                    AssociateTMPTextWithLanguagesPopUp.Show(Tipe, mKeyHandler, mLanguages, mPath, null, AddKeyToUnit);
            }
            EditorGUILayout.EndHorizontal();
        }

        void AddKeyToUnit(LanguageKeySO _key)
        {
            MyKey = _key;
            mTheHelper.AddTextWithKey(mComponent, _key);
            EditorUtility.SetDirty(mTheHelper);
        }

        public void Setup(LanguagePartitionSO _partition)
        {
            Tipe = _partition;
        }
        
        public void Setup(LocalizationHelper _theHelper, LanguageKeyHandler _keyHandler, LanguageCreatorHandler _languages, string _path, LanguagePartitionHandler _partitions)
        {
            mPartitionHandler = _partitions;
            mKeyHandler = _keyHandler;
            mLanguages = _languages;
            mTheHelper = _theHelper;
            mPath = _path;
            Tipe = _theHelper.Tipe;
            
            if (_theHelper.Texts != null && _theHelper.Texts.Contains(mComponent))
            {
                mIndexOfMine = _theHelper.Texts.IndexOf(mComponent);
                MyKey = _theHelper.Keys[mIndexOfMine];
                UsingAKey = true;
            }
        }
    }
}
#endif