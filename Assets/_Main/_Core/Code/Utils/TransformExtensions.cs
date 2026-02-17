using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class ObjectMoveSettings
{
     public AnimationCurve scaleCurve;
     public AnimationCurve toTargetCurve;
     public AnimationCurve heightCurve;
     public AnimationCurve extraRotationCurve;
     public float time = 0.5f;
     public float randomOffsetMagnitude;
     public Vector3 rotVec = new Vector3(1, 1, 1);
     public bool rotateWhenBounce;
}

public static class TransformExtensions
{
     public static float DotTo(this Transform source, Transform to)
     {
          var diff = to.position - source.position;
          diff.y = 0f;
          return Vector3.Dot(source.forward, diff);
     }
     public static Bounds GetBounds(this Transform transform)
     {
          var mrs = transform.GetComponentsInChildrenRecursiveUntilComponent<MeshRenderer, MeshRenderer>();
          var bounds = GetBoundsFromMRs(mrs.ToArray());

          return bounds;
     }

     public static bool TryGetBounds<T>(this Transform transform, out Bounds bounds) where T : MonoBehaviour
     {
          var mrs = new List<MeshRenderer>();
          transform.GetComponentsInChildrenRecursiveUntilComponent<MeshRenderer, T>(mrs, true);
          
          bounds = GetBoundsFromMRs(mrs.ToArray());
          return mrs.Count > 0;
     }

     private static Bounds GetBoundsFromMRs(MeshRenderer[] mrs)
     {
          // Find the first valid MeshRenderer to use for initialization
          MeshRenderer firstValid = null;
          foreach (var mr in mrs)
          {
               var mf = mr.GetComponent<MeshFilter>();
               if (mf != null)
               {
                    firstValid = mr;
                    break;
               }
          }

          // If no valid MeshRenderers, return empty bounds
          if (firstValid == null)
          {
               return new Bounds();
          }

          // Initialize bounds with the first valid MeshRenderer's bounds
          var bounds = new Bounds(firstValid.bounds.center, firstValid.bounds.size);

          // Now encapsulate the rest, starting from the next one
          bool isFirst = true;
          for (var i = 0; i < mrs.Length; i++)
          {
               var mr = mrs[i];
               var mf = mr.GetComponent<MeshFilter>();
               if (mf == null || (isFirst && mr == firstValid))
               {
                    isFirst = false;
                    continue;
               }
               if (mr.bounds.size.sqrMagnitude < 0.5f)
               {
                    continue;
               }
               bounds.Encapsulate(mr.bounds);
          }
    
          return bounds;
     }


     public static async UniTaskVoid LocalJump(this Transform root, float height)
     {
          var time = Mathf.Sqrt(2 * height / 9.8f);
          var startTime = Time.time;
          var token = EditorSafeCancellation.Instance.GetCancellationToken(root);
          while (Time.time - startTime < time)
          {
               var t = (Time.time - startTime) / time;
               root.localPosition = Vector3.up * height * Mathf.Sqrt(Mathf.PingPong(t * 2f, 1f));
               await UniTask.Yield(cancellationToken:token);
          }
          root.localPosition = Vector3.zero;

     }
     public static void SetDirty(this GameObject go)
     {
          #if UNITY_EDITOR
          UnityEditor.EditorUtility.SetDirty(go);
          #endif
     }
     
     public static void SetDirty<T>(this T go) where T: MonoBehaviour
     {
#if UNITY_EDITOR
          UnityEditor.EditorUtility.SetDirty(go);
          go.gameObject.SetDirty();
#endif
     }
     public static T GetNearest<T>(this IEnumerable<T> list, Vector3 from) where T : MonoBehaviour
     {
          return list.GetNearest(from, Mathf.Infinity);
     }
     
     public static T GetNearest<T>(this IEnumerable<T> list, Vector3 from, float radius) where T : MonoBehaviour
     {
          T nearest = null;
          var minDist = radius * radius;
          foreach (var el in list)
          {
               var diff = el.transform.position - from;
               diff.y = 0f;
               if (diff.sqrMagnitude < minDist)
               {
                    minDist = diff.sqrMagnitude;
                    nearest = el;
               }
          }

          return nearest;
     }
     
     public static async UniTask Fade(this CanvasGroup group, float time, float delay, float target)
     {
          var startTime = Time.time;
          var token = EditorSafeCancellation.Instance.GetCancellationToken(group);
          while (Time.time - startTime < delay)
          {
               await UniTask.Yield(cancellationToken:token);
          }

          startTime = Time.time;
          
          while (Time.time - startTime < time)
          {
               var t = (Time.time - startTime) / time;
               group.alpha = Mathf.Lerp(Mathf.Abs(1- target), target, t);
               await UniTask.Yield(cancellationToken:token);
          }
          if (target == null || !Application.isPlaying)
          {
               return;
          }
          group.alpha = target;
     }
     
     public static async UniTask RotateToLocalZero(this Transform target, float time, Quaternion offset)
     {
          var startTime = Time.time;
          var startRot = target.transform.localRotation;
          var token = EditorSafeCancellation.Instance.GetCancellationToken(target);
          while (Time.time - startTime < time)
          {
               var t = (Time.time - startTime) / time;
               if (target == null || !Application.isPlaying)
               {
                    return;
               }
               target.localRotation = Quaternion.Lerp(startRot, Quaternion.identity * offset, t);
               await UniTask.Yield(cancellationToken:token);
          }
          if (target == null || !Application.isPlaying)
          {
               return;
          }
          target.transform.localRotation = Quaternion.identity* offset;
     }
     
     public static async UniTask RotateTo(this Transform target, Quaternion offset, float time, PlayerLoopTiming timing = PlayerLoopTiming.Update)
     {
          var startTime = Time.time;
          var startRot = target.rotation;
          var token = EditorSafeCancellation.Instance.GetCancellationToken(target);
          while (Time.time - startTime < time)
          {
               var t = (Time.time - startTime) / time;
               if (target == null || !Application.isPlaying)
               {
                    return;
               }
               target.rotation = Quaternion.Lerp(startRot, offset, t);
               await UniTask.Yield(cancellationToken:token, timing: timing);
          }
          if (target == null || !Application.isPlaying)
          {
               return;
          }
          target.transform.rotation = offset;
     }
     
     public static async UniTask MoveToLocalZero(this Transform target, Vector3 offset, float time)
     {
          var startTime = Time.time;
          var startRot = target.transform.localPosition;
          var token = EditorSafeCancellation.Instance.GetCancellationToken(target);
          while (Time.time - startTime < time)
          {
               var t = (Time.time - startTime) / time;
               if (target == null || !Application.isPlaying)
               {
                    return;
               }
               target.localPosition = Vector3.Lerp(startRot, offset, t);
               await UniTask.Yield(cancellationToken:token);
          }

          if (target == null || !Application.isPlaying)
          {
               return;
          }
          target.localPosition = offset;
     }
     
     public static async UniTask MoveToWorldPos(this Transform target, Vector3 pos, float time)
     {
          var startTime = Time.time;
          var startRot = target.transform.position;
          var token = EditorSafeCancellation.Instance.GetCancellationToken(target);
          while (Time.time - startTime < time)
          {
               var t = (Time.time - startTime) / time;
               if (target == null || !Application.isPlaying)
               {
                    return;
               }
               target.position = Vector3.Lerp(startRot,  pos, t);
               await UniTask.Yield(cancellationToken:token);
          }
     }
     
     public static async UniTask ScaleTo(this Transform target, float scale, float time)
     {
          var startTime = Time.time;
          var startScale = target.localScale;
          var token = EditorSafeCancellation.Instance.GetCancellationToken(target);
          while (Time.time - startTime < time)
          {
               var t = (Time.time - startTime) / time;
               if (target == null || !Application.isPlaying)
               {
                    return;
               }
               target.localScale = Vector3.Lerp(startScale, scale * Vector3.one, t);
               await UniTask.Yield(cancellationToken:token);
               
          }
          if (target == null || !Application.isPlaying)
          {
               return;
          }
          target.localScale = scale * Vector3.one;
     }
     
     public static async UniTask ScaleTo(this Transform target, Vector3 scale, float time)
     {
          var startTime = Time.time;
          var startScale = target.localScale;
          var token = EditorSafeCancellation.Instance.GetCancellationToken(target);
          while (Time.time - startTime < time)
          {
               var t = (Time.time - startTime) / time;
               if (target == null || !Application.isPlaying)
               {
                    return;
               }
               target.localScale = Vector3.Lerp(startScale, scale, t);
               await UniTask.Yield(cancellationToken:token);
          }
          if (target == null || !Application.isPlaying)
          {
               return;
          }
          target.localScale = scale;
     }

     public static float EaseInOutCubic(float t) {
          // This function can be used to transform your 't' parameter
          t = Mathf.Clamp01(t); // Ensure t is between 0 and 1
          return t < 0.5 ? 4 * t * t * t : 1 - Mathf.Pow(-2 * t + 2, 3) / 2;
     }
     
     public static async UniTask MoveBounceWithSettings(this Transform target, Transform to, Vector3 offset, 
          ObjectMoveSettings moveSettings, int trajectoryIndex = 0)
     {
          var randomOffsetMagnitude = moveSettings.randomOffsetMagnitude;
          var time = moveSettings.time;
          var heightCurve = moveSettings.heightCurve;
          var scaleCurve = moveSettings.scaleCurve;
          var toTargetCurve = moveSettings.toTargetCurve;
          var extraRotCurve = moveSettings.extraRotationCurve;
          var rotVec = moveSettings.rotVec;
          var startTime = Time.time;
          var start = target.position;

          var diff = (target.position - GetEndPos());
          diff.y = 0f;

          var startRot = target.rotation.eulerAngles;
          var token = EditorSafeCancellation.Instance.GetCancellationToken(target);

          while (Time.time - startTime < time)
          {
               var t = (Time.time - startTime) / time;
               target.position = Vector3.Lerp(start, GetEndPos(), toTargetCurve.Evaluate(t)) + 
                                 heightCurve.Evaluate(t) * Vector3.up;
               
               var endRot = (to == null ? Vector3.zero : to.rotation.eulerAngles);
               var endRotFinal = endRot + rotVec * extraRotCurve.Evaluate(t);
               var rotIntermediate = target.rotation;
               var ownRot = target.rotation;
               
               if (moveSettings.rotateWhenBounce)
               {
                    ownRot *= Quaternion.AngleAxis(Time.deltaTime * 360f, Vector3.up) *
                              Quaternion.AngleAxis(Time.deltaTime * 360f, Vector3.right) *
                              Quaternion.AngleAxis(Time.deltaTime * 360f, Vector3.forward);
               }
               
               rotIntermediate.eulerAngles = Vector3.Lerp((ownRot.eulerAngles), Vector3.Lerp(startRot, endRotFinal, t), t);
               target.rotation = rotIntermediate;
               target.localScale = scaleCurve.Evaluate(t) * Vector3.one;
               
               await UniTask.Yield(cancellationToken:token);
               if (target == null || !Application.isPlaying)
               {
                    return;
               }
          }

          var endRot2 = (to == null ? Vector3.zero : to.rotation.eulerAngles);
          target.position = GetEndPos();
          var rot = target.rotation;
          rot.eulerAngles = endRot2 + rotVec * extraRotCurve.Evaluate(1f);
          target.rotation = rot;

          Vector3 GetEndPos()
          {
               if (to == null)
               {
                    return offset;
               }
               return to.position + offset;
          }
     }

     public static async UniTask MoveFromTo(this Transform target, Transform from, Transform to, float time)
     {
          var startTime = Time.time;

          var startLP = target.parent.InverseTransformPoint(from.position);
          var endLP = target.parent.InverseTransformPoint(to.position);
          var token = EditorSafeCancellation.Instance.GetCancellationToken(target);
          while (Time.time - startTime < time)
          {
               var t = (Time.time - startTime) / time;
               target.localPosition = Vector3.Lerp(startLP, endLP, t);
               
               await UniTask.Yield(cancellationToken:token);
          }

          target.localPosition = endLP;
     }
     
     public static async UniTask RotateTo(this Transform root, Transform target, float time)
     {
          var startTime = Time.time;
          var token = EditorSafeCancellation.Instance.GetCancellationToken(target);
          while (Time.time - startTime < time)
          {
               var dir = (target.position - root.position).normalized;
               
               var t = (Time.time - startTime) / time;
               root.rotation = Quaternion.Lerp(root.rotation, Quaternion.LookRotation(dir, Vector3.up), Time.deltaTime * 50 * t);
               
               await UniTask.Yield(cancellationToken:token);
          }
     }
     
     public static async UniTask BounceScale(this Transform target, float maxScale = 1.1f, float endScale = 1f, float time = 0.4f)
     {
          var startTime = Time.time;
          var token = EditorSafeCancellation.Instance.GetCancellationToken(target);
          while (Time.time - startTime < time)
          {
               if (target == null || !Application.isPlaying)
               {
                    return;
               }
               var t = (Time.time - startTime) / (time);
               if (t < 0.5f)
               {
                    target.transform.localScale = Vector3.Lerp(Vector3.one, maxScale * Vector3.one, (t / 0.5f));    
               }
               else
               {
                    target.transform.localScale = Vector3.Lerp(maxScale * Vector3.one, Vector3.one * endScale, ((t-0.5f)/0.5f));
               }
               await UniTask.Yield(cancellationToken:token);
          }
          if (target == null || !Application.isPlaying)
          {
               return;
          }
          target.transform.localScale = Vector3.one * endScale;
     }
     
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
     
     public static void DestroyAllChildComponents<T>(this Transform transform) where T : Component
     {
          var children = transform.GetComponentsInChildren<T>(true);
          foreach (var child in children)
          {
               GameObject.DestroyImmediate(child);
          }
     }

     public static T GetComponentInDirectChildren<T>(this Component c)
     {
          var transform = c.transform; 
          for (int i = transform.childCount - 1; i >= 0; i--)
          {
               var component = transform.GetChild(i).GetComponent<T>();
               if (component != null)
               {
                    return component;
               }
          }
          return default;
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
     
     public static List<T> GetComponentsInDirectChildren<T>(this Transform transform, int maxDepth)
     {
          var components = new List<T>();
          for (int i = transform.childCount - 1; i >= 0; i--)
          {
               var component = transform.GetChild(i).GetComponent<T>();
               if (component != null)
               {
                    components.Add(component);
               }

               var newDepth = maxDepth - 1;
               if (newDepth > 0)
               {
                    var fromChild = GetComponentsInDirectChildren<T>(transform.GetChild(i), newDepth);
                    components.AddRange(fromChild);
               }
          }
          return components;
     }
     
     public static List<T> GetComponentsInChildrenRecursiveUntilComponent<T, U>(this Transform transform)
          where T : Component
          where U : Component
     {
          var components = new List<T>();
          GetComponentsInChildrenRecursiveUntilComponent<T, U>(transform, components, true);
          return components;
     }

     private static void GetComponentsInChildrenRecursiveUntilComponent<T, U>(this Transform transform,
          List<T> components, bool ignoreThisOne) 
          where T : Component
          where U : Component
     {
          for (int i = 0; i < transform.childCount; i++)
          {
               var child = transform.GetChild(i);
               if (child.GetComponent<U>() != null && !ignoreThisOne)
               {
                    if(typeof(T) == typeof(U))
                    {
                         components.Add(child.GetComponent<T>());
                    }
                    continue;
               }
               var component = child.GetComponent<T>();
               if (component != null)
               {
                    components.Add(component);
               }

               GetComponentsInChildrenRecursiveUntilComponent<T, U>(child, components, false);
          }
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
          var canvas = rectTransform.GetComponentsInParent<Canvas>(true).Last();
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
     
     public static string GetHierarchyStringIndexes(this Transform transform)
     {
          var hierarchyString = transform.GetIndexString();
          var parent = transform.parent;
          while (parent != null)
          {
               var indexString = parent.GetIndexString();
               hierarchyString = $"{indexString}/{hierarchyString}";
               parent = parent.parent;
          }

          return hierarchyString;
     }

     private static string GetIndexString(this Transform transform)
     {
          var indexString = string.Empty;
          var index = transform.GetSiblingIndex();
          if (index < 10)
          {
               indexString = $"0{index}";
          }
          else
          {
               indexString = index.ToString();
          }

          if (index > 99)
          {
               Debug.LogError($"format error possible! {index} but only 2 symbols supported");
          }

          return indexString;
     }
}
