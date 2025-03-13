using UnityEngine;

public class OnVisibilityChangeNotifier : MonoBehaviour
{
    public bool isVisible { get; protected set; }
    
    private void OnBecameVisible()
    {
        isVisible = true;
    }
    
    private void OnBecameInvisible()
    {
        isVisible = false;
    }
}