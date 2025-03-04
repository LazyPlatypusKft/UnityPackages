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
using UnityEditor;
using UnityEngine;

namespace LazyPlatypus.TheLocalizationPackage.Editor
{
    [Serializable]
    public class PartitionCreatePopUp : EditorWindow
    {
        string mPartitionName;
        string mOriginalName;
        bool mIsCreateMode;
        Action<int, string> mOnAction;

        public static void Show(string _initialName, bool _createMode, Action<int, string> _onAction)
        {
            var window = CreateInstance<PartitionCreatePopUp>();
            window.mPartitionName = _initialName;
            window.mOriginalName = _initialName;
            window.mIsCreateMode = _createMode;
            window.mOnAction = _onAction;

            window.titleContent = new GUIContent(_createMode ? "Create Partition" : "Edit Partition");
            window.minSize = new Vector2(300, 150);
            window.maxSize = new Vector2(300, 150);
            window.ShowUtility();
        }

        void OnGUI()
        {
            GUILayout.Label(mIsCreateMode ? "Create New Partition" : "Edit Partition", EditorStyles.boldLabel);
            mPartitionName = EditorGUILayout.TextField("Partition Name:", mPartitionName);
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            if (!mIsCreateMode && GUILayout.Button("Cancel", GUILayout.Width(100)))
                Close();

            if (!mIsCreateMode)
            {
                if (GUILayout.Button("Delete", GUILayout.Width(100)))
                {
                    mOnAction?.Invoke(2, mPartitionName);
                    Close();
                }
                if (mOriginalName != titleContent.text && GUILayout.Button("Rename", GUILayout.Width(100)))
                {
                    mOnAction?.Invoke(1, mPartitionName);
                    Close();
                }
            }
            GUILayout.FlexibleSpace();
            if (mIsCreateMode && GUILayout.Button("Create", GUILayout.Width(100)))
            {
                if (!string.IsNullOrEmpty(mPartitionName))
                {
                    mOnAction?.Invoke(0, mPartitionName);
                    Close();
                }
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
    }
}
#endif