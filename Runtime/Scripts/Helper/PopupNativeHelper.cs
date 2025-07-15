/*namespace SAGE.Framework.Core.Helper
{
    using PlayAd.SDK.Ads;
    using SAGE.Framework.SDK;
    using UnityEngine;

    public class PopupNativeHelper : MonoBehaviour
    {
        [SerializeField] private PlayAdAdmobNativeAd nativeAds;

        private void OnEnable()
        {
            SDKHandler.Instance.OnLoginSuccess += OnLoginSuccess;
            EnableNativeAds();
        }

        private void OnDisable()
        {
            SDKHandler.Instance.OnLoginSuccess -= OnLoginSuccess;
            HideNativeAds();
        }

        private void OnLoginSuccess()
        {
            EnableNativeAds();
        }

        private void EnableNativeAds()
        {
            if (!PlayAdSupport.GetUser().BuyNoAds)
            {
                if (nativeAds != null) nativeAds.RequestNativeAd();
            }
            else
            {
                if (nativeAds != null) nativeAds.Destroy();
            }
        }

        private void HideNativeAds()
        {
            if (!PlayAdSupport.GetUser().BuyNoAds)
            {
                if (nativeAds != null) nativeAds.Hide();
            }
        }
    }
}*/