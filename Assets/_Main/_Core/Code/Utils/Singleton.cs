using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                CreateInstance();
            }
            return instance;
        }
    }

    private static T instance;

    private void Awake()
    {
        if (instance == null)
        {
            CreateInstance();
        }
        else if(instance != this)
        {
            Destroy(gameObject);
            return;
        }
        OnCreated();
    }
    
    private static void CreateInstance()
    {
        instance = GameObject.FindObjectOfType<T>(true);
        if (instance == null)
        {
            var sceneContext = FindObjectOfType<SceneContext>();
            var newGO = new GameObject($"[{typeof(T).Name}]");
            instance = sceneContext.Container.InstantiateComponent<T>(newGO);
        }
    }
    
    protected virtual void OnCreated()
    {
        
    }
}
