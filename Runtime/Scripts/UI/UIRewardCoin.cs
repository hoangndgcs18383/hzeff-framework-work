

/*
namespace SAGE.Framework.Core.UI
{
    using System;
    using AssetKits.ParticleImage;
    using Core;
    using DG.Tweening;
    using SAGE.Framework.Core.Extensions;
    using SAGE.Framework.SDK;
    using TMPro;
    using UnityEngine;

    public class UIRewardCoin : MonoBehaviour
    {
        [SerializeField] private RectTransform target;
        [SerializeField] private ParticleImage coinParticleImage;
        [SerializeField] private RectTransform topTarget;
        [SerializeField] private GameObject lockClick;
        [SerializeField] private TMP_Text coinText;

        private Action onCoinParticleComplete;

        private int saparateValue = 0;
        private int currentCoin;
        private int minCoin = 10;
        private int maxCoin = 20;
        private Transform attractorGemTarget;

        private void Awake()
        {
            coinParticleImage.onAnyParticleFinished.AddListener(OnAnyCoinParticleFinished);
            coinParticleImage.onParticleStop.AddListener(OnCoinParticleComplete);
            UserProfileService.OnUserProfileLoaded += OnUserProfileLoaded;
            UserProfileService.OnUserProfileChanged += OnUserProfileLoaded;
        }

        private void OnUserProfileLoaded(UserProfile userProfile)
        {
            currentCoin = userProfile.Coins;
            //coinText.text = currentCoin.ToString();
        }

        private void OnCoinParticleComplete()
        {
            topTarget.DOAnchorPosY(200, 0.5f).SetDelay(0.5f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                lockClick.gameObject.SetSafeActive(false);
                onCoinParticleComplete?.Invoke();
                onCoinParticleComplete = null;
            });
        }

        private void OnAnyCoinParticleFinished()
        {
            int targetCoin = currentCoin + saparateValue;
            coinText.DOComplete();
            coinText.DOCounter(currentCoin, targetCoin, 0.1f);
            currentCoin = targetCoin;
            target.DOComplete();
            target.DOPunchScale(Vector3.one * 0.1f, 0.5f, 1, 0.5f);
            AudioManager.Instance.PlaySound(SoundKey.CollectCoin);
            VibrationManager.Instance.Vibrate();
        }

        public void StartCoinAnimation(int mCoinReward = 0, Action onActionComplete = null)
        {
            lockClick.gameObject.SetSafeActive(true);
            onCoinParticleComplete = onActionComplete;
            saparateValue = mCoinReward / 10;
            //Debug.Log($"StartCoinAnimation: {mCoinReward} + {saparateValue}");

            coinParticleImage.rateOverTime = 10;
            topTarget.DOAnchorPosY(-200f, 0.5f).SetEase(Ease.OutBack);
            coinParticleImage.Play();
        }
    }
}*/