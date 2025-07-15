#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using SAGE.Framework.Log;
using UnityEditor;
using UnityEngine;

public class DebugHistoryWindow : EditorWindow
{
    private Vector2 scrollPosition;
    private string searchFilter = "";
    private bool showErrors = true;
    private bool showWarnings = true;
    private bool showLogs = true;
    private bool showStackTraces = false;
    private bool autoScroll = true;
    private static bool needsRefresh;

    private GUIStyle timestampStyle;
    private GUIStyle categoryStyle;
    private GUIStyle messageStyle;
    private GUIStyle stackTraceStyle;

    private Dictionary<LogType, Color> logTypeColors = new Dictionary<LogType, Color>()
    {
        { LogType.Log, new Color(0.8f, 0.8f, 0.8f) },
        { LogType.Warning, new Color(1f, 0.8f, 0.3f) },
        { LogType.Error, new Color(1f, 0.4f, 0.4f) },
        { LogType.Exception, new Color(1f, 0.2f, 0.2f) },
        { LogType.Assert, new Color(0.8f, 0.2f, 0.8f) }
    };

    [MenuItem("Tools/Debug History")]
    static void ShowWindow()
    {
        var window = GetWindow<DebugHistoryWindow>();
        window.titleContent = new GUIContent("Debug History");
        window.minSize = new Vector2(600, 400);
        window.Show();
    }

    private void OnEnable()
    {
        InitializeStyles();
    }

    private void InitializeStyles()
    {
        timestampStyle = new GUIStyle(EditorStyles.label)
        {
            fontSize = 10,
            normal = { textColor = new Color(0.7f, 0.7f, 0.7f) },
            alignment = TextAnchor.MiddleLeft,
            fixedWidth = 80
        };

        categoryStyle = new GUIStyle(EditorStyles.label)
        {
            fontSize = 10,
            normal = { textColor = new Color(0.8f, 0.8f, 1f) },
            alignment = TextAnchor.MiddleLeft,
            fixedWidth = 120
        };

        messageStyle = new GUIStyle(EditorStyles.label)
        {
            fontSize = 12,
            wordWrap = true,
            richText = true
        };

        stackTraceStyle = new GUIStyle(EditorStyles.label)
        {
            fontSize = 10,
            normal = { textColor = new Color(0.7f, 0.7f, 0.7f) },
            wordWrap = true,
            padding = new RectOffset(20, 0, 0, 0)
        };
    }

    void OnGUI()
    {
        DrawToolbar();
        DrawLogEntries();
    }

    private void DrawToolbar()
    {
        GUILayout.BeginHorizontal(EditorStyles.toolbar);

        searchFilter = EditorGUILayout.TextField(searchFilter, EditorStyles.toolbarSearchField);

        showErrors = GUILayout.Toggle(showErrors, "Errors", EditorStyles.toolbarButton);
        showWarnings = GUILayout.Toggle(showWarnings, "Warnings", EditorStyles.toolbarButton);
        showLogs = GUILayout.Toggle(showLogs, "Logs", EditorStyles.toolbarButton);
        showStackTraces = GUILayout.Toggle(showStackTraces, "Stack Traces", EditorStyles.toolbarButton);
        autoScroll = GUILayout.Toggle(autoScroll, "Auto Scroll", EditorStyles.toolbarButton);

        if (GUILayout.Button("Clear", EditorStyles.toolbarButton))
        {
            DebugHistoryManager.Instance.ClearHistory();
        }

        if (GUILayout.Button("Expand All", EditorStyles.toolbarButton))
        {
            foreach (var entry in DebugHistoryManager.Instance.GetEntries())
            {
                entry.isExpanded = true;
            }
        }

        if (GUILayout.Button("Collapse All", EditorStyles.toolbarButton))
        {
            foreach (var entry in DebugHistoryManager.Instance.GetEntries())
            {
                entry.isExpanded = false;
            }
        }

        GUILayout.EndHorizontal();
    }

    private void DrawLogEntries()
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        var entries = DebugHistoryManager.Instance.GetEntries()
            .Where(FilterEntry)
            .Reverse();

        foreach (var entry in entries)
        {
            DrawEntry(entry);
        }

        GUILayout.EndScrollView();

        if (autoScroll && Event.current.type == EventType.Repaint)
        {
            scrollPosition.y = Mathf.Infinity;
        }
    }

    private bool FilterEntry(DebugHistoryManager.DebugEntry entry)
    {
        if (!string.IsNullOrEmpty(searchFilter) &&
            !entry.message.Contains(searchFilter) &&
            !entry.category.Contains(searchFilter))
        {
            return false;
        }

        return entry.logType switch
        {
            LogType.Error => showErrors,
            LogType.Warning => showWarnings,
            LogType.Log => showLogs,
            _ => true
        };
    }

    private void DrawEntry(DebugHistoryManager.DebugEntry entry)
    {
        // Set message color based on log type
        Color logColor = logTypeColors.ContainsKey(entry.logType)
            ? logTypeColors[entry.logType]
            : Color.white;

        GUILayout.BeginVertical(GUI.skin.box);

        // Main entry line with foldout
        GUILayout.BeginHorizontal();
        entry.isExpanded = EditorGUILayout.Foldout(entry.isExpanded,
            $"[{entry.timestamp:HH:mm:ss.fff}] [{entry.category}] {entry.message}",
            true,
            new GUIStyle(EditorStyles.foldout)
            {
                normal = { textColor = logColor },
                onNormal = { textColor = logColor },
                hover = { textColor = logColor },
                onHover = { textColor = logColor },
                active = { textColor = logColor },
                onActive = { textColor = logColor },
                focused = { textColor = logColor },
                onFocused = { textColor = logColor }
            });

        GUILayout.EndHorizontal();

        // Collapsible section
        if (entry.isExpanded)
        {
            // Stack trace
            if (!string.IsNullOrEmpty(entry.stackTrace))
            {
                stackTraceStyle.normal.textColor = logColor * 0.8f; // Dimmed version of log color
                GUILayout.Label("Stack Trace:", stackTraceStyle);
                GUILayout.Label(entry.stackTrace, stackTraceStyle);
            }

            // Additional details (optional)
            GUILayout.BeginHorizontal();
            GUILayout.Label("Log Type:", stackTraceStyle);
            GUILayout.Label(entry.logType.ToString(), stackTraceStyle);
            GUILayout.EndHorizontal();
        }

        GUILayout.EndVertical();
    }

    void Update()
    {
        if (EditorApplication.isPlaying)
        {
            Repaint();
        }
    }
}
#endif