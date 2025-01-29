#define LOGS

using System;
using UnityEngine;

namespace Architecture
{
    /// <summary>
    /// Defines different multipliers to handle time scale. This way, gameplay elements 
    /// may affect time scale while the UI pauses the game and preserve the correct 
    /// gameplay time scale when the user closes the pause menu.
    /// </summary>
    public static class TimeScales
    {
        #region CATEGORIES

        /// <summary>
        /// Defines the different time scales that will be tracked.
        /// These are multiplicative, if Gameplay = 1 and UI = 0, 
        /// the global time scale will be zero.
        /// </summary>
        private enum Category
        {
            Gameplay,
            UI,

#if CHIBIG_DEBUG
            Debug
#endif
        }

        /// <summary>
        /// Multiplies time scale for gameplay purposes.
        /// </summary>
        public static float Gameplay { get => GetTimeScale(Category.Gameplay); set { SetTimeScale(Category.Gameplay, value); } }

        /// <summary>
        /// Multiplies time scale for UI purposes.
        /// </summary>
        public static float UI { get => GetTimeScale(Category.UI); set { SetTimeScale(Category.UI, value); } }

#if CHIBIG_DEBUG
        /// <summary>
        /// Multiplies time scale for Debug purposes.
        /// </summary>
        public static float Debug { get => GetTimeScale(Category.Debug); set { SetTimeScale(Category.Debug, value); } }
#endif

#endregion

        #region INITIALIZATION

        private static float[] _scales;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            _scales = new float[Enum.GetValues(typeof(Category)).Length];

            for (int i = 0; i < _scales.Length; i++)
            {
                _scales[i] = 1f;
            }
        }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Sets all time scales to the <paramref name="scale"/> provided and updates Time.timeScale.
        /// </summary>
        /// <param name="scale"></param>
        public static void SetAll(float scale)
        {
            for (int i = 0; i < _scales.Length; i++)
            {
                _scales[i] = scale;
            }
            RecalculateTimeScale();

#if LOGS
            UnityEngine.Debug.Log($"[{nameof(TimeScales)}] Set all categories to {scale.ToString("0.00")}. Time.timeScale = {Time.timeScale.ToString("0.00")}");
#endif
        }

        #endregion

        #region PRIVATE METHODS

        private static float GetTimeScale(Category category)
        {
            return _scales[(int)category];
        }

        private static void SetTimeScale(Category category, float scale)
        {
            _scales[(int)category] = scale;
            RecalculateTimeScale();

#if LOGS
            UnityEngine.Debug.Log($"[{nameof(TimeScales)}] Set {category} to {scale.ToString("0.00")}. Time.timeScale = {Time.timeScale.ToString("0.00")}");
            //UnityEngine.Debug.LogWarning($"[{nameof(TimeScales)}] Set {category} to {scale.ToString("0.00")}. Time.timeScale = {Time.timeScale.ToString("0.00")}");
            //UnityEngine.Debug.LogError($"[{nameof(TimeScales)}] Set {category} to {scale.ToString("0.00")}. Time.timeScale = {Time.timeScale.ToString("0.00")}");
#endif

        }

        private static void RecalculateTimeScale()
        {
            float scale = 1.0f;
            for (int i = 0; i < _scales.Length; i++)
            {
                scale *= _scales[i];
            }
            Time.timeScale = scale;
        }

        #endregion
    }
}
