namespace SAGE.Framework.UI
{
    using Cysharp.Threading.Tasks;
    using DG.Tweening;
    using UnityEngine;
    using UnityEngine.UI;

    public class UILoading : BaseScreen
    {
        [SerializeField] private Image _fillImage;
        [SerializeField] private RectTransform _fillHandle;

        public async UniTask SetProgressTweenAsync(float progress, float duration = 0.5f)
        {
            _fillImage.DOKill();
            await _fillImage.DOFillAmount(progress, duration).OnUpdate(UpdateFillHandlePosition)
                .SetEase(Ease.Linear)
                .AsyncWaitForCompletion();
        }

        private void UpdateFillHandlePosition()
        {
            if (_fillHandle)
            {
                _fillHandle.anchoredPosition = new Vector2(
                    _fillImage.rectTransform.rect.width * _fillImage.fillAmount,
                    _fillHandle.anchoredPosition.y);
            }
        }

        public void SetProgressTween(float progress, float duration = 0.5f)
        {
            _fillImage.DOKill();
            _fillImage.DOFillAmount(progress, duration)
                .SetEase(Ease.Linear);
            UpdateFillHandlePosition();
        }

        public void SetProgress(float progress)
        {
            _fillImage.fillAmount = progress;
            UpdateFillHandlePosition();
        }
    }
}