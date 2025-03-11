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
    public int purchaseXP = 5;
    public PlayerLevelData[] levelUps;

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
    public int moneyRewardMaxModels = 10;
    public float moneyToStackTime = 1f;
    public Ease moneyToStackEase = Ease.InOutCirc;
    
    [Header("Camera")] 
    public float hintCameraDistanceMult = 0.8f;
    public float unlockCameraDelay = 1f;
    public float cameraTransitionMaxTime = 1f;
    public float cameraTransitionSpeed = 10f;
    public Ease cameraTransitionEase;
    public Vector2 cameraUnlockXBorders;
    public Vector2 cameraUnlockYBorders;
    [Header("Price")]
    public float assistantPricesMult = 0.5f;
    public float upgradeIncomePriceMult = 0.5f;
    public float incomeIncrease = 1.5f;
    [Header("Progression")]
    public int skinsPerLevel = 10;
}