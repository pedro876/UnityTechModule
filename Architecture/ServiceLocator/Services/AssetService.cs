using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Architecture
{
    public class AssetService : IService
    {
        #region Variables

        private Dictionary<string, Resource>[] assets;

        #endregion

        #region Base Methods

        public void Initialize() { }
        public void Dispose() { }
        
        public void SetFolders(int foldersNumber) => assets = new Dictionary<string, Resource>[foldersNumber];

        #endregion

        #region Service Methods

        public IEnumerator CO_LoadAsset<T>(int folder, string key)
        {
            assets[folder] ??= new();

            if (assets[folder].ContainsKey(key)) yield break;

            AsyncOperationHandle op = Addressables.LoadAssetAsync<T>(key);

            if (op.Status == AsyncOperationStatus.Failed)
            {
                Addressables.Release(op);
                yield break;
            }

            assets[folder].Add(key, new(op));
            yield return op.WaitForCompletion();
        }

        public bool TryToInstantiate(int folder, string key, out InstanceReference reference)
        {
            reference = null;

            if (folder >= assets.Length || assets[folder] == null) return false;
            if (!assets[folder].TryGetValue(key, out var resource)) return false;

            reference = resource.CreateInstance(folder, key);

            return reference != null;
        }

        public bool TryToGetAsset<T>(int folder, string key, out T asset)
        {
            asset = default;

            if (folder >= assets.Length || assets[folder] == null) return false;
            if (!assets[folder].TryGetValue(key, out var resource)) return false;

            asset = (T)resource.operation.Result;
            return true;
        }

        public void ReleaseAsset(int folder, string key)
        {
            if (folder >= assets.Length || assets[folder] == null) return;
            if (!assets[folder].TryGetValue(key, out var resource)) return;

            resource.ReleaseAsset();
            assets[folder].Remove(key);
        }

        public void ReleaseInstance(ref InstanceReference reference)
        {
            if (reference.Folder >= assets.Length || assets[reference.Folder] == null) return;
            if (!assets[reference.Folder].TryGetValue(reference.Key, out var resource)) return;

            resource.ReleaseInstance(ref reference);
        }

        public void ReleaseFolder(int folder)
        {
            if (folder >= assets.Length || assets[folder] == null) return;

            foreach (var asset in assets[folder].Values) asset.ReleaseAsset();
            assets[folder].Clear();
        }

        public bool IsAssetLoaded(int folder, string key)
        {
            if (folder >= assets.Length || assets[folder] == null) return false;
            return assets[folder].ContainsKey(key);
        }

        #endregion

        private class Resource
        {
            public Resource(AsyncOperationHandle operation) => this.operation = operation;

            internal AsyncOperationHandle operation;
            private List<InstanceReference> instances;

            internal InstanceReference CreateInstance(int folder, string key)
            {
                if (instances == null && operation.Result is not GameObject) return null;

                instances ??= new();
                instances.Add(new(instances.Count, Addressables.InstantiateAsync(key), folder, key));

                return instances[instances.Count - 1];
            }

            internal void ReleaseInstance(ref InstanceReference reference)
            {
                if (instances == null || reference.Index == -1 || reference.Index >= instances.Count) return;

                //TODO: (Agus/Pedro) Check if must use the DestroyImmediate
                Addressables.ReleaseInstance(reference.Operation);

                if (instances.Count > 1)
                {
                    instances[reference.Index] = instances[instances.Count - 1];
                    instances[reference.Index].Index = reference.Index;
                }

                reference = null;
                instances.RemoveAt(instances.Count - 1);
            }

            internal void ReleaseAsset()
            {
                if (instances != null)
                {
                    for (int i = 0; i < instances.Count; i++)
                    {
                        Addressables.ReleaseInstance(instances[i].Operation);
                        instances[i].Index = -1;
                    }

                    instances = null;
                }

                Addressables.Release(operation);
            }
        }

        public class InstanceReference
        {
            public InstanceReference(int index, AsyncOperationHandle<GameObject> operation, int folder, string key)
            {
                Index = index;
                Operation = operation;
                Folder = folder;
                Key = key;
            }

            public int Index { get; internal set; }
            public AsyncOperationHandle<GameObject> Operation { get; private set; }
            public int Folder { get; private set; }
            public string Key { get; private set; }
        }
    }
}
