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
    [CreateAssetMenu(fileName = "L_Dictionary", menuName = "LP_Localization/Language - Dictionary", order = 12)]
    public class LoadableLanguageDictionarySO : ScriptableObject
    {
        [SerializeField] public LanguagePartitionSO TheType;
        [SerializeField] public List<LanguageKeyValueSO> TheDictionary;

        public string GetValue(string key)
        {
            for (int i = 0; i < TheDictionary.Count; i++)
                if (TheDictionary[i].TheKey.KEY == key)
                    return TheDictionary[i].TheValue;
            return "<<LOCALIZATION ERROR>>";
        }
    }
}