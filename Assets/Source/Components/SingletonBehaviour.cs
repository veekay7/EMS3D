using UnityEngine;

/// <summary>
/// Inherit from this to force any MonoBehaviour to have one and only one instance in the scene.
/// Any duplicates with the same MonoBehaviour attached will automatically be destroyed.
/// </summary>
/// <typeparam name="T"></typeparam>
[DisallowMultipleComponent]
public class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T>
{
    // returns the singleton instance of this Game Controller
    public static T Instance { get; protected set; }

    // returns true if the Game Controller single instance is instantiated
    public static bool Instantiated { get => (Instance != null); }


    protected void Awake()
    {
        if (Instantiated && Instance != this)
        {
            Destroy(Instance.gameObject);
        }
        else
        {
            Instance = (T)this;
            AfterAwake();
        }
    }

    protected virtual void AfterAwake()
    {
        return;
    }

    protected virtual void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}