using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class EditorSafeCancellation : Singleton<EditorSafeCancellation>
{
    private Dictionary<GameObject, CancellationTokenSource> ctsGOs = new();
    private Dictionary<Component, CancellationTokenSource> ctsComponents = new();
    protected override void OnCreated()
    {
        base.OnCreated();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeChanged;
        void OnPlayModeChanged(UnityEditor.PlayModeStateChange state)
        {
            if (state == UnityEditor.PlayModeStateChange.ExitingPlayMode)
            {
                UnityEditor.EditorApplication.playModeStateChanged -= OnPlayModeChanged;
                ctsGOs.Clear();
                ctsComponents.Clear();
            }
        }
        ctsComponents.Clear();
        ctsGOs.Clear();
#endif
    }

    public CancellationToken GetCancellationToken(GameObject go)
    {
        var token = go.GetCancellationTokenOnDestroy();

        
#if UNITY_EDITOR
        if (!ctsGOs.ContainsKey(go))
        {
            ctsGOs[go] = CancellationTokenSource.CreateLinkedTokenSource(
                token,
                this.destroyCancellationToken
            );
        }
        return ctsGOs[go].Token;
# else
        return token;
#endif
    }
    
    public CancellationToken GetCancellationToken(Component component)
    {
        var token = component.GetCancellationTokenOnDestroy();
        
#if UNITY_EDITOR
        if (!ctsComponents.ContainsKey(component))
        {
            ctsComponents[component] = CancellationTokenSource.CreateLinkedTokenSource(
                token,
                this.destroyCancellationToken
            );
        }
        return ctsComponents[component].Token;
# else
        return token;
#endif
    }
}