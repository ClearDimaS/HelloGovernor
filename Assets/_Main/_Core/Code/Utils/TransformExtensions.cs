using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtensions
{
     public static void DestroyAllChildren(this Transform transform, bool immediate = false)
     {
          if (Application.isPlaying && !immediate)
          {
               for (int i = transform.childCount - 1; i >= 0; i--)
               {
                    GameObject.Destroy(transform.GetChild(i).gameObject);
               }
          }
          else
          {
               for (int i = transform.childCount - 1; i >= 0; i--)
               {
                    GameObject.DestroyImmediate(transform.GetChild(i).gameObject);
               }
          }
     }
     
     public static List<T> GetComponentsInDirectChildren<T>(this Transform transform)
     {
          var components = new List<T>();
          for (int i = transform.childCount - 1; i >= 0; i--)
          {
               var component = transform.GetChild(i).GetComponent<T>();
               if (component != null)
               {
                    components.Add(component);
               }
          }
          return components;
     }
}
