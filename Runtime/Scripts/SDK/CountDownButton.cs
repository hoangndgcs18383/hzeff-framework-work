namespace SAGE.Framework.Extensions
{
    using System;
    using System.Collections;
    using TMPro;
    using UnityEngine;

    public class CountDownButton : MonoBehaviour
    {
        [SerializeField] private GameObject[] hiddenObjects;

        private TMP_Text _countDownText;


        public TMP_Text CountDownText
        {
            get
            {
                if (_countDownText == null)
                {
                    _countDownText = gameObject.GetComponentInChildren<TMP_Text>();
                }

                return _countDownText;
            }
        }

        public void SetCountDown(int countDown, Action onComplete = null)
        {
            SetEnable(true);
            foreach (var obj in hiddenObjects)
            {
                if (obj == null) continue;
                obj.SetSafeActive(false);
            }

            StartCoroutine(CountDown(countDown, onComplete));
        }

        public void SetEnable(bool enable)
        {
            //Mask.enabled = enable;
            gameObject.SetSafeActive(enable);
        }

        private IEnumerator CountDown(int countDown, Action onComplete = null)
        {
            float currentTime = Time.realtimeSinceStartup;
            float endTime = currentTime + countDown;

            while (true)
            {
                currentTime = Time.realtimeSinceStartup;
                int remainTime = (int)(endTime - currentTime);

                CountDownText.text = $"{remainTime / 60:00}:{remainTime % 60:00}";
                if (remainTime <= 0)
                {
                    CountDownText.text = "00:00";
                    SetEnable(false);
                    foreach (var obj in hiddenObjects)
                    {
                        if (obj == null) continue;
                        obj.SetSafeActive(true);
                    }

                    break;
                }

                yield return null;
            }

            onComplete?.Invoke();
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}