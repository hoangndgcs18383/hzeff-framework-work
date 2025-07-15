/*using System;
using PlayAd.SDK.Ads;
using SAGE.Framework.SDK;
using UnityEngine;
using UnityEngine.UI;

public class DemoSDK : MonoBehaviour
{
    [SerializeField] private Button loginButton;
    [SerializeField] private Button showAdsRewardButton;
    [SerializeField] private CountDownButton countDownButton;

    private async void Start()
    {
        loginButton.onClick.AddListener(OnLogin);
        showAdsRewardButton.onClick.AddListener(OnShowAdsReward);

        await SDKHandler.Instance.LoginAsync();

        if (SDKHandler.Instance.IsAvailableRewardedAd(AdRewardType.shopCoinRewardCD))
        {
            countDownButton.SetEnable(false);
        }
        else
        {
            countDownButton.SetCountDown(SDKHandler.Instance.GetAdCountDownInfo(AdRewardType.shopCoinRewardCD));
        }
    }

    private void OnLogin()
    {
        SDKHandler.Instance.LoginAsync();
    }

    private void OnShowAdsReward()
    {
        /*SDKHandler.Instance.ShowRewardedAdAsync(AdRewardType.shopCoinRewardCD,
            (cd) => { countDownButton.SetCountDown(cd); });#1#
    }
}*/