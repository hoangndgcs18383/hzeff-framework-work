namespace SAGE.Framework.Core.PoolingSystem
{
    public interface IPoolable
    {
        void OnSpawn();
        void OnDespawn();
    }
}
