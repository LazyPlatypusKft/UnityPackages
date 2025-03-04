/**************************************************************************
 *  Lazy Platypus Kft. - Game Development Studio
 *
 *  Copyright © 2025 Lazy Platypus Kft. All rights reserved.
 *
 *  This script is part of the Lazy Platypus Kft. game framework and is
 *  protected under applicable copyright laws. Unauthorized use,
 *  distribution, or modification of this file is strictly prohibited.
 *
 *  For licensing inquiries, please contact: info@lazyplatypus.com
 *
 *  ───────────────────────────────────────────────────────────────
 *  "Games with Heart, Crafted with Code." - Lazy Platypus Kft.
 *  ───────────────────────────────────────────────────────────────
 **************************************************************************/

#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;

namespace LazyPlatypus.TheFolderSupportPackage
{
    [Serializable]
    [CreateAssetMenu(fileName = "FolderConfiguration", menuName = "LP_FolderSupport/FolderConfiguration", order = 0)]
    public class FolderConfigurationSO : ScriptableObject
    {
        [SerializeField] public string linkedAssetId = "-";
        [SerializeField] public string selectedPath = "Assets";

        public virtual void MyOnGUI()
        {
            GUILayout.Label("Path Configuration: ", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();

            // Display the short name of the folder with a tooltip for the full path
            string displayName = FolderStructures.GetDisplayName(selectedPath);
            GUIContent labelContent = new GUIContent(displayName, selectedPath); // Tooltip is set here
            EditorGUILayout.LabelField(labelContent);

            // Configure button
            if (GUILayout.Button("Configure", GUILayout.Width(100)))
            {
                FolderConfConfiguratorPopUp.Show(selectedPath, PathUpdated);
            }

            EditorGUILayout.EndHorizontal();
        }

        void PathUpdated(string _path)
        {
            selectedPath = _path;
            
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public string GetPath()
        {
            return selectedPath;
        }
    }
}
#endif