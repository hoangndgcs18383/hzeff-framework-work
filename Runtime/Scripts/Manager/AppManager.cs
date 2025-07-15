/*using System;
using Cysharp.Threading.Tasks;
using GoodsTripleSort;
using PlayAd.SDK.Ads;
using SAGE.Framework.Core.Addressable;
using SAGE.Framework.Core.Extensions;
using SAGE.Framework.Core.UI;
using SAGE.Framework.SDK;
using UnityEngine.Profiling;

namespace SAGE.Framework.Core
{
    using UnityEngine;

    public class AppManager : BehaviorSingleton<AppManager>
    {
        [SerializeField] private int _targetFrameRate = 60;
        [SerializeField] private UILoading _uiLoading;

        private float _fillAmount = 0f;
        private bool _isLoading = false;

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                if (_isLoading)
                {
                    _uiLoading.Show();
                }
                else
                {
                    _uiLoading.Hide();
                }
            }
        }

        public async void Start()
        {
            await DOInitialize().AttachExternalCancellation(this.GetCancellationTokenOnDestroy());
        }

        public async UniTask DOInitialize()
        {
#if !UNITY_EDITOR
            Application.targetFrameRate = _targetFrameRate;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
#else
            Application.targetFrameRate = 300;
#endif
            DeviceBasedPipelineManager.Instance.ApplyOptimalPipeline();
            IsLoading = true;
            await UIManager.Instance.DoInitializeAsync();
            await ModelManager.Instance.InitializeAsync();
            await SDKHandler.Instance.LoginAsync();
            _fillAmount = 0.2f;
            await _uiLoading.SetProgressTweenAsync(0.2f);
#if !UNITY_EDITOR
            await ShowOpenAppAsync();
#endif
            await _uiLoading.SetProgressTweenAsync(1f);
            await UIManager.Instance.FadeInLoadingAsync();
            AudioManager.Instance.PlayBackgroundMusic(SoundKey.MainMenu);
            await UIManager.Instance.ShowAndLoadScreenAsync<UIMainMenu>(BaseScreenAddress.UIMAINMENU);
            IsLoading = false;
            await UniTask.WaitForSeconds(0.1f);
            SDKHandler.Instance.ShowBannerAdAsync();
        }
        

        public async UniTask ShowOpenAppAsync()
        {
            float delay = 0;
            if (PlayAdSupport.IsLoggeds && !PlayAdSupport.GetUser().BuyNoAds)
            {
                delay = PlayAdSupport.GetUser().GetConfig<int>(GameConfigName.DelayLoading);
                float fill = 0;
                float cachedDelay = delay;

                while (delay > 0)
                {
                    delay -= Time.deltaTime;
                    fill = Mathf.Clamp01((cachedDelay - delay) / cachedDelay);
                    if (PlayAdSupport.IsAppOpenAdAvailable()) break;

                    _uiLoading.SetProgressTween(_fillAmount + fill);
                    await UniTask.Yield();
                }
            }
            
            if (!PlayAdSupport.GetUser().BuyNoAds) PlayAdSupport.ShowOpenAppAd();
        }

        public async UniTask LoadingScreen<T>(string baseScreen, float duration = 3f) where T : BaseScreen
        {
            UIManager.Instance.HideAllPreviousScreens();
            IsLoading = true;
            SDKHandler.Instance.ShowBannerAdAsync(AdBannerSize.MediumRectangle);
            _uiLoading.SetProgress(0);
            await _uiLoading.SetProgressTweenAsync(0.1f, 0.15f);
            //Debug.Log($"Before Total Memory: {GC.GetTotalMemory(false) / 1024 / 1024}MB");
            //Debug.Log($"Before Texture Memory: {Profiler.GetTotalAllocatedMemoryLong() / 1024 / 1024}MB");
            //GC.Collect();
            await UniTask.WaitForSeconds(0.15f);
            //await Resources.UnloadUnusedAssets();
            //Debug.Log($"After Total Memory: {GC.GetTotalMemory(false) / 1024 / 1024}MB");
            //Debug.Log($"After Texture Memory: {Profiler.GetTotalAllocatedMemoryLong() / 1024 / 1024}MB");
            await _uiLoading.SetProgressTweenAsync(1f, duration);
            await UIManager.Instance.FadeInLoadingAsync();
            IsLoading = false;
            SDKHandler.Instance.ShowBannerAdAsync();
            await UIManager.Instance.ShowAndLoadScreenAsync<T>(baseScreen);
        }
        

        private void OnApplicationQuit()
        {
            UserProfileService.Sync();
        }
    }
}*/