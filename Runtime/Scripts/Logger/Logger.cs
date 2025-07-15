namespace SAGE.Framework.Core
{
    using Log;
    using UnityEngine;

    public static class Logger
    {
        //[System.Diagnostics.Conditional("ENABLE_LOG")]
        public static void Log(string tag, object message, Object context = null, Color color = default)
        {
            DebugHistoryManager.Instance.Log(message.ToString(), tag, LogType.Log);
            string mMessage = $"<color=white>[{message}]</color>";
            if (color != default)
            {
                mMessage = $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>[{message}]</color>";
            }

            Debug.Log(string.Format("{0} {1}", $"<b><color=green>[{tag}]</color></b>", mMessage, context));
        }

        //[System.Diagnostics.Conditional("ENABLE_LOG")]
        public static void LogWarning(string tag, object message, Object context = null, Color color = default)
        {
            DebugHistoryManager.Instance.Log(message.ToString(), tag, LogType.Warning);
            string mMessage = $"<color=yellow>[{message}]</color>";
            if (color != default)
            {
                mMessage = $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>[{message}]</color>";
            }

            Debug.LogWarning(string.Format("{0} {1}", $"<b><color=green>[{tag}]</color></b>", mMessage, context));
        }

        //[System.Diagnostics.Conditional("ENABLE_LOG")]
        public static void LogError(string tag, object message, Object context = null, Color color = default)
        {
            DebugHistoryManager.Instance.Log(message.ToString(), tag, LogType.Error);
            string mMessage = $"<color=red>[{message}]</color>";
            if (color != default)
            {
                mMessage = $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>[{message}]</color>";
            }

            Debug.LogError(string.Format("{0} {1}", $"<b><color=green>[{tag}]</color></b>", mMessage, context));
        }
    }
}