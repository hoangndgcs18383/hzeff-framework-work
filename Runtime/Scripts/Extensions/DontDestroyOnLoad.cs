namespace SAGE.Framework.Extensions
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