using UnityEngine;

public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviourSingleton<T>
{
    public static T Instance { get; private set; } = GetInstance();
    private static object _lock = new();
    private static bool _isDestroyed;
    private static T _instance;

    private static T GetInstance()
    {
        _lock ??= new object();

        lock (_lock)
        {
            if (_isDestroyed)
            {
                Debug.Log($"Singleton {typeof(T)} is Destroyed");
                return null;
            }

            if (_instance == null)
            {
                var singletonObject = new GameObject(typeof(T).ToString());
                _instance = singletonObject.AddComponent<T>();
                DontDestroyOnLoad(singletonObject);
            }

            return _instance;
        }
    }

    private void OnApplicationQuit()
    {
        OnDestroy();
    }

    private void OnDestroy()
    {
        _isDestroyed = true;
        _instance = null;
    }
}