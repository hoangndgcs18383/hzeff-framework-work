namespace SAGE.Framework.Core.Extensions
{
    using System;
    using System.Collections;
    using UnityEngine;
#if TIMING
    using System.Collections.Generic;
    using MEC;
#endif

    public static class MonoBehaviourExtensions
    {
        public static Coroutine Delay(this MonoBehaviour mono, float delay, Action action, bool unscaledTime = false)
        {
            return mono.StartCoroutine(DelayCoroutine(delay, action, unscaledTime));
        }

        private static IEnumerator DelayCoroutine(float delay, Action action, bool unscaledTime)
        {
            if (unscaledTime)
            {
                yield return new WaitForSecondsRealtime(delay);
            }
            else
            {
                yield return new WaitForSeconds(delay);
            }

            action?.Invoke();
        }

#if TIMING
        public static CoroutineHandle Delay(this MonoBehaviour mono, float delay, Action action,
            Segment segment = Segment.Update)
            => Timing.RunCoroutine(DelayCoroutineTiming(delay, action), segment);

        private static IEnumerator<float> DelayCoroutineTiming(float delay, Action action)
        {
            yield return Timing.WaitForSeconds(delay);
            action?.Invoke();
        }
#endif
    }
}