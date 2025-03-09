using UnityEngine;

public abstract class WishPostProcessor : MonoBehaviour
{
    public abstract void OnUpdate();
    public abstract void Add(CitizenController processed);
    public abstract bool HasMorePlace();

    public abstract bool IsProcessing(CitizenController citizen);
}