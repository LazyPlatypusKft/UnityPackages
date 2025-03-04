# The Localization Package
Current version: 1.0.0

The localization package helps with localizing keys, creating separate partitions and let you load and unload different packages with minimum manual setup from the developer's end. To get a better understanding of the content, either browse the given files, or look up the tutorial on youtube: 

## High overview and key definitions:
- _**Language(SO):**_ The data that represents a given language. Read the class description for more info.
- _**(Language)Partition(SO):**_ A unit of translations, which can be applied. This is a good way to separate different parts of the translation keys, and categorize them. Good examples would be: "Menu", "Game part - 1", "Game part - 2" etc.
- **_LanguageKey(SO):_** The key for a given translation
- **_LanguageKeyValue(SO):_** The translation

helpers:
- **_(Loadable)LanguageDictionary(SO):_** All keys for a given partition. Strong connection with LocalizedLanguagePartitionSO.
- **_LocalizedLanguagePartition(SO):_** Contains the **Partition** and the Path to the Partition Addressable (later)

So the smallest element is **LanguageKeyValue**, which contains the very specific translation for the very specific key. Above that there's the **LanguageKeys**, which are part of a single **Partition**. These keys has to contain a unique string, by which they can be identified. The next one is the **Partition**, which is a collection of keys, which are in connection with each other. Then the **Language**, which is defining how many different folders and copies of keys should be created.

## Requirements:
- From Lazy Platypus Kft: TheFolderSupportPackage
- From Unity: Addressables (com.unity.addressables)

## Usage:
### Editor part:
- Open the Localization Helper from **Unity's header / LazyPlatypus / Localization Helper**
- Configure the path where you want to store all the localization assets by clicking on the Configure button, and then navigating to the desired path.
- Open up the Language Editor, and create your first language by clicking on the Create Language button. Fill in the fields and read the info bubbles for detailed help.
- Then Open up the Language Partition Editor, and create a new partition (like "Menu").
- Then drag and drop a GameObject from the Scene (or a Prefab from the project folder), into the **_Localization Root Object:_** field
- Press "Attach Localization Component". Select the newly created partition. Then down below you can see which Text can be found in the given canvas. Click on Add Key to create a new localization Key. Follow through the dialog.
- Click on the "Change Translation" button if you want to change the translation key in the Languages you have added to the game
- Click on the Refresh button on the top if the changes does not appear instantly.
- Done


### Before the runtime part:
- Create an empty GameObject. Attach the TheLocalizationSystem Component.
- Add the default language and add all the Languages into the list.
- Add all the partitions which should be preloaded. E.g. Menu should be available right away, so add that to the list.
- Check in VERBOSE, so that you can more easily identify if there's an issue with the setup.
- Done

## Scripting use cases:
### Loading a different Partition:
- Use `TheLocalizationSystem.Instance.LoadPartition(ThePartition.KEY, CallbackToContinue);`
- Where the CallbackToContinue would be `StartCoroutine(LoadMyStuff());`
- LoadMyStuff should be an enum, starting with: `yield return new WaitForEndOfFrame();` there needs to be a frame skipped before the loaded locals could be used, because of the asynchronous mode it is working. 
- Done
### Dynamic changes or compound changes:
In case you need dynamic changes e.g. the same text component changes texts mid-run. 
- Use the `TheLocalizationSystem.Instance.Localize` to also apply the text.
- Or use the `TheLocalizationSystem.Instance.LocalizeKey` to only retrieve the translated key
- Or use the `TheLocalizationSystem.Instance.LocalizeCompound` in case you have a string like "Please [0] use the [1] to open the [2]", where you can exchange the given keys dynamically.