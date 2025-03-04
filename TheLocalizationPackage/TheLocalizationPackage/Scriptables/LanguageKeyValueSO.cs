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

using System;
using UnityEngine;

namespace LazyPlatypus.TheLocalizationPackage
{
    [Serializable]
    [CreateAssetMenu(fileName = "L_KeyValue", menuName = "LP_Localization/Language - Key Value", order = 13)]
    public class LanguageKeyValueSO : ScriptableObject
    {
        [SerializeField] public LanguageKeySO TheKey;
        [SerializeField] public string TheValue;
    }
}