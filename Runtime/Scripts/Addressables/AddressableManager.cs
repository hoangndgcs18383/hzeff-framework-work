namespace SAGE.Framework.Core
{
    using SAGE.Framework.Extensions;
    using System.Collections.Generic;
    using MEC;
    using Cysharp.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;


    public class AddressableManager : Singleton<AddressableManager>
    {
        protected Dictionary<string, AsyncOperationHandle<IList<GameObject>>> cachedAssets;

        public void LoadAllAssetByLabel(string label) => Timing.RunCoroutine(IELoadAllAssetByLabel(label));

        public IEnumerator<float> IELoadAllAssetByLabel(string label)
        {
            cachedAssets ??= new Dictionary<string, AsyncOperationHandle<IList<GameObject>>>();

            if (cachedAssets.ContainsKey(label))
            {
                if (cachedAssets[label].IsValid())
                {
                    Logger.Log("Addressable", $"is already loaded: {label}");
                    yield break;
                }
            }

            AsyncOperationHandle downloadHandle = Addressables.DownloadDependenciesAsync(label, false);
            float progress = 0;
            AsyncOperationHandle<IList<GameObject>> assets = Addressables.LoadAssetsAsync<GameObject>(label, null);
            while (downloadHandle.Status == AsyncOperationStatus.None)
            {
                float percentageComplete = downloadHandle.GetDownloadStatus().Percent;
                if (percentageComplete > progress * 1.1) // Report at most every 10% or so
                {
                    progress = percentageComplete; // More accurate %
                    Logger.Log("Addressable", $"Downloading: {progress * 100}%");
                }

                yield return Timing.WaitForOneFrame;
            }

            cachedAssets.Add(label, assets);
        }

        public async UniTask LoadAllAssetByLabelAsync(string label)
        {
            cachedAssets ??= new Dictionary<string, AsyncOperationHandle<IList<GameObject>>>();

            if (cachedAssets.ContainsKey(label))
            {
                if (cachedAssets[label].IsValid())
                {
                    Debug.Log($"<b><color=green>[Addressable]</color></b> is already loaded: {label}");
                    return;
                }
            }

            AsyncOperationHandle downloadHandle = Addressables.DownloadDependenciesAsync(label, false);
            float progress = 0;
            AsyncOperationHandle<IList<GameObject>> assets = Addressables.LoadAssetsAsync<GameObject>(label, null);
            while (downloadHandle.Status == AsyncOperationStatus.None)
            {
                float percentageComplete = downloadHandle.GetDownloadStatus().Percent;
                if (percentageComplete > progress * 1.1) // Report at most every 10% or so
                {
                    progress = percentageComplete; // More accurate %
                    Logger.Log("Addressable", $"Downloading: {progress * 100}%");
                }

                await UniTask.Yield();
            }

            cachedAssets.Add(label, assets);

            await UniTask.Yield();
        }

        public void UnloadAllAssetByLabel(string label)
        {
            if (cachedAssets.ContainsKey(label))
            {
                Addressables.Release(cachedAssets[label]);
                cachedAssets.Remove(label);
            }
        }

        public void UnloadAllAssets()
        {
            foreach (KeyValuePair<string, AsyncOperationHandle<IList<GameObject>>> asset in cachedAssets)
            {
                Addressables.Release(asset.Value);
            }

            cachedAssets.Clear();
        }

        private bool IsAssetLoaded(string label)
        {
            return cachedAssets.ContainsKey(label) && cachedAssets[label].IsValid();
        }
    }
}