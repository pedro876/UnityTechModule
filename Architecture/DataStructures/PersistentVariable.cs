using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Architecture
{
    public abstract class PersistentVariable
    {

        public static bool HasKey(string name)
        {
#if UNITY_EDITOR
            return EditorPrefs.HasKey(name);
#else
            return PlayerPrefs.HasKey(name);
#endif
        }

        public static float GetFloat(string name)
        {
#if UNITY_EDITOR
            return EditorPrefs.GetFloat(name);
#else
            return PlayerPrefs.GetFloat(name);
#endif
        }

        public static void SetFloat(string name, float value)
        {
#if UNITY_EDITOR
            EditorPrefs.SetFloat(name, value);
#else
            PlayerPrefs.SetFloat(name, value);
#endif
        }

        public static int GetInt(string name)
        {
#if UNITY_EDITOR
            return EditorPrefs.GetInt(name);
#else
            return PlayerPrefs.GetInt(name);
#endif
        }

        public static void SetInt(string name, int value)
        {
#if UNITY_EDITOR
            EditorPrefs.SetInt(name, value);
#else
            PlayerPrefs.SetInt(name, value);
#endif
        }

        public static string GetString(string name)
        {
#if UNITY_EDITOR
            return EditorPrefs.GetString(name);
#else
            return PlayerPrefs.GetString(name);
#endif
        }

        public static void SetString(string name, string value)
        {
#if UNITY_EDITOR
            EditorPrefs.SetString(name, value);
#else
            PlayerPrefs.SetString(name, value);
#endif
        }
    }

    public abstract class PersistentVariable<T> : PersistentVariable
    {
        private bool initialized;
        protected string name;
        protected T defaultValue;
        protected T cachedValue;

        public PersistentVariable(string name, T defaultValue)
        {
            this.name = name;
            this.defaultValue = defaultValue;
            this.cachedValue = defaultValue;
            this.initialized = false;
        }

        public T Value
        {
            get
            {
                if (!initialized)
                {
                    if (HasKey(this.name)) cachedValue = ReadPersistentValue();
                    else cachedValue = this.defaultValue;
                    initialized = true;
                }
                
                return cachedValue;
            }
            set
            {
                cachedValue = value;

                if (!HasKey(this.name))
                {
                    if (!value.Equals(defaultValue))
                    {
                        WritePersistentValue(value);
                    }
                }
                else
                {
                    WritePersistentValue(value);
                }

                initialized = true;
            }
        }

        protected abstract T ReadPersistentValue();
        protected abstract void WritePersistentValue(T value);


    }

    public class PersistentFloat : PersistentVariable<float>
    {
        public PersistentFloat(string name, float defaultValue) : base(name, defaultValue) { }
        protected override float ReadPersistentValue() => GetFloat(this.name);
        protected override void WritePersistentValue(float value) => SetFloat(this.name, value);
    }

    public class PersistentInt : PersistentVariable<int>
    {
        public PersistentInt(string name, int defaultValue) : base(name, defaultValue) { }
        protected override int ReadPersistentValue() => GetInt(this.name);
        protected override void WritePersistentValue(int value) => SetInt(this.name, value);
    }

    public class PersistentString : PersistentVariable<string>
    {
        public PersistentString(string name, string defaultValue) : base(name, defaultValue) { }
        protected override string ReadPersistentValue() => GetString(this.name);
        protected override void WritePersistentValue(string value) => SetString(this.name, value);
    }

    public class PersistentBool : PersistentVariable<bool>
    {
        public PersistentBool(string name, bool defaultValue) : base(name, defaultValue) { }
        protected override bool ReadPersistentValue() => GetInt(this.name) == 1;
        protected override void WritePersistentValue(bool value) => SetInt(this.name, value ? 1 : 0);
        public void Toggle() => Value = !Value;
    }

    public class PersistentInstance<T> : PersistentVariable<T>
    {
        public PersistentInstance(string name, T defaultValue) : base(name, defaultValue) { }
        protected override T ReadPersistentValue() => JsonUtility.FromJson<T>(GetString(this.name));
        protected override void WritePersistentValue(T value) => SetString(this.name, JsonUtility.ToJson(value));
    }
}
