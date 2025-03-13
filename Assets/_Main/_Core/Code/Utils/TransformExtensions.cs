using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

     public static Vector3 AxisToRandomDir(this Vector3 axis)
     {
          return new Vector3(
               Random.Range(-1f, 1f) * axis.x,
               Random.Range(-1f, 1f) * axis.y,
               Random.Range(-1f, 1f) * axis.z).normalized;
     }

     public static void FillParent(this RectTransform rect, float value, bool horizontal = true)
     {
          rect.anchorMin = Vector2.zero;
          if (horizontal)
          {
               rect.anchorMax = new Vector2(value, 1f);    
          }
          else
          {
               rect.anchorMax = new Vector2(1f, value);
          }
          rect.sizeDelta = Vector2.zero;
     }

     public static Camera GetRenderCamera(this RectTransform rectTransform)
     {
          var canvas = rectTransform.GetComponentsInParent<Canvas>().Last();
          return canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
     }

     public static string GetHierarchyString(this Transform transform)
     {
          var hierarchyString = transform.name;
          var parent = transform.parent;
          while (parent != null)
          {
               hierarchyString = $"{parent.name}/{hierarchyString}";
               parent = parent.parent;
          }

          return hierarchyString;
     }
}
