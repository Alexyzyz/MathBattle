using UnityEngine;

/// <summary>
/// Overrides the current instance instead of destroying it.
/// </summary>
public abstract class StaticInstance<T> : MonoBehaviour where T : MonoBehaviour {
    public static T Instance { get; private set; }
    protected virtual void Awake() => Instance = this as T;

    protected virtual void OnApplicationQuit() {
        Instance = null;
        Destroy(gameObject);
    }
}

/// <summary>
/// Keeps the original instance by destroying any new ones.
/// </summary>
public abstract class Singleton<T> : StaticInstance<T> where T : MonoBehaviour {
    protected override void Awake() {
    if (Instance != null) {
      Destroy(gameObject);
      return;
    }
    base.Awake();
    }
}

/// <summary>
/// Singleton variant that persists through scene loads.
/// </summary>
public abstract class PersistentSingleton<T> : Singleton<T> where T : MonoBehaviour {
    protected override void Awake() {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
}

