namespace SAGE.Framework.SDK
{
    using Extensions;
    using UI;
    using Core;
    using Logger = Core.Logger;

#if PLAY_ADS
    using PlayAd.SDK.Leaderboard;
    using PlayAd.SDK.Ads.Fyber;
    using PlayAd.SDK.Ads;
#endif
    using System;
    using Cysharp.Threading.Tasks;
    using System.Collections.Generic;
    using UnityEngine;


    public class SDKHandler : Singleton<SDKHandler>
    {
        public event Action OnLoginSuccess = delegate { };
        public event Action OnBeginRequest = delegate { };
        public event Action OnEndRequest = delegate { };

        private bool _isReconnecting = false;
        private InternetCheck _internetCheck;

        public const string RemoveAdsPackage = "game.swg.swingo.remove_ads";
        public const string GEM_SHOP = "ad_reward";
        public const string COIN_SHOP = "coin_ads";
        public const string DAILY_REWARD = "daily_reward_ads";
        public const string REVIVE = "revive_ads";
        public const string UPDATE_SKILL = "update_skill_ads";
        public const string UNLOCK_SKIN = "unlock_skin_ads";
        public const string BONUS_COIN = "bonus_reward_ads";
        public const string END_GAME = "endgame_ads";
        public const string PRESS_SHOP = "open_shop";
        public const string IAP = "in_app_purchase";
        public const string PASS_LEVEL = "pass_level";

        public bool IsBuyNoAds
        {
#if UNITY_EDITOR
            get => true;
#else
            get { return PlayAdSupport.GetUser().BuyNoAds; }
#endif
        }

        public bool IsNewBieRewardAvailable
        {
#if UNITY_EDITOR
            get => true;
#else
            get => !PlayAdSupport.GetUser().HasGotFistRewardAd &&
                   PlayAdSupport.GetUser().FirstRewardAdStatus &&
                   PlayAdSupport.IsLoggeds;
#endif
        }

        public override void Initialize()
        {
            OnBeginRequest += () => UIManager.Instance.ShowCircleLoading(0);
            OnEndRequest += () => UIManager.Instance.HideCircleLoading();
        }

        public async void Reconnect()
        {
            if (_isReconnecting)
                return;
            _isReconnecting = true;

            Debug.Log("Reconnected");
            await LoginAsync(true);
#if PLAY_ADS
            _isReconnecting = PlayAdSupport.IsLoggeds;
            if (PlayAdSupport.IsLoggeds) _internetCheck.StopPing();
#endif
            Logger.Log("Reconnected", "Reconnecting to the server...");
        }

        public void InitializeIAP()
        {
#if PLAY_ADS
            List<IAPInitializeBuilder> initializeBuilders = new
                List<IAPInitializeBuilder>
                {
                    new IAPInitializeBuilder
                    {
                        id = RemoveAdsPackage,
                        productType = ProductType.Consumable,
                    }
                };

            PlayAdSupport.IAP.Initialize(initializeBuilders);
#endif
        }

        public void Track(string key, string paramName, object paramValue)
        {
#if PLAY_ADS
            switch (paramValue)
            {
                case string value:
                    PlayAdSupport.Analytics.LogEvent(key, paramName, value);
                    break;
                case double stringValue:
                    PlayAdSupport.Analytics.LogEvent(key, paramName, stringValue);
                    break;
                case long longValue:
                    PlayAdSupport.Analytics.LogEvent(key, paramName, longValue);
                    break;
                case int intValue:
                    PlayAdSupport.Analytics.LogEvent(key, paramName, intValue);
                    break;
            }
#endif
        }

        public async UniTask LoginAsync(bool isReconnect = false)
        {
#if PLAY_ADS
            var result = await PlayAdSupport.LoginAsync();
            if (result.isSuccess)
            {
                PlayAdFyberInit.Instance.Init();
                PlayAdSupport.InitTapJoy();
                InitializeIAP();
                OnLoginSuccess.Invoke();
                Logger.Log("Login", "Login successful");
                if (!isReconnect)
                {
                    ShowBannerAdAsync(AdBannerSize.MediumRectangle);
                }
                else
                {
                    ShowBannerAdAsync();
                }

                UserProfileService.LoadUserProfile();
                PlayadLeaderboard.Init(UserProfileService.GetDisplayName(), UserProfileService.GetBestScore());
                PlayAdSupport.GetUser().onChangeMoney += OnGameChanged;

                void OnGameChanged(int gem)
                {
                    if (gem > UserProfileService.GetUserProfile().Gem)
                    {
                        /*UIManager.Instance.ShowAndLoadScreen<UIReward>(BaseScreenAddress.UIREWARD, new UIRewardData
                        {
                            Gems = gem - UserProfileService.GetUserProfile().Gem,
                        }, CanvasType.Loading);*/
                        FlyTextManager.Instance.ShowFlyText(
                            $"You have received {gem - UserProfileService.GetUserProfile().Gem} gems",
                            Color.green);

                        UserProfileService.GetUserProfile().Gem = gem;
                        UserProfileService.LocalSync();
                    }
                }

                PlayAdSupport.RegisterORewardInterstitialAdEvent(null, b =>
                {
                    if (b)
                    {
                        OnComplete();
                    }
                    else
                    {
                        if (!IsInternetAvailable()) return;

                        if (!PlayAdSupport.IsRewardedVideoAvailable)
                        {
                            FlyTextManager.Instance.ShowFlyText(Constants.NoRewardAd, Color.red);
                        }
                    }
                });
            }
            else
            {
                if (!_internetCheck && Application.internetReachability == NetworkReachability.NotReachable)
                {
                    GameObject internetCheck = new GameObject();
                    _internetCheck = internetCheck.AddComponent<InternetCheck>();
                    _internetCheck.OnReconnect += Reconnect;
                }

                UserProfileService.LoadUserProfile();
                PlayadLeaderboard.Init(UserProfileService.GetDisplayName(), UserProfileService.GetBestScore());
                Logger.Log("Login", "Login failed");
            }

            async void OnComplete()
            {
                await PlayAdSupport.RefreshUserAsync();
                UserProfileService.GetUserProfile().Gem = PlayAdSupport.GetUser().Money;
                UserProfileService.LocalSync();
            }
#endif
        }

        public async UniTask<bool> BuyRemoveAds()
        {
#if PLAY_ADS
            if (!IsInternetAvailable()) return false;
            OnBeginRequest.Invoke();
            var result = await PlayAdSupport.IAP.Purchase(RemoveAdsPackage);
            OnEndRequest.Invoke();

            return result.isSuccess;
#endif
            return false;
        }
#if PLAY_ADS
        public async UniTask<bool> ShowRewardedAdShopAsync(string adRewardType = AdRewardType.shopGemRewardCD,
            string placement = null, Action<int> onSuccess = null,
            Action onFailed = null, Action onCanceled = null)
        {
            if (!IsInternetAvailable())
            {
                onCanceled?.Invoke();
                return false;
            }

            if (!PlayAdSupport.IsRewardedVideoAvailable)
            {
                FlyTextManager.Instance.ShowFlyText(Constants.NoRewardAd, Color.red);
                onCanceled?.Invoke();
                return false;
            }

            OnBeginRequest.Invoke();
            var result = await PlayAdSupport.ShowVideoRewardAsync(RewardAdsPosition.Shop, adRewardType, placement);
            if (result.isSuccess)
            {
                onSuccess?.Invoke(GetAdCountDownInfo(adRewardType));
                /*FlyTextManager.Instance.ShowFlyText(
                    $"You have received {PlayAdSupport.GetUser().GetConfig<int>(GameConfigName.ShopDiamondScale)} gems",
                    Color.green);*/
                /*UserProfileService.GetUserProfile().Gem = PlayAdSupport.GetUser().Money;
                UserProfileService.Sync();*/
            }
            else
            {
                onFailed?.Invoke();
            }
#if !USE_SDK
            //UserProfileService.AddGems(10);
#endif
            OnEndRequest.Invoke();
            return result.isSuccess;
        }

        public async UniTask<bool> ShowFyberOfferWallAsync()
        {
            if (!IsInternetAvailable()) return false;

            if (!PlayAdSupport.fyberOfferWall.isHas)
            {
                FlyTextManager.Instance.ShowFlyText(Constants.NoFyberOfferWallAds, Color.red);
                return false;
            }

            OnBeginRequest.Invoke();
            //int currentGem = PlayAdSupport.GetUser().Money;
            var result = await PlayAdSupport.fyberOfferWall.ShowAsync();
            /*if (result)
            {
                int rewardGem = PlayAdSupport.GetUser().Money - currentGem;
                //FlyTextManager.Instance.ShowFlyText($"You have received {rewardGem} gems", Color.green);
                UserProfileService.GetUserProfile().Gem = PlayAdSupport.GetUser().Money;
                UserProfileService.LocalSync();
            }
            */

            OnEndRequest.Invoke();
            return result;
        }

        public async UniTask<bool> ShowTapJoyOfferWallAsync()
        {
            if (!IsInternetAvailable()) return false;

            if (!PlayAdSupport.IsHasTapJoy)
            {
                FlyTextManager.Instance.ShowFlyText(Constants.NoTapjoy, Color.red);
                return false;
            }

            OnBeginRequest.Invoke();
            //int currentGem = PlayAdSupport.GetUser().Money;
            var result = await PlayAdSupport.ShowTapJoyAsync();
            if (result)
            {
                UserProfileService.GetUserProfile().Gem = PlayAdSupport.GetUser().Money;
                UserProfileService.LocalSync();
            }

            OnEndRequest.Invoke();
            return result;
        }

        public async void ShowRewardedAdAsync(string adRewardType, string placement, Action<int> onSuccess = null,
            Action onFailed = null, Action onCanceled = null)
        {
            if (!IsInternetAvailable())
            {
                onCanceled?.Invoke();
                return;
            }

            if (!PlayAdSupport.IsRewardedVideoAvailable)
            {
                FlyTextManager.Instance.ShowFlyText(Constants.NoRewardAd, Color.red);
                onCanceled?.Invoke();
                return;
            }

            OnBeginRequest.Invoke();
            var result = await PlayAdSupport.ShowVideoRewardAsync(RewardAdsPosition.Normal, adRewardType, placement);
            if (result.isSuccess)
            {
                onSuccess?.Invoke(GetAdCountDownInfo(adRewardType));
            }
            else
            {
                onFailed?.Invoke();
            }

            OnEndRequest.Invoke();
        }

        public async UniTask ShowInterstitialAdAsync()
        {
            if (!IsBuyNoAds)
            {
                await PlayAdSupport.ShowInterstitialAdAsync();
            }
        }

        public int GetAdCountDownInfo(string type)
        {
            return PlayAdSupport.GetAdCountDownInfo(type);
        }

        public bool IsAvailableRewardedAd(string adRewardType)
        {
#if UNITY_EDITOR
            return GetAdCountDownInfo(adRewardType) == 0;
#else
            return PlayAdSupport.IsRewardedVideoAvailable && GetAdCountDownInfo(adRewardType) == 0;
#endif
        }

        public void ShowBannerAdAsync(AdBannerSize bannerSize = AdBannerSize.AnchoredAdaptive)
        {
#if !UNITY_EDITOR
            if (!IsBuyNoAds)
                PlayAdSupport.ShowBanner(placement: "Startup", bannerSize: bannerSize);
#endif
        }

        public bool IsInternetAvailable()
        {
            bool isInternetAvailable = Application.internetReachability != NetworkReachability.NotReachable;
            if (!isInternetAvailable) FlyTextManager.Instance.ShowFlyText(Constants.NoInternet, Color.red);

            return isInternetAvailable;
        }

        public void DestroyBanner()
        {
            PlayAdSupport.HideBanner();
        }
#endif
    }
}