using Cysharp.Threading.Tasks;
using UnityEngine;

public static class UniTaskExtensions
{
    public static void AddTo(this UniTask task, GameObject go)
    {
        task.AttachExternalCancellation(go.GetCancellationTokenOnDestroy());
    }
    
    public static void AddTo(this UniTask task, Component component)
    {
        task.AttachExternalCancellation(component.GetCancellationTokenOnDestroy());
    }
}