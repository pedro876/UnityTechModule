using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Architecture
{
    public class SceneService : IService, ISceneLoader
    {
        #region Variables

        //Components
        private IFader fader;

        //Data
        private string activeScene;

        private Dictionary<string, AsyncOperationHandle<SceneInstance>> enabled;
        private Dictionary<string, AsyncOperationHandle<SceneInstance>> loadingEnable;
        private Dictionary<string, AsyncOperationHandle<SceneInstance>> loadingDisable;
        private Dictionary<string, AsyncOperationHandle<SceneInstance>> disabled;
        private Dictionary<string, AsyncOperationHandle<SceneInstance>> unloading;

        //Events
        public event Action<string> onEnabled;
        public event Action<string> onLoaded;
        public event Action<string> onUnloaded;
        public event Action<string> onChangeActiveScene;

        //Constants
        private const float fadeTime = 0.25f;

        #endregion

        #region Base Methods

        public void Initialize()
        {
            enabled = new();
            loadingEnable = new();
            loadingDisable = new();
            disabled = new();
            unloading = new();

            activeScene = "BootScene";
        }

        public void Dispose() { }

        public void SetFader(IFader fader) => this.fader = fader;

        #endregion

        #region Progress Methods

        public float GetLoadingProgress()
        {
            if (loadingEnable.Count == 0 && loadingDisable.Count == 0) return 1f;

            float avgProgress = 0;
            foreach (var op in loadingEnable.Values) avgProgress += op.PercentComplete;
            foreach (var op in loadingDisable.Values) avgProgress += op.PercentComplete;

            return avgProgress / (loadingEnable.Count + loadingDisable.Count);
        }

        public float GetLoadingProgress(string key)
        {
            if (enabled.ContainsKey(key) || disabled.ContainsKey(key)) return 1f;
            if (loadingEnable.TryGetValue(key, out var op) || loadingDisable.TryGetValue(key, out op)) return op.PercentComplete;

            return -1;
        }

        public float GetUnloadingProgress()
        {
            if (loadingEnable.Count == 0) return 1f;

            float avgProgress = 0;
            foreach (var op in unloading.Values) avgProgress += op.PercentComplete;

            return avgProgress / loadingEnable.Count;
        }

        public float GetUnloadingProgress(string key)
        {
            if (unloading.TryGetValue(key, out var op)) return op.PercentComplete;

            return -1;
        }

        #endregion

        #region Manage Methods

        public string GetActiveScene() => activeScene;

        public bool ChangeActiveScene(string key)
        {
            if (!enabled.TryGetValue(key, out var op)) return false;

            bool change = SceneManager.SetActiveScene(op.Result.Scene);

            if (change) onChangeActiveScene?.Invoke(key);

            return change;
        }

        public bool HasDisableScenes() => disabled.Count > 0;

        public bool EnableAll()
        {
            bool anyChange = false;

            foreach (var key in disabled.Keys) anyChange = Enable(key) || anyChange;
            foreach (var key in loadingDisable.Keys) anyChange = Enable(key) || anyChange;

            return anyChange;
        }

        public bool Enable(string key)
        {
            if (disabled.TryGetValue(key, out var op))
            {
                op.Result.ActivateAsync();

                disabled.Remove(key);
                enabled.Add(key, op);

                onEnabled?.Invoke(key);
                return true;
            }
            else if (loadingDisable.TryGetValue(key, out op))
            {
                op.Result.ActivateAsync();
                loadingEnable[key] = op;
                return true;
            }

            return false;
        }

        #endregion

        #region Load Methods

        public bool IsLoading(string key) => loadingEnable.ContainsKey(key) || loadingDisable.ContainsKey(key);
        public bool IsLoaded(string key) => enabled.ContainsKey(key) || disabled.ContainsKey(key);

        public bool LoadAsync(string key, bool allowActivation, bool enableFade = true, Action onCompleted = null)
        {
            if (!TryLoadScene(key, allowActivation, LoadSceneMode.Single, out var op)) return false;

            CoroutineManager.Start(CO_LoadAsync(key, enableFade, op, onCompleted));
            return true;
        }

        public bool AddAsync(string key, bool allowActivation, bool enableFade = true, Action onCompleted = null)
        {
            if (!TryLoadScene(key, allowActivation, LoadSceneMode.Additive, out var op)) return false;

            CoroutineManager.Start(CO_LoadAsync(key, enableFade, op, onCompleted));
            return true;
        }

        public bool UnloadAsync(string key, Action onCompleted = null)
        {
            if (!enabled.TryGetValue(key, out var op) || !disabled.TryGetValue(key, out op)) return false;
            if (unloading.ContainsKey(key) || loadingEnable.ContainsKey(key)) return false;

            enabled.Remove(key);

            unloading[key] = Addressables.UnloadSceneAsync(op);
            unloading[key].Completed += (unload) =>
            {
                onUnloaded?.Invoke(key);
                onCompleted?.Invoke();
            };

            return true;
        }

        #endregion

        #region Private Methods

        private bool TryLoadScene(string key, bool allowActivation, LoadSceneMode mode, out AsyncOperationHandle<SceneInstance> op)
        {
            op = default;

            if (BeingOperated(key)) return false;

            op = Addressables.LoadSceneAsync(key, mode, allowActivation);

            if (op.Status == AsyncOperationStatus.Failed)
            {
                Addressables.Release(op);
                return false;
            }

            if (allowActivation) loadingEnable[key] = op;
            else loadingDisable[key] = op;

            return true;
        }

        private bool BeingOperated(string key)
        {
            if (enabled.ContainsKey(key)) return true;
            if (loadingEnable.ContainsKey(key)) return true;
            if (unloading.ContainsKey(key)) return true;
            if (disabled.ContainsKey(key)) return true;

            return false;
        }

        private IEnumerator CO_LoadAsync(string key, bool fade, AsyncOperationHandle<SceneInstance> op, Action onCompleted)
        {
            if (fade) yield return fader?.CO_FadeOut(fadeTime);

            yield return op.WaitForCompletion();

            if (loadingDisable.ContainsKey(key))
            {
                loadingDisable.Remove(key);
                disabled.Add(key, op);
            }
            else if (loadingEnable.ContainsKey(key))
            {
                loadingEnable.Remove(key);
                enabled.Add(key, op);
            }

            onLoaded?.Invoke(key);

            if (fade) yield return fader?.CO_FadeIn(fadeTime);

            onCompleted?.Invoke();
        }

        #endregion
    }
}
