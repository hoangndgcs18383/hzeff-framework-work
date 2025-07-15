/*namespace SAGE.Framework.Core.UI
{
    using EasyTransition;
    using System;
#if UNITASK_SUPPORT
    using Cysharp.Threading.Tasks;
#else
    using System.Threading.Tasks;
#endif
    using UnityEngine;
    using System.Collections;

    public class LoadingTransitionHandler : MonoBehaviour
    {
        [SerializeField] TransitionSettings transitionSettingsForFadeIn;
        [SerializeField] TransitionSettings transitionSettingsForFadeOut;
        [SerializeField] private CanvasGroup fadeCanvasGroup;
        [SerializeField] private float fadeDuration = 1f;

        private void Awake()
        {
            fadeCanvasGroup.alpha = 0;
            fadeCanvasGroup.blocksRaycasts = false;
        }

        public void FadeIn(Action onComplete = null)
        {
            StartCoroutine(IEFadeIn(onComplete));
        }

        public void FadeOut(Action onComplete = null)
        {
            StartCoroutine(IEFadeOut(onComplete));
        }

#if UNITASK_SUPPORT
        public async UniTask FadeInAsync()
        {
            await IEFadeIn().ToUniTask();
        }

        public async UniTask FadeOutAsync()
        {
            await IEFadeOut().ToUniTask();
        }
#else
        public async Task FadeInAsync()
        {
            await Task.Run(() => StartCoroutine(IEFadeIn()));
        }

        public async Task FadeOutAsync()
        {
            await Task.Run(() => StartCoroutine(IEFadeOut()));
        }
#endif

        private IEnumerator IEFadeIn(Action onComplete = null)
        {
            TransitionManager.Instance().Transition(transitionSettingsForFadeIn, 0);
            yield return new WaitForSeconds(transitionSettingsForFadeIn.transitionTime);
            onComplete?.Invoke();
        }

        private IEnumerator IEFadeOut(Action onComplete = null)
        {
            TransitionManager.Instance().Transition(transitionSettingsForFadeOut, 0);
            yield return new WaitForSeconds(transitionSettingsForFadeOut.transitionTime);
            onComplete?.Invoke();
        }
    }
}*/