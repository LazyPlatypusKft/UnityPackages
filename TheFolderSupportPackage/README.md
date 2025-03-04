# The Folder Support Package
Current version: 1.0.0

Used to make it easier to navigate between folders, retrieve ScriptableObjects or delete or create them.

## Usage:
The only structure there is the FolderConfigurationSO. Which uses LinkedAssetId. This is something that has to be unique for the given project, and saves the relative path for the given folder. E.g. We are using the Localization package, and saving the root folder for the translation keys using the FolderConfigurationSO. The LinkedAssetId is LP_LanguagePackageWindow, while the selected path should be left alone. It is filled by using the MyOnGUI(). All these assets are stored in the Assets/LazyPlatypus/Configurations folder.

### Main functions: 
- EnsureFolderExists: creates the directory recursively
- GetRelativeAssetsPath: converts a full path to a given asset to Unity relative path (from the Assets folder)
- GetDisplayName: retrieves the last folder's name, this is to shorten the displayed path.
- RetrieveConfigurationAsset: Retrieves the given folder asset with the given LinkedAssetId.
- CreateAsset: Creates an assets at a given path, use the Action<T> _assetSetup = null to setup your custom ScriptableObject
- RenameAsset: Renames the given asset, you can use the _assetSetup callback to modify its values.
- DeletingGenericAsset: Delete an asset at a given path, it will show a pop up to ensure that you are deleting the right assets.
- FindAssets: Finds assets of a given type at the location either recursively or normally, returning an  empty array if found none.