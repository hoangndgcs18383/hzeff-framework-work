namespace SAGE.Framework.Extensions
{
    using UnityEngine;

    public class BehaviorSingleton<T> : MonoBehaviour where T : BehaviorSingleton<T>
    {
        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = (T)this;
                DontDestroyOnLoad(gameObject); // Optional
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    public class Singleton<T> where T : Singleton<T>
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = System.Activator.CreateInstance<T>();
                    _instance.Initialize();
                }
                return _instance;
            }
            set => _instance = value;
        }

        public virtual void Initialize() {}
    }
}