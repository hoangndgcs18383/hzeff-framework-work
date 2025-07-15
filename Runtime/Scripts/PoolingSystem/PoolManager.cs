namespace SAGE.Framework.Core
{
    using SAGE.Framework.Extensions;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;


    public class PoolManager : BehaviorSingleton<PoolManager>
    {
        #region Singleton

        protected override void Awake()
        {
            base.Awake();
            InitializeAllPools();
        }

        #endregion

        [Serializable]
        public class PoolConfig
        {
            public string poolTag;

            public AssetReferenceGameObject prefabReference;

            public int initialSize = 10;
            public int maxSize = 100;
            public float growthFactor = 0.2f;
            public bool autoScale = true;
            public bool asyncLoading = true;
            public List<PoolConfig> childPools = new();
        }

        [SerializeField] private List<PoolConfig> _poolConfigs = new();

        private Dictionary<string, Pool> _pools = new();
        private Dictionary<string, PoolConfig> _configLookup = new();

        private void InitializeAllPools()
        {
            foreach (var config in _poolConfigs)
            {
                _configLookup[config.poolTag] = config;
                CreatePool(config);
            }
        }

        private void CreatePool(PoolConfig config)
        {
            if (_pools.ContainsKey(config.poolTag))
            {
                Debug.LogWarning($"Pool {config.poolTag} already exists!");
                return;
            }

            var newPool = new Pool(
                config.poolTag,
                config.prefabReference,
                config.initialSize,
                config.maxSize,
                config.growthFactor,
                config.autoScale,
                config.asyncLoading
            );

            _pools[config.poolTag] = newPool;

            // Initialize child pools
            foreach (var childConfig in config.childPools)
            {
                CreatePool(childConfig);
            }
        }

        public GameObject Spawn(string poolTag, Vector3 position, Quaternion rotation)
        {
            if (!_pools.ContainsKey(poolTag))
            {
                Debug.LogError($"Pool {poolTag} not found!");
                return null;
            }

            return _pools[poolTag].Spawn(position, rotation);
        }

        public void Return(GameObject obj)
        {
            var pooledObj = obj.GetComponent<PooledObject>();
            if (pooledObj == null)
            {
                Debug.LogError("Object is not poolable!");
                return;
            }

            if (_pools.TryGetValue(pooledObj.PoolTag, out var pool))
            {
                pool.Return(obj);
            }
            else
            {
                Debug.LogError($"Pool {pooledObj.PoolTag} not found for object {obj.name}");
            }
        }

        public class Pool
        {
            private Queue<GameObject> _inactive = new();
            private List<GameObject> _active = new();

            private AssetReferenceGameObject _prefabRef;

            private GameObject _loadedPrefab;
            private string _poolTag;
            private int _maxSize;
            private float _growthFactor;
            private bool _autoScale;
            private bool _asyncLoad;

            public Pool(
                string poolTag,
                AssetReferenceGameObject prefabRef,
                int initialSize,
                int maxSize,
                float growthFactor,
                bool autoScale,
                bool asyncLoad)
            {
                _poolTag = poolTag;

                _prefabRef = prefabRef;

                _maxSize = maxSize;
                _growthFactor = growthFactor;
                _autoScale = autoScale;
                _asyncLoad = asyncLoad;

                if (asyncLoad)
                {
                    Instance.StartCoroutine(AsyncInitialize(initialSize));
                }
                else
                {
                    SyncInitialize(initialSize);
                }
            }

            private IEnumerator AsyncInitialize(int initialSize)
            {
                var loadHandle = _prefabRef.LoadAssetAsync();
                yield return loadHandle;

                if (loadHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    _loadedPrefab = loadHandle.Result;
                    Instance.StartCoroutine(CreateObjectsAsync(initialSize));
                }
                else
                {
                    Debug.LogError($"Failed to load {_prefabRef.RuntimeKey}");
                }
            }

            private IEnumerator CreateObjectsAsync(int count)
            {
                for (int i = 0; i < count; i++)
                {
                    if (_inactive.Count >= _maxSize) break;

                    var obj = Instantiate(_loadedPrefab);
                    obj.SetActive(false);
                    var pooledObj = obj.GetOrAddComponent<PooledObject>();
                    pooledObj.PoolTag = _poolTag;
                    _inactive.Enqueue(obj);

                    if (i % 5 == 0) yield return null;
                }
            }

            private void SyncInitialize(int initialSize)
            {
                _loadedPrefab = _prefabRef.Asset as GameObject;
                for (int i = 0; i < initialSize; i++)
                {
                    var obj = Instantiate(_loadedPrefab);
                    obj.SetActive(false);
                    obj.GetOrAddComponent<PooledObject>().PoolTag = _poolTag;
                    _inactive.Enqueue(obj);
                }
            }

            public GameObject Spawn(Vector3 position, Quaternion rotation)
            {
                if (_inactive.Count == 0)
                {
                    if (!_autoScale || !CanExpand()) return null;
                    ExpandPool();
                }

                var obj = _inactive.Dequeue();
                _active.Add(obj);

                obj.transform.SetPositionAndRotation(position, rotation);
                obj.SetActive(true);
                obj.GetComponent<IPoolable>()?.OnSpawn();

                return obj;
            }

            private bool CanExpand() => _inactive.Count + _active.Count < _maxSize;

            private void ExpandPool()
            {
                int growthAmount = Mathf.CeilToInt(_active.Count * _growthFactor);
                growthAmount = Mathf.Min(growthAmount, _maxSize - (_active.Count + _inactive.Count));

                for (int i = 0; i < growthAmount; i++)
                {
                    var obj = Instantiate(_loadedPrefab);
                    obj.SetActive(false);
                    _inactive.Enqueue(obj);
                }
            }

            public void Return(GameObject obj)
            {
                obj.GetComponent<IPoolable>()?.OnDespawn();
                obj.SetActive(false);
                _active.Remove(obj);
                _inactive.Enqueue(obj);
            }
        }
    }
}