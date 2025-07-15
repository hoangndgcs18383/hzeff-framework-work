namespace SAGE.Framework.Core.Extensions
{
    using UnityEngine;

    public class DontDestroyOnLoad : MonoBehaviour
    {
        void Start()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}