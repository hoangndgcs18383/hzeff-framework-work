/*namespace SAGE.Framework.Core.Log
{
    using System.Collections.Generic;
    using Sirenix.OdinInspector;
    using MEC;
    using Extensions;
    using UnityEngine;

#if ENABLE_ADDRESS
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;
    using UnityEngine.ResourceManagement.ResourceProviders;
#endif
    using UnityEngine.SceneManagement;

    public class LoadingManager : BehaviorSingleton<LoadingManager>
    {
        [SerializeField] private List<string> sceneAddresses;
#if ENABLE_ADDRESS
        private Dictionary<string, AsyncOperationHandle<SceneInstance>> _preloadHandles;
#endif
        private CoroutineHandle _loadingCoroutine;

        private void Start()
        {
#if ENABLE_ADDRESS
            _preloadHandles = new Dictionary<string, AsyncOperationHandle<SceneInstance>>();
#endif

            PreloadScenes(sceneAddresses);
        }

        public void PreloadScenes(List<string> addresses)
        {
            if (addresses == null || addresses.Count == 0)
            {
                Logger.LogError("LoadingManager", "No scene addresses provided!");
                return;
            }

            foreach (var address in addresses)
            {
#if ENABLE_ADDRESS
                if (!_preloadHandles.ContainsKey(address))
                {
                    var handle = Addressables.LoadSceneAsync(address, LoadSceneMode.Single, false);
                    handle.Completed += (operation) => OnScenePreloaded(address, operation);
                    _preloadHandles[address] = handle;
                }
#endif
            }
        }

#if ENABLE_ADDRESS
        private void OnScenePreloaded(string address, AsyncOperationHandle<SceneInstance> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                Logger.Log("LoadingManager", $"Scene preloaded successfully: {address}");
            }
            else
            {
                Logger.LogError("LoadingManager", $"Failed to preload scene: {address}");
            }
        }
#endif

        [Button]
        public void ActivatePreloadedScene(string address)
        {
#if ENABLE_ADDRESS
            if (_preloadHandles.TryGetValue(address, out var handle) && handle.Status == AsyncOperationStatus.Succeeded)
            {
                // Activate the preloaded scene
                handle.Result.ActivateAsync();
                _preloadHandles.Remove(address);
                PreloadScenes(sceneAddresses);
                Logger.Log("LoadingManager", $"Scene activated successfully: {address}");
            }
            else
            {
                Logger.LogError("LoadingManager", $"Failed to activate scene: {address}");
            }
#endif
        }

        [Button]
        public void PreloadSceneByLabel()
        {
            //LoadScene("Menu");
        }

        /*public void LoadScene(string sceneAddress) =>
            _loadingCoroutine = Timing.RunCoroutine(LoadSceneAsync(sceneAddress));
            #1#

        /*IEnumerator<float> LoadSceneAsync(string sceneAddress)
        {
            _sceneLoadHandle = Addressables.LoadSceneAsync(sceneAddress, LoadSceneMode.Single, false);

            float time = Time.timeSinceLevelLoad;
            while (!_sceneLoadHandle.IsDone)
            {
                float progress = _sceneLoadHandle.PercentComplete;
                Logger.Log("LoadingManager", $"Loading: {progress * 100}%");
                yield return Timing.WaitForOneFrame;
            }

            if (_sceneLoadHandle.Status == AsyncOperationStatus.Succeeded)
            {
                _sceneLoadHandle.Result.ActivateAsync();
                Logger.Log("LoadingManager", $"Scene loaded in {Time.timeSinceLevelLoad - time} seconds");
            }
            else
            {
                Debug.LogError("Failed to load scene: " + sceneAddress);
            }
        }#1#

        void OnDestroy()
        {
#if ENABLE_ADDRESS
            foreach (var handle in _preloadHandles.Values)
            {
                if (handle.IsValid())
                {
                    Addressables.Release(handle);
                }
            }
#endif

            if (Timing.IsRunning(_loadingCoroutine))
            {
                Timing.KillCoroutines(_loadingCoroutine);
            }
        }
    }
}*/