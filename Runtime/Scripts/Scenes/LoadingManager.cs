namespace SAGE.Framework.Core
{
    using SAGE.Framework.Extensions;
    using System.Collections.Generic;
    using Sirenix.OdinInspector;
    using MEC;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;
    using UnityEngine.ResourceManagement.ResourceProviders;
    using UnityEngine.SceneManagement;
    using Log = Logger;
    
    public class LoadingManager : BehaviorSingleton<LoadingManager>
    {
        [SerializeField] private List<string> sceneAddresses;

        private Dictionary<string, AsyncOperationHandle<SceneInstance>> _preloadHandles;

        private CoroutineHandle _loadingCoroutine;

        private void Start()
        {
            _preloadHandles = new Dictionary<string, AsyncOperationHandle<SceneInstance>>();
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
                if (!_preloadHandles.ContainsKey(address))
                {
                    var handle = Addressables.LoadSceneAsync(address, LoadSceneMode.Single, false);
                    handle.Completed += (operation) => OnScenePreloaded(address, operation);
                    _preloadHandles[address] = handle;
                }
            }
        }


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


        [Button]
        public void ActivatePreloadedScene(string address)
        {
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
        }

        [Button]
        public void PreloadSceneByLabel()
        {
            //LoadScene("Menu");
        }
        
        void OnDestroy()
        {
            foreach (var handle in _preloadHandles.Values)
            {
                if (handle.IsValid())
                {
                    Addressables.Release(handle);
                }
            }

            if (Timing.IsRunning(_loadingCoroutine))
            {
                Timing.KillCoroutines(_loadingCoroutine);
            }
        }
    }
}