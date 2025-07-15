namespace SAGE.Framework.Core.Addressable
{
    using System.Collections.Generic;
#if UNITASK_SUPPORT
    using Cysharp.Threading.Tasks;
#else
    using System.Threading.Tasks;
#endif
    //using MEC;
    using SAGE.Framework.Core.Log;
    using UnityEngine;
#if ENABLE_ADDRESS
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;
#endif
    using Logger = Log.Logger;
    using Extensions;

    public class AddressableManager : Singleton<AddressableManager>
    {
#if ENABLE_ADDRESS
        protected Dictionary<string, AsyncOperationHandle<IList<GameObject>>> cachedAssets;
#endif

        //public void LoadAllAssetByLabel(string label) => Timing.RunCoroutine(IELoadAllAssetByLabel(label));

        public IEnumerator<float> IELoadAllAssetByLabel(string label)
        {
#if ENABLE_ADDRESS
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
#else
            yield return 0;
#endif

        }

#if UNITASK_SUPPORT
        public async UniTask LoadAllAssetByLabelAsync(string label)
        {
#if ENABLE_ADDRESS
            /*cachedAssets ??= new Dictionary<string, AsyncOperationHandle<IList<GameObject>>>();

            if (cachedAssets.ContainsKey(label))
            {
                if (cachedAssets[label].IsValid())
                {
                    Debug.Log($"<b><color=green>[Addressable]</color></b> is already loaded: {label}");
                    return;
                }
            }*/

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
            //cachedAssets.Add(label, assets);
            
            //release the download handle if you don't need it anymore
#else
            await UniTask.Yield();
#endif
        }
#else
        public async Task LoadAllAssetByLabelAsync(string label)
        {
#if ENABLE_ADDRESS
            cachedAssets ??= new Dictionary<string, AsyncOperationHandle<IList<GameObject>>>();

            if (cachedAssets.ContainsKey(label))
            {
                if (cachedAssets[label].IsValid())
                {
                    Logger.Log("Addressable", $"is already loaded: {label}");
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

                await Task.Yield();
            }
            cachedAssets.Add(label, assets);
#else
            await Task.Yield();
#endif
        }
#endif

        public void UnloadAllAssetByLabel(string label)
        {
#if ENABLE_ADDRESS
            if (cachedAssets.ContainsKey(label))
            {
                Addressables.Release(cachedAssets[label]);
                cachedAssets.Remove(label);
            }
#endif
        }

        public void UnloadAllAssets()
        {
#if ENABLE_ADDRESS
            foreach (KeyValuePair<string, AsyncOperationHandle<IList<GameObject>>> asset in cachedAssets)
            {
                Addressables.Release(asset.Value);
            }

            cachedAssets.Clear();
#endif
        }

        private bool IsAssetLoaded(string label)
        {
#if ENABLE_ADDRESS
            return cachedAssets.ContainsKey(label) && cachedAssets[label].IsValid();
#else
            return false;
#endif
        }
    }
}