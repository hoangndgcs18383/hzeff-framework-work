namespace SAGE.Framework.Core.Tools
{
    using Sirenix.OdinInspector;
    using System;
    using UnityEngine;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using UnityEditor;
    using UnityEditor.AddressableAssets;
    using UnityEditor.AddressableAssets.Settings;
    using UnityEngine.AddressableAssets;

    [CreateAssetMenu(menuName = "SAGE/Tools/AddressLibTool")]
    public class AddressLibTool : ScriptableObject
    {
        [Title("Template")] [FoldoutGroup("Require Template")] [Required] [Sirenix.OdinInspector.FilePath]
        public string templatePath;

        [FoldoutGroup("Require Template")] [Required] [FolderPath]
        public string generatePath;

        [FoldoutGroup("Require Template")]
        public string namespaceName;

        [SerializeField] private List<AddressReferenceEditor> screenReferences = new List<AddressReferenceEditor>();

        [Space] [ReadOnly] [HideLabel] [ProgressBar(0, 100, ColorGetter = "GetBarColor")] [SerializeField]
        private float ProcessB = 0f;

        private bool CompleteBuild => ProcessB >= 100f || ProcessB <= 0f;

        private Color GetBarColor(float value)
        {
            return Color.Lerp(Color.red, Color.green, MathF.Pow(value / 100f, 2));
        }

        [PropertySpace(20)]
        [EnableIf("CompleteBuild")]
        [Button(ButtonSizes.Gigantic)]
        public async void Build()
        {
            screenReferences.Clear();
            string[] guids = AssetDatabase.FindAssets("t:AddressReferenceEditor");
            templatePath = templatePath.Replace("Assets/", "");
            generatePath = generatePath.Replace("Assets/", "");

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                AddressReferenceEditor screenReference = AssetDatabase.LoadAssetAtPath<AddressReferenceEditor>(path);
                screenReferences.Add(screenReference);
            }

            DeleteAllGeneratedScripts();

            for (int i = 0; i < screenReferences.Count; i++)
            {
                string template =
                    await File.ReadAllTextAsync(Path.Combine(Application.dataPath, templatePath));

                template = template.Replace("{0}", namespaceName);
                template = template.Replace("{1}", screenReferences[i].className);
                template = template.Replace("{2}", screenReferences[i].getNameMethod);
                StringBuilder sb = new StringBuilder();
                int index = 0;
                foreach (AddressReferenceConfig screen in screenReferences[i].addresses)
                {
                    if (screen == null) continue;

                    if (screen.isAddressable)
                    {
                        AssignAddress(screen.reference, screenReferences[i].group, screen.key,
                            screen.labels);
                        sb.AppendLine("\t\t\t\tcase \"" + screen.key + "\":");
                        sb.AppendLine("\t\t\t\t\treturn \"" +
                                      GetGuid(screen.reference) + "\";");


                        ProcessB = (index + 1) * 100 / screenReferences[i].addresses.Count;
                        index++;
                        await Task.Delay(100);
                    }
                }

                template = template.Replace("{3}", sb.ToString());
                sb.Clear();

                string resourcesFolder = "LocalAssets";
                UnityEngine.Object[] resources = Resources.LoadAll(resourcesFolder, typeof(UnityEngine.Object));

                //clear in resources
                foreach (UnityEngine.Object resource in resources)
                {
                    Debug.Log(resource.name);
                    string path = AssetDatabase.GetAssetPath(resource);
                    AssetDatabase.DeleteAsset(path);
                }

                await Task.Delay(100);

                foreach (AddressReferenceConfig screen in screenReferences[i].addresses)
                {
                    if (screen == null) continue;

                    if (!screen.isAddressable)
                    {
                        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
                        AddressableAssetEntry entry =
                            settings.FindAssetEntry(screen.reference.AssetGUID);
                        if (entry != null)
                            settings.RemoveAssetEntry(entry.guid);

                        sb.AppendLine("\t\t\t\tcase \"" + screen.key + "\":");
                        sb.AppendLine("\t\t\t\t\treturn true;");
                        string path = Path.Combine(Application.dataPath, resourcesFolder);
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);

                        string assetPath = AssetDatabase.GetAssetPath(screen.asset);
                        string assetName = Path.GetFileName(assetPath);
                        string newPath = Path.Combine(path, assetName);
                        if (File.Exists(newPath))
                        {
                            File.Delete(newPath);
                        }

                        File.Copy(assetPath, newPath);
                        ProcessB = (i + 1) * 100 / screenReferences.Count;
                        Debug.Log($"Copying {ProcessB} to {(i + 1) * 100 / screenReferences.Count}");
                        await Task.Delay(100);
                    }
                }

                template = template.Replace("{4}", sb.ToString());
                sb.Clear();

                foreach (AddressReferenceConfig screen in screenReferences[i].addresses)
                {
                    sb.AppendLine("\t\tpublic static string " + screen.key.ToUpper().Replace(" ", "_") + " = \"" +
                                  screen.key + "\";");
                }

                template = template.Replace("{5}", sb.ToString());

                await File.WriteAllTextAsync(
                    Path.Combine(Application.dataPath,
                        generatePath + "/" + screenReferences[i].className + ".cs"),
                    template);
            }

            SetDirtyBuild();
            Debug.Log("Build AddressLibTool success");
        }

        private void SetDirtyBuild()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void DeleteAllGeneratedScripts()
        {
            string[] scripts = Directory.GetFiles(Path.Combine(Application.dataPath, generatePath), "*.cs",
                SearchOption.AllDirectories);

            for (int i = 0; i < scripts.Length; i++)
            {
                File.Delete(scripts[i]);
            }
        }

        [Button]
        private void ParseTemplate()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("// This file is auto-generated. DO NOT EDIT");
            sb.AppendLine("public static class {0}");
            sb.AppendLine("{");
            sb.AppendLine("\tpublic static string {1}(string key)");
            sb.AppendLine("\t{");
            sb.AppendLine("\t\tswitch (key)");
            sb.AppendLine("\t\t{");
            sb.AppendLine("{2}");
            sb.AppendLine("\t\t}");
            sb.AppendLine("\t\treturn null;");
            sb.AppendLine("\t}");
            sb.AppendLine("}");

            File.WriteAllText(Path.Combine(Application.dataPath, templatePath), sb.ToString());
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }


        private void AssignAddress<T>(T data, string groupName, string address, string[] labels)
            where T : AssetReference
        {
            data.SetEditorAsset(data.editorAsset);

            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            AddressableAssetGroup group = settings.FindGroup(groupName);
            if (group == null)
                group = settings.CreateGroup(groupName, false, false, true, new List<AddressableAssetGroupSchema>());
            AddressableAssetEntry entry =
                settings.CreateOrMoveEntry(data.AssetGUID, group);
            entry.address = address;
            entry.labels.Clear(); //clear all labels
            if (labels != null)
                foreach (string label in labels)
                {
                    entry.SetLabel(label, true, true); //add label
                }

            //create schema content upload restriction
            AddressableAssetGroupSchema schema = group.GetSchema<AddressableAssetGroupSchema>();
            if (schema == null)
                schema = group.AddSchema<AddressableAssetGroupSchema>();
        }


        private string GetGuid<T>(T data) where T : AssetReference
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            AddressableAssetEntry entry =
                settings.FindAssetEntry(data.AssetGUID);
            return entry.guid;
        }
    }
}