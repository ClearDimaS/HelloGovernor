using UnityEngine;

public class WishPostProcessorAnimator : MonoBehaviour
{
    [SerializeField] private Transform root;
    [SerializeField] private Transform[] points;
    [SerializeField] private float time = 1f;
    [SerializeField] private bool changeRotationFWD;
    protected float t = 0f;
    
    public void Move()
    {
        t += Time.deltaTime / time;
        var tPingPong = Mathf.PingPong(t, 0.5f) * 2;
        int segmentIndex = Mathf.FloorToInt(tPingPong * (points.Length - 1)) % (points.Length - 1);

        // Calculate the position between two points
        Vector3 startPoint = points[segmentIndex].position;
        Vector3 endPoint = points[segmentIndex + 1].position;
        
        // Interpolate between start and end points
        var newPos = Vector3.Lerp(startPoint, endPoint, tPingPong);
        if (changeRotationFWD)
        {
            var diff = newPos - root.position;
            var dir = diff.normalized;
            root.rotation = Quaternion.LookRotation(dir);
        }
        root.position = newPos;
    }

    public void StopMove()
    {
        if (points.Length == 0)
            return; // No points to move to

        // Move to the first point
        root.position = points[0].position;
        t = 0f; // Reset time
    }
}