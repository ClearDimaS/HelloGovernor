using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FashionBuilding : MonoBehaviour
{
    [SerializeField] private SkinChooser skinChooser;
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger || other.attachedRigidbody == null)
        {
            return;
        }

        if(!other.attachedRigidbody.TryGetComponent<PlayerController>(out var player))
        {
            return;
        }
        skinChooser.Show();
    }
}
