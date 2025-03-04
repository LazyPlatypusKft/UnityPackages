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
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace LazyPlatypus.TheLocalizationPackage.Editor
{
    public class ScriptableObjectTranslatorPopUp : EditorWindow
    {
        ScriptableObject mObject;
        LanguagePartitionHandler mPartitions;
        LanguageKeyHandler mKeyHandler;
        LanguageCreatorHandler mLanguagesToLoad;
        string mPath;
        List<LanguageKeysToModify> keysToModify;
        
        public static void Show(LanguagePartitionHandler _partitions, LanguageKeyHandler _keyHandler, LanguageCreatorHandler _languagesToLoad, string _path)
        {
            var window = CreateInstance<ScriptableObjectTranslatorPopUp>();

            window.mPath = _path;
            window.mLanguagesToLoad = _languagesToLoad;
            window.mKeyHandler = _keyHandler;
            window.mPartitions = _partitions;
            
            window.titleContent = new GUIContent("Migrating Strings to Localized Version");
            window.minSize = new Vector2(350, 250);
            window.ShowUtility();
        }

        void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
            mObject = (ScriptableObject)EditorGUILayout.ObjectField("Scriptable Object", mObject, typeof(ScriptableObject), true);
            if (EditorGUI.EndChangeCheck())
                DiscoverScriptableObject();

            if (mObject != null)
                foreach (var keyValue in keysToModify)
                    keyValue.ShowMyGUI();
        }

        void DiscoverScriptableObject()
        {
            keysToModify = new List<LanguageKeysToModify>();
            Type type = mObject.GetType();
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (FieldInfo field in fields)
            {
                if (field.FieldType == typeof(LanguageKeySO) && (field.IsPublic || Attribute.IsDefined(field, typeof(SerializeField))))
                {
                    var fieldValue = field.GetValue(mObject) as LazyPlatypus.TheLocalizationPackage.LanguageKeySO;
                    keysToModify.Add(new LanguageKeysToModify(mObject,  field.Name, fieldValue, mPartitions, mKeyHandler, mLanguagesToLoad, mPath));
                }
            }
        }
    }
}
#endif