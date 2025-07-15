using SAGE.Framework.Core.Extensions;

namespace SAGE.Framework.Core.Log
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class DebugHistoryManager : BehaviorSingleton<DebugHistoryManager>
    {
        [Serializable]
        public class DebugEntry
        {
            public string message;
            public DateTime timestamp;
            public string category;
            public LogType logType;
            public string stackTrace;
            public bool showStackTrace;
            public bool isExpanded;
        }

        private int maxEntries = 100;
        private List<DebugEntry> entries = new List<DebugEntry>();

        public void Log(string message, string category = "Default",
            LogType logType = LogType.Log,
            bool captureStackTrace = false)
        {
            var newEntry = new DebugEntry
            {
                message = message,
                timestamp = DateTime.Now,
                category = category,
                logType = logType,
                stackTrace = captureStackTrace ? GetCleanStackTrace(2) : ""
            };

            entries.Insert(0, newEntry);

            while (entries.Count > maxEntries)
            {
                entries.RemoveAt(entries.Count - 1);
            }
            
        }

        private string GetCleanStackTrace(int skipFrames)
        {
            var stackTrace = new System.Diagnostics.StackTrace(skipFrames, true);
            return stackTrace.ToString()
                .Replace("UnityEngine.Debug.", "") // Clean up Unity internals
                .Replace("DebugHistoryManager.Log", "");
        }

        public List<DebugEntry> GetEntries() => entries;

        public void ClearHistory()
        {
            entries.Clear();
        }
    }
}