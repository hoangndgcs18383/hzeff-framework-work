namespace SAGE.Framework.Core.Tools.Editor
{
    using System;
    using UnityEditor;
    using UnityEngine;

    public class AddressLibPostprocessor : AssetPostprocessor
    {
        private const string _addressLibPath = "Assets/QuickFramework/Editor/Tools/SO/AddressLibTool.asset";

        static async void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            if (IsHasUpdated()) return;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

            try
            {
                AddressLibTool addressLibTool = AssetDatabase.LoadAssetAtPath<AddressLibTool>(_addressLibPath);
                if (addressLibTool == null) return;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredEditMode)
            {
                EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
                AddressLibTool addressLibTool = AssetDatabase.LoadAssetAtPath<AddressLibTool>(_addressLibPath);
                if (addressLibTool == null) return;
                addressLibTool.Build();
                EditorPrefs.SetBool("AddressLib", true);
                Debug.Log("AddressLib has been updated");
            }
        }

        private static bool IsHasUpdated()
        {
            return EditorPrefs.GetBool("AddressLib", false);
        }
    }
}