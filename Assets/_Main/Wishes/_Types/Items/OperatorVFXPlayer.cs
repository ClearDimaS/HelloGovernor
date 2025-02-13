using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class OperatorVFXPlayer : MonoBehaviour
{
    [SerializeField] protected ParticleSystem[] upgradePS;

    private Animator assitantAnimator;
    private UpgradableOperator upgradable;

    private void Awake()
    {
        upgradable = GetComponentInParent<UpgradableOperator>();
        assitantAnimator = upgradable.GetComponentInChildren<Animator>();
        
        upgradable.SubscribeUpgrade(PlayUpgradeVFX);
    }

    private void Update()
    {
        foreach (var ps in upgradePS)
        {
            ps.transform.position = assitantAnimator.transform.position;
        }
    }

    private void PlayUpgradeVFX()
    {
        var rot = assitantAnimator.transform.rotation;
        assitantAnimator.transform.DORotateQuaternion(rot * Quaternion.AngleAxis(360f, Vector3.up), 0.3f);
        foreach (var ps in upgradePS)
        {
            ps.Play();
        }
    }
}
