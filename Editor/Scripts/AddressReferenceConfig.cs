namespace SAGE.Framework.Core.Tools
{
    using System;
    using Sirenix.OdinInspector;
    using UnityEngine;
    using UnityEngine.AddressableAssets;

    [Serializable]
    public class AddressReferenceConfig
    {
        [Tooltip("This is the key that will be used to reference the asset in code.")]
        public string key;

        public string[] labels;
        public bool isAddressable;

        [HideIf("isAddressable")] [Required] public UnityEngine.Object asset;

        [Tooltip("This is the asset that will be referenced in code.")] [ShowIf("isAddressable")] [Required]
        public AssetReference reference;
    }
}