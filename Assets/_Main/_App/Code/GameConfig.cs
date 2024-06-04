using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
    public int startMoney;
    public float moneySpendDelayAfterInput = 0.3f;
    public float buyTime;
    public float moneySpendPause;
    public float moneySpendFlyHeight;
    public float moneyGainFlyHeight;
    public float moneyFromStackPause = 0.03f;
    public float moneyFromStackMaxTime = 1.3f;
    public float moneyFlyTime1;
    public float moneyFlyTime2;
    public Ease moneyFlyEase1;
    public Ease moneyFlyEase2;
    public int moneyInOneModel = 3;

    [Header("Camera")] 
    public float hintCameraDistanceMult = 0.8f;
    public float unlockCameraDelay = 1f;
    public float unlockCameraTimer = 3f;
    public float cameraTransitionMaxTime = 1f;
    public float cameraTransitionSpeed = 10f;
    public Ease cameraTransitionEase;
    public Vector2 cameraUnlockXBorders;
    public Vector2 cameraUnlockYBorders;
    
    [Header("Break house")]
    public Vector2 breakTimerMinMax;
    public float repairHouseTime;

    [Header("Thief")]
    public float thiefPause = 10;
    public int thiefMaxSteal;
    public int moneyToSteal = 10;
}