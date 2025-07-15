namespace SAGE.Framework.Core.UI
{
    using System;
    //using Sirenix.OdinInspector;
    using System.Collections.Generic;
#if UNITASK_SUPPORT
    using Cysharp.Threading.Tasks;
#else
    using System.Threading.Tasks;
#endif
    //using MEC;
    using Addressable;
    using Extensions;
    using UnityEngine;
    using Logger = Log.Logger;
#if ENABLE_ADDRESS
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;
#endif

    public enum CanvasType
    {
        Main,
        Popup,
        Loading
    }

    public class UIManager : BehaviorSingleton<UIManager>
    {
        //[FoldoutGroup("Loading Handler")] [Required] [SerializeField]
        //private LoadingCircleHandler loadingCircleHandler;

        //[FoldoutGroup("Loading Handler")] [Required] [SerializeField]
        //private LoadingTransitionHandler loadingTransitionHandler;

        //[Title("Canvas")] [FoldoutGroup("Canvas")] [Required] [SerializeField]
        private RectTransform mainCanvas;

        //[FoldoutGroup("Canvas")] [Required] [SerializeField]
        private RectTransform popupCanvas;

        //[FoldoutGroup("Canvas")] [Required] [SerializeField]
        private RectTransform loadingCanvas;

        private Dictionary<string, IBaseScreen> screens;
#if ENABLE_ADDRESS
        private AsyncOperationHandle<IList<GameObject>> screenAssets;
        private List<AsyncOperationHandle<GameObject>> opHandles;
#endif
        private Stack<IBackScreen> backScreens;

        //[SerializeField] private UIRewardCoin rewardVFX;
        [SerializeField] private Camera uiCamera;
        private readonly string screenLabel = "BaseScreen";
        
        public Camera UICamera => uiCamera;

        protected override void Awake()
        {
            base.Awake();
            screens = new Dictionary<string, IBaseScreen>();
            backScreens = new Stack<IBackScreen>();
#if ENABLE_ADDRESS
            opHandles = new List<AsyncOperationHandle<GameObject>>();
            screenAssets = default;
#endif
        }

        #region API

        public IEnumerator<float> IEInitialize()
        {
            return AddressableManager.Instance.IELoadAllAssetByLabel(screenLabel);
        }

        public IEnumerator<float> LoadAllAssetByLabel(string label)
        {
#if ENABLE_ADDRESS
            //check if the label is already loaded
            if (screenAssets.IsValid())
            {
                Logger.Log("Addressable", "Already loaded");
                yield break;
            }

            AsyncOperationHandle downloadHandle = Addressables.DownloadDependenciesAsync(label, false);
            float progress = 0;
            screenAssets = Addressables.LoadAssetsAsync<GameObject>(label, null);
            while (downloadHandle.Status == AsyncOperationStatus.None)
            {
                float percentageComplete = downloadHandle.GetDownloadStatus().Percent;
                if (percentageComplete > progress * 1.1) // Report at most every 10% or so
                {
                    progress = percentageComplete; // More accurate %
                    Logger.Log("Addressable", $"Downloading: {progress}");
                }

                yield return Timing.WaitForOneFrame;
            }
#else
            yield break;
#endif
        }

        public void RegisterBackScreen(IBackScreen screen)
        {
            backScreens.Push(screen);
        }

        public void UnregisterBackScreen(IBackScreen screen)
        {
            if (backScreens.Count > 0 && backScreens.Peek() == screen) backScreens.Pop();
        }

        public void HideAllPreviousScreens()
        {
            while (backScreens.Count > 0)
            {
                backScreens.Pop().BackToScreen();
            }
        }

        public T GetScreen<T>(string screenName) where T : class, IBaseScreen
        {
            if (screens.TryGetValue(screenName, out var screen))
            {
                return screen as T;
            }

            return null;
        }

        public void ShowAndLoadScreen<T>(string screenName, IUIData data = null, CanvasType parent = CanvasType.Main,
            Action<T> callback = null)
            where T : class, IBaseScreen
        {
           // Timing.RunCoroutine(IELoadAndShowScreen<T>(screenName, data, parent, callback));
        }

        public void ShowCircleLoading(float delayShow = 3f)
        {
            //loadingCircleHandler.Show(delayShow);
        }

        public void HideCircleLoading()
        {
            //loadingCircleHandler.Hide();
        }
        
        public void StartCoinAnimation(int coinReward = 10, Action onComplete = null)
        {
            //rewardVFX.StartCoinAnimation(coinReward, onComplete);
        }

        public void FadeInLoading(Action onComplete = null)
        {
            //loadingTransitionHandler.FadeIn(onComplete);
        }

        public void FadeOutLoading(Action onComplete = null)
        {
            //loadingTransitionHandler.FadeOut(onComplete);
        }

#if UNITASK_SUPPORT
        public async UniTask FadeInLoadingAsync()
        {
            await loadingTransitionHandler.FadeInAsync();
        }

        public async UniTask FadeOutLoadingAsync()
        {
            await loadingTransitionHandler.FadeOutAsync();
        }
#else
        public async Task FadeInLoadingAsync()
        {
           // await loadingTransitionHandler.FadeInAsync();
        }

        public async Task FadeOutLoadingAsync()
        {
            //await loadingTransitionHandler.FadeOutAsync();
        }
#endif

        #endregion

/*#if UNITY_ANDROID
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (backScreens.Count > 0)
                {
                    backScreens.Pop().BackToScreen();
                }
            }
        }
#endif*/

        private RectTransform GetParent(CanvasType type)
        {
            switch (type)
            {
                case CanvasType.Main:
                    return mainCanvas;
                case CanvasType.Popup:
                    return popupCanvas;
                case CanvasType.Loading:
                    return loadingCanvas;
                default:
                    return mainCanvas;
            }
        }

        private IEnumerator<float> IELoadAndShowScreen<T>(string screenName, IUIData data = null,
            CanvasType parent = CanvasType.Main,
            Action<T> callback = null)
            where T : class, IBaseScreen
        {
            IBaseScreen baseScreen = null;
            ShowCircleLoading();

            if (!screens.TryGetValue(screenName, out var screen))
            {
#if ENABLE_ADDRESS
                AsyncOperationHandle<GameObject> opHandle =
                    Addressables.LoadAssetAsync<GameObject>(BaseScreenAddress.GetName(screenName));
                float time = Time.realtimeSinceStartup;
                yield return Timing.WaitUntilTrue(() => opHandle.Status == AsyncOperationStatus.Succeeded);
                if (opHandle.Result == null)
                {
                    Logger.LogError("Addressable", $"Failed to load screen: {screenName}");
                    HideCircleLoading();
                    yield break;
                }

                Logger.Log("Addressable", $"Load time: {Time.realtimeSinceStartup - time}]");
                GameObject go = Instantiate(opHandle.Result, GetParent(parent));
                Logger.Log("Addressable", $"Instantiated time: {Time.realtimeSinceStartup - time}");
                baseScreen = go.GetComponent<BaseScreen>();
                screens.Add(screenName, baseScreen);
                screens[screenName].Initialize();
                opHandles.Add(opHandle);
#else
                yield break;
#endif
            }
            else
            {
                baseScreen = (BaseScreen)screen;
            }

            baseScreen.RectTransform.SetParent(GetParent(parent));
            baseScreen.RectTransform.anchoredPosition = Vector2.zero;
            baseScreen.RectTransform.sizeDelta = Vector2.zero;
            baseScreen.RectTransform.localScale = Vector3.one;
            baseScreen.RectTransform.SetAsLastSibling();
            if (data != null) baseScreen.SetData(data);
            baseScreen.Show();

            callback?.Invoke(baseScreen as T);
            HideCircleLoading();
        }

        #region Async

#if UNITASK_SUPPORT
        public async UniTask DoInitializeAsync()
        {
            await AddressableManager.Instance.LoadAllAssetByLabelAsync(screenLabel);
        }

        public async UniTask ShowAndLoadScreenAsync<T>(string screenName, IUIData data = null,
            CanvasType parent = CanvasType.Main,
            Action<T> callback = null)
            where T : class, IBaseScreen
        {
            await IELoadAndShowScreenAsync<T>(screenName, data, parent, callback);
        }

        private async UniTask IELoadAndShowScreenAsync<T>(string screenName, IUIData data = null,
            CanvasType parent = CanvasType.Main,
            Action<T> callback = null)
            where T : class, IBaseScreen
        {
            IBaseScreen baseScreen = null;
            ShowCircleLoading();

            if (!screens.TryGetValue(screenName, out var screen))
            {
#if ENABLE_ADDRESS
                AsyncOperationHandle<GameObject> opHandle =
                    Addressables.LoadAssetAsync<GameObject>(BaseScreenAddress.GetName(screenName));

                float time = Time.realtimeSinceStartup;
                await opHandle.Task;
                if (opHandle.Result == null)
                {
                    HideCircleLoading();
                    Logger.LogError("Addressable", $"Failed to load screen: {screenName}");
                    return;
                }

                Logger.Log("Addressable", $"Load time: {Time.realtimeSinceStartup - time}]");
                GameObject go = Instantiate(opHandle.Result, GetParent(parent));
                Logger.Log("Addressable", $"Instantiated time: {Time.realtimeSinceStartup - time}");
                baseScreen = go.GetComponent<BaseScreen>();
                screens.Add(screenName, baseScreen);
                screens[screenName].Initialize();
                opHandles.Add(opHandle);
#else
                await UniTask.Yield();
#endif
            }
            else
            {
                baseScreen = (BaseScreen)screen;
            }

            baseScreen.RectTransform.SetParent(GetParent(parent));
            baseScreen.RectTransform.anchoredPosition = Vector2.zero;
            baseScreen.RectTransform.sizeDelta = Vector2.zero;
            baseScreen.RectTransform.localScale = Vector3.one;
            baseScreen.RectTransform.SetAsLastSibling();
            baseScreen.SetData(data);
            baseScreen.Show();
            await UniTask.Yield();
            callback?.Invoke(baseScreen as T);
            HideCircleLoading();
        }
#else
        public async Task DoInitializeAsync()
        {
            await AddressableManager.Instance.LoadAllAssetByLabelAsync(screenLabel);
        }

        public async Task ShowAndLoadScreenAsync<T>(string screenName, IUIData data = null,
            CanvasType parent = CanvasType.Main,
            Action<T> callback = null)
            where T : class, IBaseScreen
        {
            await IELoadAndShowScreenAsync<T>(screenName, parent, data, callback);
        }

        private async Task IELoadAndShowScreenAsync<T>(string screenName, CanvasType parent, IUIData data = null,
            Action<T> callback = null)
            where T : class, IBaseScreen
        {
            IBaseScreen baseScreen = null;
            ShowCircleLoading();
            if (!screens.TryGetValue(screenName, out var screen))
            {
#if ENABLE_ADDRESS
                AsyncOperationHandle<GameObject> opHandle =
                    Addressables.LoadAssetAsync<GameObject>(BaseScreenAddress.GetName(screenName));

                float time = Time.realtimeSinceStartup;
                await opHandle.Task;
                if (opHandle.Result == null)
                {
                    Logger.LogError("Addressable", $"Failed to load screen: {screenName}");
                    HideCircleLoading();
                    return;
                }

                Logger.Log("Addressable", $"Load time: {Time.realtimeSinceStartup - time}]");
                GameObject go = Instantiate(opHandle.Result, GetParent(parent));
                Logger.Log("Addressable", $"Instantiated time: {Time.realtimeSinceStartup - time}");
                baseScreen = go.GetComponent<BaseScreen>();
                screens.Add(screenName, baseScreen);
                screens[screenName].Initialize();
                opHandles.Add(opHandle);
#else
                await Task.Yield();
#endif
            }
            else
            {
                baseScreen = (BaseScreen)screen;
            }

            baseScreen.RectTransform.SetParent(GetParent(parent));
            baseScreen.RectTransform.anchoredPosition = Vector2.zero;
            baseScreen.RectTransform.sizeDelta = Vector2.zero;
            baseScreen.RectTransform.localScale = Vector3.one;
            baseScreen.RectTransform.SetAsLastSibling();
            baseScreen.SetData(data);
            baseScreen.Show();

            callback?.Invoke(baseScreen as T);
            HideCircleLoading();
        }
#endif

        #endregion

        private void OnDestroy()
        {
#if ENABLE_ADDRESS
            foreach (var opHandle in opHandles)
            {
                Addressables.Release(opHandle);
            }

            if (screenAssets.IsValid()) Addressables.Release(screenAssets);
#endif
        }
    }
}