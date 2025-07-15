namespace SAGE.Framework.Extensions
{
    using System;
#if PLAY_ADS
    using PlayAd.SDK.Ads;
    using SAGE.Framework.SDK;
#endif
    using System.Collections;
    using SAGE.Framework.Core.UI;
    using Sirenix.OdinInspector;
    using UnityEngine.UI;
    using UnityEngine;

    public class AdsCDHelper : MonoBehaviour
    {
#if PLAY_ADS
        [SerializeField] private bool useCoinRewardAnimation = true;
        [SerializeField] private Button rewardAdButton;

        [SerializeField] private int rewardCoinsReward = 30;

        [ValueDropdown("AdRewardTypeKey")] [SerializeField]
        private string AdRewardTypeString = "shopCoinRewardCD";

        [SerializeField] private CountDownButton countDownButton;

        public event Action onRewardAdButtonClicked = delegate { };
        public event Action onStartAd = delegate { };
        public event Action onCompleteAd = delegate { };
        public event Action onFailedAd = delegate { };

        private void Awake()
        {
            rewardAdButton.onClick.AddListener(OnRewardAdButtonClicked);
        }

        private void OnEnable()
        {
            CheckCDAds();
        }

        public void ManualSetRewardCoins(int coins)
        {
            rewardCoinsReward = coins;
        }

        private void OnRewardAdButtonClicked()
        {
            onRewardAdButtonClicked.Invoke();
            SDKHandler.Instance.ShowRewardedAdAsync(AdRewardTypeString, AdRewardTypeString, i =>
            {
                onStartAd.Invoke();
                SDKHandler.Instance.Track(SDKHandler.COIN_SHOP, AdRewardTypeString, 1);
                CheckCDAds();
                if (useCoinRewardAnimation)
                {
                    UserProfileService.AddCoins(rewardCoinsReward, false);
                    UIManager.Instance.StartCoinAnimation(rewardCoinsReward, () =>
                    {
                        onCompleteAd.Invoke();
                        UserProfileService.Sync();
                    });
                }
                else
                {
                    onCompleteAd.Invoke();
                    UserProfileService.AddCoins(rewardCoinsReward);
                }
            }, () => { onFailedAd.Invoke(); });
        }

        public void CheckCDAds()
        {
            if (!SDKHandler.Instance.IsAvailableRewardedAd(AdRewardTypeString))
            {
                rewardAdButton.interactable = false;
                countDownButton.SetCountDown(SDKHandler.Instance.GetAdCountDownInfo(AdRewardTypeString),
                    () => { rewardAdButton.interactable = true; });
            }
            else
            {
                countDownButton.SetEnable(false);
                rewardAdButton.interactable = true;
            }
        }

        private static IEnumerable AdRewardTypeKey => new ValueDropdownList<string>()
        {
            AdRewardType.doubleRewardCD,
            AdRewardType.adRewardPopUpCD,
            AdRewardType.shopGemRewardCD,
            AdRewardType.shopCoinRewardCD,
            AdRewardType.gameplayCD,
            AdRewardType.levelUpCD,
            AdRewardType.unlockSkinCD
        };
#endif
    }
}