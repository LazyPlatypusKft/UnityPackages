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
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace LazyPlatypus.TheLocalizationPackage
{
    public class TheLocalizationSystem : MonoBehaviour
    {
        public static TheLocalizationSystem Instance;

        [Header("Turn this on, IF you need more logs. But turn this off in production, to not pollute the logs.")]
        [SerializeField] bool VERBOSE = true;
        [Header("Drag drop all languages here, and define the default")]
        [SerializeField] LanguageSO DefaultLanguage;
        [SerializeField] List<LanguageSO> Languages;
        [Header("Drag drop all partitions which should be preloaded. E.g. 'MENU'")]
        [SerializeField] List<LanguagePartitionSO> PreLoadedLanguagePartitions;
        
        LanguageSO mCurrentLanguage;
        List<string> mLoadedPartitions = new List<string>();
        Dictionary<string, LoadedLanguage> mLoadedLanguagePartitions = new Dictionary<string, LoadedLanguage>();
        List<string> mPartitionsToLoad = new List<string>();
        Dictionary<LocalizationHelper, string> mLocalizationDisplays = new Dictionary<LocalizationHelper, string>();
        List<ILightLanguageUpdate> mLightLanguageInterfaces = new List<ILightLanguageUpdate>();

        // Change this to anything that you want to have in the PlayerPrefs, contains the EnglishName of the given Language
        const string LANGUAGE_KEY = "SetLanguage_ZS58415";
        
        void Awake()
        {
            Instance = this;
            mLoadedPartitions = new List<string>();
            if (!PlayerPrefs.HasKey(LANGUAGE_KEY))
                mCurrentLanguage = DefaultLanguage;
            else
            {
                var englishLanguageName = PlayerPrefs.GetString("SetLanguage_ZS58415");
                for (int i = 0; i < Languages.Count; i++)
                {
                    if (Languages[i].EnglishName == englishLanguageName)
                    {
                        mCurrentLanguage = Languages[i];
                        break;
                    }
                }
                if (mCurrentLanguage == null)
                    mCurrentLanguage = DefaultLanguage;
            }
            for (int i = 0; i < PreLoadedLanguagePartitions.Count; i++)
                mLoadedPartitions.Add(PreLoadedLanguagePartitions[i].KEY);
            UnloadCurrentAndSetNewLanguage(mCurrentLanguage);
        }

        /* Change the loaded language.
         * If changing to the same language, function call is ignored
         */
        public void ChangeLanguage(LanguageSO _language)
        {
            UnloadCurrentAndSetNewLanguage(_language);
        }

        /* Loading a given partition of the Language Applicator
         */
        public void LoadPartition(string _type, Action _onLoadFinished)
        {
            if (mLoadedPartitions.Contains(_type))
            {
                if (VERBOSE)
                    Debug.LogWarning($"[LP_Language]: The given partition ({_type}) has already been loaded!");
                return;
            }
            mPartitionsToLoad.Add(mCurrentLanguage.EnglishName);
            mLoadedLanguagePartitions[_type] = new LoadedLanguage(mCurrentLanguage.EnglishName, mCurrentLanguage.GetPath(_type));
            mLoadedLanguagePartitions[_type].LoadLanguage(_onLoadFinished);
            mLoadedPartitions.Add(_type);
        }

        /* Unloading a given partition of the Language Applicator
         */
        public void UnloadPartition(string _type)
        {
            if (VERBOSE)
                Debug.LogWarning($"[LP_Language]: Unloading partition ({_type})!");
            
            mLoadedLanguagePartitions[_type].Unload();
            mLoadedPartitions.Remove(_type);
            mLoadedLanguagePartitions.Remove(_type);
        }

        /* Subscribe a LightLanguageUpdate interface
         * so that the text can be updated when the Language changed. Don't forget to unsubscribe OnDestroy
         */
        public void Subscribe(ILightLanguageUpdate _helper)
        {
            mLightLanguageInterfaces.Add(_helper);   
        }
        
        /* Unsubscribe a LightLanguageUpdate interface
         */
        public void Unsubscribe(ILightLanguageUpdate _helper)
        {
            mLightLanguageInterfaces.Remove(_helper);   
        }
        
        /* Subscribe a LocalizationHelper Behaviour
         * so that the text can be updated when the Language changed
         */
        public void Subscribe(LocalizationHelper _helper, string _type)
        {
            mLocalizationDisplays[_helper] = _type;
            if (mLoadedLanguagePartitions.ContainsKey(_type) && mLoadedLanguagePartitions[_type].IsLoaded)
                _helper.ApplyKeys(mLoadedLanguagePartitions[_type]);
        }

        /* Unsubscribe a LocalizationHelper Behaviour
         * so that the text is no longer updated if not necessary
         */
        public void Unsubscribe(LocalizationHelper _helper)
        {
            if (mLocalizationDisplays.ContainsKey(_helper))
                mLocalizationDisplays.Remove(_helper);
        }

        /* Localizes the given TMP_Text's text
         * Use this, if you have a project using TMP_Text components, and if you are displaying something that changes dynamically
         */
        public void Localize(LanguageKeySO _localizationKey, TMP_Text _textBody)
        {
            var translation = LocalizeKey(_localizationKey);
            if (translation != "")
                _textBody.text = translation;
        }
        public void Localize(TMP_Text _textBody, LanguageKeySO _localizationKey)
        {
            Localize(_localizationKey, _textBody);
        }

        /* Localizes the given Text's text
         * Use this, if you have a project using Text instead of TMP_Text components, and if you are displaying something that changes dynamically
         */
        public void Localize(LanguageKeySO _localizationKey, Text _textBody)
        {
            var translation = LocalizeKey(_localizationKey);
            if (translation != "")
                _textBody.text = translation;
        }
        public void Localize(Text _textBody, LanguageKeySO _localizationKey)
        {
            Localize(_localizationKey, _textBody);
        }

        /* Get Translation for a given Key.
         * Use this, if you need a specific translation, or if you are displaying something that changes dynamically
         */
        public string LocalizeKey(LanguageKeySO _localizationKey)
        {
            var translation = "";
            var hasTranslation = false;
            var locKey = _localizationKey.KEY;
            
            foreach (var keyValue in mLoadedLanguagePartitions)
            {
                if (keyValue.Value.TryGetKey(locKey, out translation))
                {
                    hasTranslation = true;
                    break;
                }
            }
            if (hasTranslation)
                return translation;
            return "";
        }

        /* Localize a compound string.
         * Use this, if you are translating a compound string
         */
        public void LocalizeCompound(LanguageKeySO _key, string[] _inputs, TMP_Text _text)
        {
            var translation = LocalizeKey(_key);
            if (translation == "")
                return;
            for (int i = 0; i < _inputs.Length; i++)
            {
                var placeholder = $"[{i}]";
                translation = translation.Replace(placeholder, _inputs[i]);
            }
            _text.text = translation;
        }
        public void LocalizeCompound(TMP_Text _text, LanguageKeySO _key, string[] _inputs)
        {
            LocalizeCompound(_key, _inputs, _text);
        }

        void UnloadCurrentAndSetNewLanguage(LanguageSO _language)
        {
            if (_language == mCurrentLanguage)
                return;
            
            if (mCurrentLanguage != null)
                UnloadLanguage();
            
            mCurrentLanguage = _language;
            for (int i = 0; i < mLoadedPartitions.Count; i++)
            {
                mPartitionsToLoad.Add(mCurrentLanguage.EnglishName);
                mLoadedLanguagePartitions[mLoadedPartitions[i]] = new LoadedLanguage(_language.EnglishName, _language.GetPath(mLoadedPartitions[i]));
                mLoadedLanguagePartitions[mLoadedPartitions[i]].LoadLanguage();
            }
        }

        void UnloadLanguage()
        {
            foreach (var keyValue in mLoadedLanguagePartitions)
                keyValue.Value.Unload();
            mLoadedLanguagePartitions = new Dictionary<string, LoadedLanguage>();
        }

        public void LanguageLoadedCallbackFunction(string _tipe)
        {
            mPartitionsToLoad.Remove(_tipe);
            if (mPartitionsToLoad.Count == 0)
                StartCoroutine(UpdateLocalsCoroutine());
        }

        IEnumerator UpdateLocalsCoroutine()
        {
            yield return new WaitForNextFrameUnit();
            if (VERBOSE)
                Debug.LogWarning($"[LP_Language]: Updating all displays!");
            foreach (var keyValue in mLocalizationDisplays)
                if (mLoadedLanguagePartitions.ContainsKey(keyValue.Value))
                    keyValue.Key.ApplyKeys(mLoadedLanguagePartitions[keyValue.Value]);
            for (int i = mLightLanguageInterfaces.Count - 1; i >= 0; i--)
            {
                if (mLightLanguageInterfaces[i] == null)
                    mLightLanguageInterfaces.RemoveAt(i);
                else
                    mLightLanguageInterfaces[i].UpdateLanguage();
            }
        }
    }
}