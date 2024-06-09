using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WishesVFXPlayer : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] pausedVFXs;
    [SerializeField] private ParticleSystem[] happyPS;

    private bool wasPaused = true;
    private WishesController wishesController;

    private void Awake()
    {
        wishesController = GetComponentInParent<WishesController>();
        wishesController.SubscribeWishesResult(PlayWishResultFVX);
    }

    private void Update()
    {
        var pause = wishesController.IsPaused;
        if (wasPaused != pause)
        {
            wasPaused = pause;
            var playIndex = Random.Range(0, pausedVFXs.Length);
            for (var i = 0; i < pausedVFXs.Length; i++)
            {
                var pausedVFX = pausedVFXs[i];
                if (pause && i == playIndex)
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
    }

    private void PlayWishResultFVX(bool isSuccess)
    {
        happyPS[Random.Range(0, happyPS.Length)].Play();
    }
}
