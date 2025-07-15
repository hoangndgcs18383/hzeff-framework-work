using UnityEngine;

internal static class InitializeOnLoad
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeApplication()
    {
        GameObject[] prefabs = Resources.LoadAll<GameObject>("InitializeOnLoad");

        if (prefabs.Length > 0)
        {
            foreach (var prefab in prefabs)
            {
                GameObject gameObject = Object.Instantiate(prefab);
                gameObject.name = prefab.name;
                Object.DontDestroyOnLoad(gameObject);
            }
        }
    }
}