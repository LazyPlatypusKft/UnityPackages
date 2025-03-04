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

using System.IO;
using UnityEditor;
using UnityEngine;

namespace LazyPlatypus.TheFolderSupportPackage
{
    public class FolderConfConfiguratorPopUp : EditorWindow
    {
        string mPath;
        System.Action<string> mOnSet;
        Vector2 mScrollPosition;

        public static void Show(string _initialPath, System.Action<string> _onSet)
        {
            var window = CreateInstance<FolderConfConfiguratorPopUp>();
            window.mPath = _initialPath;
            window.mOnSet = _onSet;
            window.titleContent = new GUIContent("Configure Path");
            window.minSize = new Vector2(400, 300);
            window.ShowUtility();
        }

        void OnGUI()
        {
            GUILayout.Label("Path Configuration: ", EditorStyles.boldLabel);

            // Display current path
            EditorGUILayout.BeginHorizontal();
            mPath = EditorGUILayout.TextField("Path", mPath);

            // Up button
            if (GUILayout.Button("Up", GUILayout.Width(50)))
            {
                string parentPath = Path.GetDirectoryName(mPath);
                if (!string.IsNullOrEmpty(parentPath) && parentPath.StartsWith("Assets"))
                {
                    mPath = parentPath;
                }
            }
            EditorGUILayout.EndHorizontal();

            // Warn if path does not exist
            if (!AssetDatabase.IsValidFolder(mPath))
            {
                EditorGUILayout.HelpBox("Path does not exist. It will be created.", MessageType.Warning);
            }

            mScrollPosition = EditorGUILayout.BeginScrollView(mScrollPosition, GUILayout.Height(200));
            // List subdirectories
            string[] subdirectories = AssetDatabase.GetSubFolders(mPath);
            foreach (string subdirectory in subdirectories)
            {
                if (GUILayout.Button(Path.GetFileName(subdirectory)))
                {
                    mPath = subdirectory;
                }
            }
            EditorGUILayout.EndScrollView();

            GUILayout.FlexibleSpace();

            // Set button
            if (GUILayout.Button("Set", GUILayout.Height(30)))
            {
                if (!AssetDatabase.IsValidFolder(mPath))
                {
                    FolderStructures.EnsureFolderExists(mPath);
                }

                mOnSet?.Invoke(mPath);
                Close();
            }
        }
    }
}
#endif