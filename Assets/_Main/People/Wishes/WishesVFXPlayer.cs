using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WishesVFXPlayer : MonoBehaviour
{
    [SerializeField] private ParticleSystem pausedVFX;
    [SerializeField] private ParticleSystem[] happyPS;
    
    private WishesController wishesController;

    private void Awake()
    {
        wishesController = GetComponentInParent<WishesController>();
        wishesController.SubscribeWishesResult(PlayWishResultFVX);
    }

    private void Update()
    {
        var pause = wishesController.IsPaused;
        if (pause != pausedVFX.gameObject.activeSelf)
        {
            if (pause)
            {
                pausedVFX.gameObject.SetActive(true);
                pausedVFX.Play();
            }
            else
            {
                pausedVFX.gameObject.SetActive(false);
            }
        }
    }

    private void PlayWishResultFVX(bool isSuccess)
    {
        happyPS[Random.Range(0, happyPS.Length)].Play();
    }
}
