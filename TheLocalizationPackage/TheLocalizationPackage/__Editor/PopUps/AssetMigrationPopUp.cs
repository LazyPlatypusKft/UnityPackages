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
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace LazyPlatypus.TheLocalizationPackage.Editor
{
    public class AssetMigrationPopUp : EditorWindow
    {
        ScriptableObject mObject;

        Dictionary<string, string> mFieldsToOverwrite;
        
        const string USING_STATEMENT = "using LazyPlatypus_Salata;";
        
        public static void Show()
        {
            var window = CreateInstance<AssetMigrationPopUp>();
            window.titleContent = new GUIContent("Migrating Strings to Localized Version");
            window.minSize = new Vector2(350, 250);
            window.ShowUtility();
        }

        void OnGUI()
        {
            EditorGUILayout.HelpBox("!!!!!!!!!!! CAN NOT DEAL WITH NESTED SCRIPTABLES !!!!!!!!!!!", MessageType.Error);
            EditorGUILayout.HelpBox("PLEASE MAKE SURE, that you made a backup, if this tools fails to modify your ScriptableObject!!", MessageType.Error);
            
            EditorGUI.BeginChangeCheck();
            mObject = (ScriptableObject)EditorGUILayout.ObjectField("Migration Object", mObject, typeof(ScriptableObject), true);
            if (EditorGUI.EndChangeCheck())
                DiscoverScriptableObject();

            if (mObject != null)
            {
                EditorGUILayout.HelpBox("THESE WILL MODIFY YOUR SCRIPTABLE OBJECT SCRIPT, MAKE SURE TO MAKE A BACKUP!", MessageType.Error);
                foreach (var keyValue in mFieldsToOverwrite)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(keyValue.Key + " is present, would you like to create " + keyValue.Value + "?");
                    if (GUILayout.Button("Add", GUILayout.Width(100)))
                        AddLocalizationTo(keyValue);
                    EditorGUILayout.EndHorizontal();
                }
                if (mFieldsToOverwrite.Count > 1)
                {
                    EditorGUILayout.Space();
                    if (GUILayout.Button("Add All Values", GUILayout.Width(100)))
                        AddLocalizationTo(mFieldsToOverwrite);
                }
            }
        }
        
        void DiscoverScriptableObject()
        {
            Type type = mObject.GetType();
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            mFieldsToOverwrite = new Dictionary<string, string>();
            foreach (FieldInfo field in fields)
            {
                // Gather ALL possible fields to convert (if not already converted)
                if (field.FieldType == typeof(string) && (field.IsPublic || Attribute.IsDefined(field, typeof(SerializeField))))
                    if (type.GetField(field.Name + "Localized", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) == null)
                        mFieldsToOverwrite[field.Name] = field.Name + "Localized";
            }
        }

        void AddLocalizationTo(KeyValuePair<string, string> keyValue)
        {
            if (mObject == null)
                return;
            
            var scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(mObject));
            var lines = File.ReadAllLines(scriptPath);
            var linesList = lines.ToList();
            InsertLazyPlatypusUtilsOnTop(linesList);
            InsertKeyValue(linesList, keyValue);
            
            File.WriteAllLines(scriptPath, linesList.ToArray());
            AssetDatabase.Refresh();
        }

        void AddLocalizationTo(Dictionary<string, string> _dictionary)
        {
            if (mObject == null)
                return;
            
            var scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(mObject));
            var lines = File.ReadAllLines(scriptPath);
            var linesList = lines.ToList();
            InsertLazyPlatypusUtilsOnTop(linesList);
            foreach (var keyValue in _dictionary)
                InsertKeyValue(linesList, keyValue);
            
            File.WriteAllLines(scriptPath, linesList.ToArray());
            AssetDatabase.Refresh();
        }
        
        void InsertKeyValue(List<string> _linesList, KeyValuePair<string, string> _keyValuePair)
        {
            var fieldName = _keyValuePair.Key;
            var newFieldName = _keyValuePair.Value;
            var localizedFieldLine = $"    [SerializeField] internal LanguageKeySO {newFieldName};";
            var fieldIndex = _linesList.FindIndex(line => line.Contains($"string {fieldName}") && line.Trim().EndsWith(";"));
            if (fieldIndex != -1)
            {
                // Check if the localized field already exists to avoid duplicates
                if (_linesList.Any(line => line.Contains($"LanguageKeySO {newFieldName}")))
                    return;
                _linesList.Insert(fieldIndex + 1, localizedFieldLine);
            }
        }

        void InsertLazyPlatypusUtilsOnTop(List<string> _lines)
        {
            // Step 0: Already using it, return
            if (_lines.Any(line => line.Trim() == USING_STATEMENT))
                return;

            int insertIndex = 0;
            // Step 1: Find first "using" statement
            for (int i = 0; i < _lines.Count; i++)
            {
                if (_lines[i].Trim().StartsWith("using "))
                {
                    insertIndex = i + 1; // Place after last using
                }
            }

            // Step 2: If no "using" found, look for "namespace"
            if (insertIndex == 0)
            {
                for (int i = 0; i < _lines.Count; i++)
                {
                    if (_lines[i].Trim().StartsWith("namespace "))
                    {
                        insertIndex = i; // Place before namespace
                        break;
                    }
                }
            }

            // Step 3: If no namespace found, look for first class declaration (`public` or `internal`)
            if (insertIndex == 0)
            {
                for (int i = 0; i < _lines.Count; i++)
                {
                    if (_lines[i].Trim().StartsWith("public ") || _lines[i].Trim().StartsWith("internal "))
                    {
                        insertIndex = i; // Place before first class declaration
                        break;
                    }
                }
            }
            
            // Step 4: If still nothing found, insert at the top
            _lines.Insert(insertIndex, USING_STATEMENT);
        }
    }
}
#endif