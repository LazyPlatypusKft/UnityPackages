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
using System.Collections.Generic;
using UnityEngine;

namespace LazyPlatypus.TheLocalizationPackage
{
    [Serializable]
    [CreateAssetMenu(fileName = "Language", menuName = "LP_Localization/Language", order = 10)]
    public class LanguageSO : ScriptableObject
    {
        [SerializeField] public int PreferredIndex;
        [SerializeField] public string EnglishName;
        [SerializeField] public string NativeName;
        [Header("ONLY USED DURING DEVELOPMENT")]
        [SerializeField] public string ShortHand;
        [Header("Paths")]
        [SerializeField] public List<LocalizedLanguagePartitionSO> Partitions;

        public string GetPath(string _type)
        {
            for (int i = 0; i < Partitions.Count; i++)
                if (Partitions[i].PartitionKey.KEY == _type)
                    return Partitions[i].PATH;
            return "";
        }
    }
}