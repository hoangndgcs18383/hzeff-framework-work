/*namespace SAGE.Framework.Core.UI
{
    using System.Collections.Generic;
    using MEC;
    using UnityEngine;

    public class LoadingCircleHandler : MonoBehaviour
    {
        [SerializeField] private CanvasGroup loadingCircle;
        [SerializeField] private float hideInterval = 3f;

        private CoroutineHandle _loadingCoroutine;

        protected void Awake()
        {
            Hide();
        }

        public void Show(float delayShow = 3f)
        {
            if (_loadingCoroutine.IsValid)
                Timing.KillCoroutines(_loadingCoroutine);

            _loadingCoroutine = Timing.RunCoroutine(IEShow(delayShow));
            transform.SetAsLastSibling();
        }

        private IEnumerator<float> IEShow(float delayShow = 3f)
        {
            loadingCircle.blocksRaycasts = true;
            loadingCircle.alpha = 0;
            yield return Timing.WaitForSeconds(delayShow);
            loadingCircle.alpha = 1;
            yield return Timing.WaitForSeconds(hideInterval);
            Hide();
        }

        public void Hide()
        {
            if (_loadingCoroutine.IsValid)
                Timing.KillCoroutines(_loadingCoroutine);

            loadingCircle.blocksRaycasts = false;
            loadingCircle.alpha = 0;
        }
    }
}*/