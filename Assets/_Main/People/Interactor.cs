using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum EInteractable
{
    None,
    Drink,
    Flowers,
    IceCream
}
public class Interactor : MonoBehaviour
{
    [Serializable]
    public class InteractableSerializedData
    {
        public EInteractable type;
        public GameObject go;
    }

    [SerializeField] private List<InteractableSerializedData> interactables;


    private void Start()
    {
        foreach (var interactable in interactables)
        {
            interactable.go.SetActive(false);
        }
    }

    public void ActivateObject(EInteractable type)
    {
        foreach (var interactable in interactables)
        {
            if (interactable.type == type)
            {
                interactable.go.SetActive(true);
            }
        }
    }
    
    public void DeactivateObject(EInteractable type)
    {
        foreach (var interactable in interactables)
        {
            if (interactable.type == type)
            {
                interactable.go.SetActive(false);
            }
        }
    }

    public bool HasItem(EWish type)
    {
        var interactableType = type.ToInteractable();
        switch (interactableType)
        {
            case EInteractable.None:
                return false;
            default:
                 return interactables.First(x => x.type == interactableType).go.activeSelf;
        }
    }
}