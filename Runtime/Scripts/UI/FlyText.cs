/*namespace SAGE.Framework.Core.UI
{
    using Core;
    using DG.Tweening;
    using TMPro;
    using UnityEngine;

    public class FlyText : MonoBehaviour
    {
        [SerializeField] private TMP_Text _flyText;
        [SerializeField] private CanvasGroup _canvasGroup;

        public void ShowFlyText(string text, Color color, float duration = 3f)
        {
            _flyText.text = text;
            _flyText.color = color;
            _canvasGroup.alpha = 1;

            _canvasGroup.DOFade(0, duration);
            transform.DOLocalMoveY(100, duration).OnComplete(() =>
            {
                transform.localPosition = Vector3.zero;
                FlyTextManager.Instance.ReturnFlyText(this);
            });
        }
    }
}*/