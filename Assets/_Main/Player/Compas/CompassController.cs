using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CompassController : MonoBehaviour
{
    [Inject] private CameraManager cameraManager;
    
    [SerializeField] private RectTransform markersParent;
    [SerializeField] private RectTransform[] targetMarkers;
    
    [field: SerializeField] public ECompasTarget Type { get; private set; }
    
    private List<Transform> notVisibleTargets = new ();
    private List<Transform> visibleTargets = new ();

    private Dictionary<Transform, RectTransform> targetMarkersDict = new ();
    private Queue<RectTransform> freeTargetMarkers = new ();

    private void Awake()
    {
        foreach (var marker in targetMarkers)
        {
            freeTargetMarkers.Enqueue(marker);   
        }
    }

    private void Update()
    {
        for (var i = 0; i < visibleTargets.Count; i++)
        {
            if (freeTargetMarkers.Count == 0)
            {
                break;
            }
            var target = visibleTargets[i];
            if (!IsVisible(target))
            {
                visibleTargets.RemoveAt(i);
                i--;
                notVisibleTargets.Add(target);
                
                var marker = freeTargetMarkers.Dequeue();
                targetMarkersDict[target] = marker;
            }
        }
        
        for (var i = 0; i < notVisibleTargets.Count; i++)
        {
            var target = notVisibleTargets[i];
            if (IsVisible(target))
            {
                notVisibleTargets.RemoveAt(i);
                i--;
                visibleTargets.Add(target);

                freeTargetMarkers.Enqueue(targetMarkersDict[target]);
                targetMarkersDict.Remove(target);
            }
        }
        
        foreach (var targetMarkerPair in targetMarkersDict)
        {
            var target = targetMarkerPair.Key;
            var marker = targetMarkerPair.Value;
            if (!marker.gameObject.activeSelf)
            {
                marker.gameObject.SetActive(true);
            }
            
            Vector2 vp = cameraManager.ActiveCamera.WorldToViewportPoint(target.position);
            vp.x = Mathf.Clamp(vp.x, 0.05f, 0.95f);
            vp.y = Mathf.Clamp(vp.y, 0.05f, 0.95f);

            var sp = new Vector2(vp.x * Screen.width, vp.y * Screen.height);
            
            RectTransformUtility.ScreenPointToLocalPointInRectangle(markersParent, sp, cameraManager.ActiveCamera, out Vector2 lp);

            marker.anchoredPosition = lp;
            var direction = (vp - Vector2.one / 2f) * 2f;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            var rot = Quaternion.AngleAxis(angle, Vector3.forward);
            marker.localRotation = rot;
        }
        
        foreach (var marker in freeTargetMarkers)
        {
            if (marker.gameObject.activeSelf)
            {
                marker.gameObject.SetActive(false);
            }
        }

    }

    public void AddTarget(Transform target)
    {
        visibleTargets.Add(target);
    }

    public void RemoveTarget(Transform target)
    {
        visibleTargets.Remove(target);
        if (notVisibleTargets.Remove(target))
        {
            var marker = targetMarkersDict[target];
            marker.gameObject.SetActive(false);
            freeTargetMarkers.Enqueue(marker);
            targetMarkersDict.Remove(target);
        }
    }

    private bool IsVisible(Transform target)
    {
        var sp = cameraManager.ActiveCamera.WorldToViewportPoint(target.position);
        return sp.x > 0 && sp.y > 0 && sp.x < 1 && sp.y < 1;
    }
    
}
