using System.Collections.Generic;
using System.Linq;

namespace SAGE.Framework.Core.Tools.Editor
{
#if UNITY_EDITOR
    using UnityEditor;

    public class LoggerEditorWindow : EditorWindow
    {
#if ENABLE_LOG
        [MenuItem("Tools/Logger/Disable Log")]
        public static void DisableLog()
        {
            List<string> defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone)
                .Split(';').ToList();
            if (!defines.Contains("ENABLE_LOG"))
            {
                return;
            }

            defines.Remove("ENABLE_LOG");
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, defines.ToArray());
        }
#else
        [MenuItem("Tools/Logger/Enable Log")]
        public static void EnableLog()
        {
            List<string> defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone)
                .Split(';').ToList();
            if (defines.Contains("ENABLE_LOG"))
            {
                return;
            }

            defines.Add("ENABLE_LOG");
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, defines.ToArray());
        }
#endif
    }
#endif
}