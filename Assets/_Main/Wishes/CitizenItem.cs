using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;


public abstract class CitizenItem : MonoBehaviour
{
    public string secondaryKey;
    
    public abstract void PoolPlease();
    public abstract ItemConfigData GetData();

    public abstract void PoolPleaseAtTimeout(Action action);
}