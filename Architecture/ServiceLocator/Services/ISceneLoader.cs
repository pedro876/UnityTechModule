using System;

namespace Architecture
{
    public interface ISceneLoader
    {
        /// <summary>
        /// Triggered when a scene is enabled.
        /// </summary>
        public event Action<string> onEnabled;

        /// <summary>
        /// Triggered when a scene is enabled, whether it is enabled or disabled.
        /// </summary>
        public event Action<string> onLoaded;

        /// <summary>
        /// Triggered when a scene is unloaded.
        /// </summary>
        public event Action<string> onUnloaded;

        /// <summary>
        /// Triggered when a scene is set as the active scene, that is, the main scene or most important one.
        /// </summary>
        public event Action<string> onChangeActiveScene;

        /// <summary>
        /// Enables activation of all scenes (loaded but disabled and also those that are still being loaded) 
        /// </summary>
        /// <returns>True if any scene was enabled</returns>
        bool EnableAll();

        /// <summary>
        /// Enables the activation of a particular scene.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>True if it could be enabled</returns>
        bool Enable(string key);

        /// <returns>The build index of the current active scene (main)</returns>
        string GetActiveScene();

        /// <summary>
        /// Tries to set a particular scene as the active scene.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>True if it could be done</returns>
        bool ChangeActiveScene(string key);

        bool HasDisableScenes();

        /// <param name="key"></param>
        /// <returns>True if the scene is being loaded.</returns>
        bool IsLoading(string key);

        /// <param name="key"></param>
        /// <returns>True if the scene is loaded, whether it's enabled or not.</returns>
        bool IsLoaded(string key);

        /// <summary>
        /// Tries to load a scene asynchronously. If the activation is enabled, it will be set as
        /// the active scene once it is loaded and all other scenes will be unloaded.
        /// If activation is not allowed, it will have to be manually enabled. Once it is loaded, the
        /// callback will be executed.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="allowActivation"></param>
        /// <param name="onCompleted"></param>
        /// <returns>True if it could be done</returns>
        bool LoadAsync(string key, bool allowActivation, bool enableFade = true, Action onCompleted = null);

        /// <summary>
        /// Tries to add a scene asynchronously. If the activation is enabled, it will be added as soon
        /// as it is loaded without changing the active scene.
        /// If activation is not allowed, it will have to be manually enabled. Once it is loaded, the
        /// callback will be executed.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="allowActivation"></param>
        /// <param name="onCompleted"></param>
        /// <returns>True if it could be done</returns>
        bool AddAsync(string key, bool allowActivation, bool enableFade = true, Action onCompleted = null);

        /// <summary>
        /// Tries to unload a scene asynchronously, will only work if it is already loaded, whether enabled or disabled.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="onCompleted"></param>
        /// <returns>True if it could be done</returns>
        bool UnloadAsync(string key, Action onCompleted = null);

        /// <returns>The averaged loading progress of all the scenes that are currently being loaded</returns>
        float GetLoadingProgress();

        /// <param name="key"></param>
        /// <returns>The loading progress of a particular scene</returns>
        float GetLoadingProgress(string key);

        /// <returns>The averaged unloading progress of all the scenes that are currently being unloaded</returns>
        float GetUnloadingProgress();

        /// <param name="key"></param>
        /// <returns>The unloading progress of a particular scene</returns>
        float GetUnloadingProgress(string key);
    }
}
