namespace SAGE.Framework.Extensions
{
    using System;
    using System.Collections;
    using UnityEngine;

    public class InternetCheck : MonoBehaviour
    {
        public event Action OnReconnect = delegate { };
        public string serverToPing = "8.8.8.8";
        public float pingInterval = 2f;

        void Start()
        {
            DontDestroyOnLoad(gameObject);
            StartCoroutine(PingRepeatedly());
        }

        IEnumerator PingRepeatedly()
        {
            while (true) // Run indefinitely
            {
                yield return StartCoroutine(PingServer());

                // Wait for the specified interval before pinging again
                yield return new WaitForSeconds(pingInterval);
            }
        }

        IEnumerator PingServer()
        {
            Ping ping = new Ping(serverToPing);
            float timeout = 5f; // Timeout in seconds
            float startTime = Time.time;

            while (!ping.isDone)
            {
                if (Time.time - startTime > timeout)
                {
                    Debug.Log("<color=yellow>Ping timed out. No internet connection.</color>");
                    yield break;
                }

                yield return null;
            }

            if (ping.isDone && ping.time >= 0)
            {
                Debug.Log("<color=green>Ping successful. Internet connection is available. Ping time: " + ping.time +
                          "ms</color>");
                OnReconnect?.Invoke();
            }
            else
            {
                Debug.Log("<color=red>Ping failed. No internet connection.</color>");
            }
        }

        public void StopPing()
        {
            StopAllCoroutines();
        }
    }
}