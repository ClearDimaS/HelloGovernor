using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WishesVFXPlayer : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] happyPS;
    
    private WishesController wishesController;

    private void Awake()
    {
        wishesController = GetComponentInParent<WishesController>();
        wishesController.SubscribeWishesResult(PlayWishResultFVX);
    }

    private void PlayWishResultFVX(bool isSuccess)
    {
        happyPS[Random.Range(0, happyPS.Length)].Play();
    }
}
