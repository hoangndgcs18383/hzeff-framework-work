#if UNITY_EDITOR
namespace SAGE.Framework.Core.Tools.Editor
{
    /*using Sirenix.OdinInspector.Editor;
    using UnityEditor;
    using UnityEngine;

    public class TreeEditorTool : OdinMenuEditorWindow
    {
        [MenuItem("HZeff/Tools/AddressTool", priority = 1000)]
        private static void OpenWindow()
        {
            GetWindow<TreeEditorTool>("AddressTool");
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            string rootPath = "Assets/QuickFramework/Editor/Tools/SO";
            var tree = new OdinMenuTree();
            tree.Selection.SupportsMultiSelect = false;
            tree.AddAllAssetsAtPath("General", rootPath, typeof(ScriptableObject));
            tree.AddAllAssetsAtPath("List Addressable", rootPath + "/Refereces", typeof(ScriptableObject));
            return tree;
        }
    }*/
}
#endif