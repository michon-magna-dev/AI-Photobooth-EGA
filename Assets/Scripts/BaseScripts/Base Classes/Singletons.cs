using UnityEngine;

public class Singletons<T> : MonoBehaviour where T:MonoBehaviour
{
    public static T Singleton;
    public virtual void Awake()
    {
        if (Singleton == null)
            Singleton = this as T;
        else
            Destroy(gameObject);
    }
}
