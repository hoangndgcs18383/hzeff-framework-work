namespace SAGE.Framework.Core
{
    using UnityEngine;

    public class PooledObject : MonoBehaviour
    {
        public string PoolTag { get; set; }

        public void ReturnToPool()
        {
            PoolManager.Instance.Return(gameObject);
        }
    }
}