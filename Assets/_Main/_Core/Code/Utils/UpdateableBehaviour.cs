using UnityEngine;

public abstract class UpdateableBehaviour : MonoBehaviour
{
    private void Awake()
    {
        OnAwake();
    }

    private void OnDestroy()
    {
        if (UpdateCallManager.UnsafeInstance != null)
        {
            UpdateCallManager.UnsafeInstance.RemoveUpdatable(this);   
        }
    }

    private void OnEnable()
    {
        UpdateCallManager.Instance.AddUpdatable(this); 

        OnOnEnable();
    }

    protected virtual void OnOnEnable()
    {
        
    }

    private void OnDisable()
    {
        if (UpdateCallManager.UnsafeInstance != null)
        {
            UpdateCallManager.UnsafeInstance.RemoveUpdatable(this);   
        }

        OnOnDisable();
    }
    
    protected virtual void OnOnDisable()
    {
        
    }

    protected virtual void OnAwake()
    {
        
    }
    

    public virtual void UpdateCall(float deltaTime)
    {
        
    }

    public virtual void LateUpdateCall(float deltaTime)
    {
        
    }
}