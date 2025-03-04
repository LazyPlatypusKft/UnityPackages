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

using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace LazyPlatypus.TheLocalizationPackage
{
    public class LocalizationHelper : MonoBehaviour
    {
        [SerializeField] public LanguagePartitionSO Tipe;
        [SerializeField] public List<TMP_Text> Texts;
        [SerializeField] public List<LanguageKeySO> Keys;

        void Start()
        {
            if (TheLocalizationSystem.Instance != null)
                TheLocalizationSystem.Instance.Subscribe(this, Tipe.KEY);
        }

        public void ApplyKeys(LoadedLanguage _keys)
        {
            for (int i = 0; i < Keys.Count; i++)
                Texts[i].text = _keys.GetLocalizedText(Keys[i].KEY);
        }

        public void AddTextWithKey(TMP_Text _textComp, LanguageKeySO _key)
        {
            Texts.Add(_textComp);
            Keys.Add(_key);
        }

        void OnDestroy()
        {
            if (TheLocalizationSystem.Instance != null)
                TheLocalizationSystem.Instance.Unsubscribe(this);
        }
    }
}