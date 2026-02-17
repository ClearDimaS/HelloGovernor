using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public enum ERectMaskAxis
{
     X_Negative,
     X_Positive,
     Y_Positive,
     Y_Negative
          
}

public static class RectTransformExtensions
{
     public static async UniTaskVoid FillTo(this RectMask2D rectMask2D, ERectMaskAxis axis, float end, float time)
     {
          var size = rectMask2D.rectTransform.rect.width;
          if (axis == ERectMaskAxis.Y_Negative || axis == ERectMaskAxis.Y_Positive)
          {
               size = rectMask2D.rectTransform.rect.height;
          }
          var start = 1f-rectMask2D.GetPadding(axis)/size;
          var t = 0f;
          
          rectMask2D.SetPadding(axis, size * (1f - start));
          
          var startTime = Time.time;

          while (Time.time - startTime < time)
          {
               t = (Time.time - startTime) / time;
               rectMask2D.SetPadding(axis, size * (1f - Mathf.Lerp(start, end, t)));
               await UniTask.Yield();
          }

          rectMask2D.SetPadding(axis, size * (1f - end));
     }

     public static float GetPadding(this RectMask2D rectMask2D, ERectMaskAxis axis)
     {
          var padding = rectMask2D.padding;
          return axis switch
          {
               ERectMaskAxis.X_Negative => padding.x,
               ERectMaskAxis.X_Positive => padding.z,
               ERectMaskAxis.Y_Positive => padding.w,
               ERectMaskAxis.Y_Negative => padding.y,
               _ => throw new NotImplementedException($"axis: {axis} not setup!")
          };
     }
     
     public static void SetPadding(this RectMask2D rectMask2D, ERectMaskAxis axis, float value)
     {
          var padding = rectMask2D.padding;
          switch(axis)
          {
               case ERectMaskAxis.X_Negative:
                    padding.x = value;
                    break;
               case ERectMaskAxis.X_Positive:
                    padding.z = value;
                    break;
               case ERectMaskAxis.Y_Positive:
                    padding.w = value;
                    break;
               case ERectMaskAxis.Y_Negative:
                    padding.y = value;
                    break;
               default:
                    throw new NotImplementedException($"axis: {axis} not setup!");
          };
          rectMask2D.padding = padding;
     }

     public static Camera GetCamera(this Canvas canvas)
     {
          return canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
     }
     
     public static void CoverRectTransform(this RectTransform target, RectTransform with, Canvas canvas, Camera worldCamera)
     {
          if (with == null || target == null || canvas == null)
          {
               Debug.LogError("Source, target, or canvas is null.");
               return;
          }

          // Get the corners of the target RectTransform in world space
          Vector3[] corners = new Vector3[4];
          target.GetWorldCorners(corners);

          var camera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
          worldCamera = null;
          
          // Convert world space corners to the local space of the source's parent
          Vector2[] localCorners = new Vector2[4];
          RectTransformUtility.ScreenPointToLocalPointInRectangle(
               with.parent as RectTransform,
               RectTransformUtility.WorldToScreenPoint(worldCamera, corners[0]),
               camera,
               out localCorners[0]
          );
          RectTransformUtility.ScreenPointToLocalPointInRectangle(
               with.parent as RectTransform,
               RectTransformUtility.WorldToScreenPoint(worldCamera, corners[2]),
               camera,
               out localCorners[2]
          );

          // Calculate the new position and size for the source RectTransform
          Vector2 newPosition = (localCorners[0] + localCorners[2]) / 2f;
          Vector2 newSize = new Vector2(
               Mathf.Abs(localCorners[2].x - localCorners[0].x),
               Mathf.Abs(localCorners[2].y - localCorners[0].y)
          );

          // Apply the new values
          with.localPosition = newPosition;
          with.sizeDelta = newSize;

          // Ensure pivot and anchors are set correctly for sizeDelta to work as expected (e.g., center aligned)
          with.anchorMin = new Vector2(0.5f, 0.5f);
          with.anchorMax = new Vector2(0.5f, 0.5f);
          with.pivot = new Vector2(0.5f, 0.5f);
     }
}