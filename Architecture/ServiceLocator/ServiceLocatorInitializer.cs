#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace Architecture
{
    public static partial class ServiceLocator
    {
#if !UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void BuildInitialize()
        {
            Application.quitting += OnQuittingApplication;

            Initialize();
        }

        private static void OnQuittingApplication()
        {
            Dispose();
        }
#else
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void EditorInitialize()
        {
            Initialize();
            EditorApplication.playModeStateChanged += OnPlayModeChange;
        }

        private static void OnPlayModeChange(PlayModeStateChange ctx)
        {
            if (ctx == PlayModeStateChange.ExitingPlayMode)
            {
                EditorApplication.playModeStateChanged -= OnPlayModeChange;
                Dispose();
            }
        }
#endif
    }
}
