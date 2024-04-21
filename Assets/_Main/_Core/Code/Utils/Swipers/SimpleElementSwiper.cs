using UnityEngine;
using Zenject;

public interface ICameraManager
{
    public Camera ActiveCamera { get; }
}

public class SimpleElementSwiper : ElementsSwiper
{
    [Inject] private ICameraManager cameraManager;

    [SerializeField] private float elementSize;

    private Vector3 startPressOnPlanePosition;
    private Vector3 startPlanePosition;
    private Transform MoveTransform => elementsParent;
 
    protected override Vector3 ParentPosition => elementsParent.localPosition;

    private Transform stableParent;
    protected Camera cam => cameraManager.ActiveCamera;

    protected override float ElementSize => elementSize;

    protected override void Initialize()
    {
        stableParent = MoveTransform.parent;
    }

    protected override void InitializeStart()
    {
        
    }

    public void OnMouseDown()
    {
        if (isBlocked)
            return;
        IsDragging = true;

        startPlanePosition = MoveTransform.position;
        var ray = cam.ScreenPointToRay(Input.mousePosition);
        MathUtils.TryIntersectPlaneWithRay(stableParent.position, stableParent.forward, ray, out startPressOnPlanePosition);
    }

    public void OnMouseDrag()
    {
        if (isBlocked)
            return;
        var currentScreenPostion = Input.mousePosition;
        var ray = cam.ScreenPointToRay(currentScreenPostion);
        MathUtils.TryIntersectPlaneWithRay(stableParent.position, stableParent.forward, ray, out Vector3 positionOnThePlane);
        var diff = positionOnThePlane - startPressOnPlanePosition;

        var localPosition = stableParent.InverseTransformPoint(startPlanePosition + diff);

        var pos = MoveTransform.localPosition;
        pos = new Vector3(Mathf.Clamp(localPosition.x, GetParentLocation(elements.Count).x, GetParentLocation(0).x), pos.y, pos.z);
        MoveTransform.localPosition = pos;
    }
    
    public void OnMouseUp()
    {
        IsDragging = false;
        MoveToClosestElement();
    }

    protected override Vector3 GetParentLocation(int i)
    {
        return -Vector3.right * (elementSize * i + spacing * i);
    }

    protected override void SetParentPosition(Vector3 position)
    {
        elementsParent.localPosition = position;
    }

    protected override Vector3 GetElementPosition(int i)
    {
        return Vector3.right * (elementSize * i + spacing * i);
    }

    protected override void SetItemPosition(int i)
    {
        elements[i].localPosition = GetElementPosition(i);
    }

    protected override void ApplyOffset(Transform transform, Vector3 vector3)
    {
        transform.localPosition += vector3;
    }
}