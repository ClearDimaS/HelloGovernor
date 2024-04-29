using System;
using System.Collections.Generic;
using UnityEngine;

public enum EInteractable
{
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
}