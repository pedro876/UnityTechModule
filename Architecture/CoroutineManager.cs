using System;
using System.Collections;
using UnityEngine;

namespace Architecture
{
    public class CoroutineManager : MonoBehaviour
    {
        #region Variable

        private static CoroutineManager instance;
        public static CoroutineManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new("CoroutineManager");
                    instance = go.AddComponent<CoroutineManager>();
                    DontDestroyOnLoad(go);
                }

                return instance;
            }
        }

        #endregion

        #region Base Methods

        public static Coroutine Start(IEnumerator routine) => Instance.StartCoroutine(Instance.CO_WrappedRoutine(routine));
        public static Coroutine StartDelay(YieldInstruction instruction, Action onFinish) => Instance.StartCoroutine(Instance.CO_WrappedRoutine(Instance.CO_Delay(instruction, onFinish)));

        public static void Stop(ref Coroutine coroutine)
        {
            if (coroutine != null) Instance.StopCoroutine(coroutine);
            coroutine = null;
        }

        public static void StopAll() => Instance.StopAllCoroutines();

        private IEnumerator CO_WrappedRoutine(IEnumerator routine)
        {
            while (true)
            {
                try
                {
                    if (!routine.MoveNext()) break;
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Error in Coroutine {routine.GetType().Name}: {ex.Message}");
                    break;
                }

                yield return routine.Current;
            }
        }

        private IEnumerator CO_Delay(YieldInstruction instruction, Action onFinish)
        {
            yield return instruction;

            onFinish?.Invoke();
        }

        #endregion
    }
}
