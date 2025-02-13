using System.Collections;
using DG.Tweening;
using UnityEngine;


public abstract class CitizenItem : MonoBehaviour
{
    public abstract void PoolPlease();
    public abstract ItemConfigData GetData();
}