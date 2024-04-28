using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class WishProbabilityData
{
    public EWish type;
    public float weight;
}

[CreateAssetMenu(menuName = "Configs/GameConfig", fileName = "GameConfig")]
public class GameConfig : ScriptableObject
{
    public float playerSpeed = 5f;
    public Vector3 moveForward = Vector3.back;
    public Vector3 moveRight = Vector3.left;
    public float moneySpendTime;
    public Vector2 breakTimerMinMax;
    public float repairHouseTime;
    [Header("Wishes")] 
    public List<WishProbabilityData> wishChances;
    public float wanderDuration;
    public float chatDuration;
}
