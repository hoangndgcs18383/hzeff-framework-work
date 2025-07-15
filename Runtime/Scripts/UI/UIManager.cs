namespace SAGE.Framework.UI
{
    using Cysharp.Threading.Tasks;
    using Core;
    using Core.Addressable;
    using System;
    using Sirenix.OdinInspector;
    using System.Collections.Generic;
    using Logger = Core.Logger;
    //using MEC;
    using Extensions;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;

    public enum CanvasType
    {
        Main,
        Popup,
        Loading
    }

    public class UIManager : BehaviorSingleton<UIManager>
    {
        [FoldoutGroup("Loading Handler")] [Required] [SerializeField]
        private LoadingCircleHandler loadingCircleHandler;

        //[FoldoutGroup("Loading Handler")] [Required] [SerializeField]
        //private LoadingTransitionHandler loadingTransitionHandler;

        //[Title("Canvas")] [FoldoutGroup("Canvas")] [Required] [SerializeField]
        private RectTransform mainCanvas;

        //[FoldoutGroup("Canvas")] [Required] [SerializeField]
        private RectTransform popupCanvas;

        //[FoldoutGroup("Canvas")] [Required] [SerializeField]
        private RectTransform loadingCanvas;

        private Dictionary<string, IBaseScreen> screens;

        private AsyncOperationHandle<IList<GameObject>> screenAssets;
        private List<AsyncOperationHandle<GameObject>> opHandles;
        
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
            opHandles = new List<AsyncOperationHandle<GameObject>>();
            screenAssets = default;
        }

        #region API
        
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

        public void ShowCircleLoading(float delayShow = 3f)
        {
            loadingCircleHandler.Show(delayShow);
        }

        public void HideCircleLoading()
        {
            loadingCircleHandler.Hide();
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
        
        public async UniTask FadeInLoadingAsync()
        {
            //await loadingTransitionHandler.FadeInAsync();
        }

        public async UniTask FadeOutLoadingAsync()
        {
            //await loadingTransitionHandler.FadeOutAsync();
        }


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

                await UniTask.Yield();

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

        private void OnDestroy()
        {
            foreach (var opHandle in opHandles)
            {
                Addressables.Release(opHandle);
            }

            if (screenAssets.IsValid()) Addressables.Release(screenAssets);
        }
    }
}