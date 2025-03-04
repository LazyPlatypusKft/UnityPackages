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
    [CreateAssetMenu(fileName = "Partition", menuName = "LP_Localization/Language - LocPartition", order = 11)]
    public class LocalizedLanguagePartitionSO : ScriptableObject
    {
        [SerializeField] public LanguagePartitionSO PartitionKey;
        [SerializeField] public string PATH;
    }
}