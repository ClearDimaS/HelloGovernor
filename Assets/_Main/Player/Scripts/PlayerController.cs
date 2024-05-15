using System;
using DG.Tweening;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour, IMoneySpender, IRepairer, ICurrencyHolder
{
    [Inject] private PlayerDataRepository repository;
    [Inject] private CurrencyPool currencyPool;
    [Inject] private GameConfig gameConfig;
    [Inject] private PlayerInput playerInput;

    [SerializeField] private Rigidbody rb;

    private int frameRequiredDelta;

    private Vector3 delta;
    
    public Transform Root => transform;
    public void Spend(int diff)
    {
        repository.GetData().money -= diff;
    }

    public int MaxToSpend()
    {
        return repository.GetData().money;
    }

    private void Awake()
    {
        playerInput.moveEvent += RequireMove;
    }

    private void RequireMove(Vector3 delta)
    {
        this.delta = delta;
        frameRequiredDelta = Time.frameCount;
    }

    private void FixedUpdate()
    {
        if (Time.frameCount > frameRequiredDelta + 1)
        {
            return;
        }

        var dir = delta.normalized;
        rb.rotation = Quaternion.Lerp(rb.rotation, Quaternion.LookRotation(dir, Vector3.up), 1f);
        rb.MovePosition(rb.position + delta * Time.fixedDeltaTime * gameConfig.playerSpeed);
    }

    public void AddCurrency(int amount)
    {
        throw new NotImplementedException();
    }

    public void MoveCurrencyToMe(CurrencyBehaviour currency)
    {
        var startPos = currency.transform.position;
        var endPos = transform.position;
        var middlePos = (startPos + endPos) / 2f + Vector3.up * gameConfig.moneyGainFlyHeight;
        
        var startRot = currency.transform.rotation;
        var middleRot = Quaternion.Euler(Random.Range(0, 360f), Random.Range(0, 360f), Random.Range(0, 360f));
        var endRot = Quaternion.Euler(Random.Range(0, 360f), Random.Range(0, 360f), Random.Range(0, 360f));

        var startScale = currency.transform.localScale;
        currency.transform.DORotateQuaternion(middleRot, gameConfig.moneyFlyTime1).SetEase(Ease.InCubic);
        currency.transform.DOMove(middlePos, gameConfig.moneyFlyTime1).SetEase(Ease.InCubic).OnComplete(() =>
        {
            var t = 0f;
            DOTween.To(() => t, x => t = x, 1f, gameConfig.moneyFlyTime2).OnUpdate(() =>
            {
                currency.transform.rotation = Quaternion.Lerp(middleRot, endRot, t);
                currency.transform.position = Vector3.Lerp(middlePos, transform.position, t);
                currency.transform.localScale = startScale * Mathf.Sqrt(t);
            }).OnComplete(() =>
            {
                repository.GetData().money += currency.Amount;
                currencyPool.Pool(currency);
            }).SetEase(gameConfig.moneyFlyEase2);;
        }).SetEase(gameConfig.moneyFlyEase1);
    }
}