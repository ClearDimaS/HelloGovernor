using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtensions
{
    public static void SetGameLayerRecursive(this GameObject _go, int _layer)
    {
        _go.layer = _layer;
        foreach (Transform child in _go.transform)
        {
            child.gameObject.layer = _layer;
 
            Transform _HasChildren = child.GetComponentInChildren<Transform>();
            if (_HasChildren != null)
                SetGameLayerRecursive(child.gameObject, _layer);
             
        }
    }

    public static void SetActiveOnce(this GameObject[] gos, bool state)
    {
        foreach (var go in gos)
        {
            SetActiveOnce(go, state);
        }
    }
    
    public static void SetActiveOnce(this List<GameObject> gos, bool state)
    {
        foreach (var go in gos)
        {
            SetActiveOnce(go, state);
        }
    }
    
    public static void SetActiveOnce(this GameObject go, bool state)
    {
        if (go.activeSelf != state)
        {
            go.SetActive(state);
        }
    }
}

public static class Vector2Extensions
{
    public static Vector2 SP_To_VP(this Vector2 sp)
    {
        return Vector2.Scale(sp, new Vector2(1f / Screen.width, 1f / Screen.height));
    }
}
