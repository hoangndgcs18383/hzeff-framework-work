using UnityEngine.AddressableAssets;

namespace SAGE.Framework.Core.Tools.Editor
{
#if UNITY_EDITOR
    using UnityEditor;
    using Object = UnityEngine.Object;
#endif
    using System.Collections.Generic;
    //using Sirenix.OdinInspector;
    using UnityEngine;

    [CreateAssetMenu(fileName = "AddressReferenceEditor", menuName = "SAGE/Tools/AddressReferenceEditor")]
    public class AddressReferenceEditor : ScriptableObject
    {
        //[Title("General")] [Required] 
        public string group;

        //[Title("Template")] [FoldoutGroup("Require Template")] [Required]
        public string className;

        //[FoldoutGroup("Require Template")] [Required]
        public string getNameMethod = "GetName";

        //[Title("Address References")] 
        public List<AddressReferenceConfig> addresses;

#if UNITY_EDITOR
        //[FolderPath] 
        [SerializeField] private string Folder;

        //[Button]
        public void Insert()
        {
            addresses.Clear();
            List<Object> objects = LoadAssetsAtPath<Object>(Folder);

            for (int i = 0; i < objects.Count; i++)
            {
                AssetReference reference =
                    new AssetReference(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(objects[i])));
                addresses.Add(new AddressReferenceConfig
                {
                    key = "M" + objects[i].name,
                    isAddressable = true,
                    labels = new string[]
                    {
                        group
                    },
#if ENABLE_ADDRESS
                    reference = reference,
#endif
                });
            }
        }

        public static List<T> LoadAssetsAtPath<T>(string path) where T : Object
        {
            List<T> assets = new List<T>();
            string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name, new[] { path });

            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null)
                {
                    assets.Add(asset);
                }
            }

            return assets;
        }
#endif
    }
}