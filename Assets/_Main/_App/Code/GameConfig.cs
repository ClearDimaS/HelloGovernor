using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Configs/GameConfig", fileName = "GameConfig")]
public class GameConfig : ScriptableObject
{
    [Header("Player")]
    public float playerSpeed = 5f;
    public Vector3 moveForward = Vector3.back;
    public Vector3 moveRight = Vector3.left;
    
    [Header("Money")]
    public float moneySpendTime;
    public int moneyInOneModel = 3;
    
    [Header("Break house")]
    public Vector2 breakTimerMinMax;
    public float repairHouseTime;

    [Header("Thief")]
    public float thiefPause = 10;
    public int thiefMaxSteal;
    public int moneyToSteal = 10;
}