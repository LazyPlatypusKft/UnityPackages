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
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace LazyPlatypus.TheLocalizationPackage
{
    [Serializable]
    public class LoadedLanguage
    {
        [SerializeField] public bool IsLoaded;
        [SerializeField] public string Path;
        [SerializeField] public string Tipe;

        LoadableLanguageDictionarySO mLanguageData;
        
        public LoadedLanguage(string _language, string _path)
        {
            Path = _path;
            Tipe = _language;
        }

        public async void LoadLanguage(Action _onLoadFinished = null)
        {
            AsyncOperationHandle<LoadableLanguageDictionarySO> handle = Addressables.LoadAssetAsync<LoadableLanguageDictionarySO>(Path);
            mLanguageData = await handle.Task;
            IsLoaded = true;
            TheLocalizationSystem.Instance.LanguageLoadedCallbackFunction(Tipe);
            if (_onLoadFinished != null)
                _onLoadFinished?.Invoke();
        }

        public string GetLocalizedText(string key)
        {
            return mLanguageData.GetValue(key);
        }
        
        public void Unload()
        {
            try
            {
                Addressables.Release(mLanguageData);
            }
            catch (Exception _error)
            {
                
            }
        }

        public bool TryGetKey(string _localizationKey, out string translation)
        {
            translation = mLanguageData.GetValue(_localizationKey);
            return translation != "<<LOCALIZATION ERROR>>";
        }
    }
}