using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    private static T instance;
    public static T Instance
    {
        get 
        {                 
            if (instance == null)
                instance =FindObjectOfType<T>();
            if(instance == null)
            {
                GameObject gameObject = new GameObject("Controller");
                instance = gameObject.AddComponent<T>();
            }
            return instance; 
        }
    }
    protected virtual void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance =this as T;
    }

    public static bool IsIntialized
    {
        get { return instance != null; }
    }

    protected virtual void OnDestory()
    {
        if(instance == this)
        {
            instance = null;
        }
    }
}
