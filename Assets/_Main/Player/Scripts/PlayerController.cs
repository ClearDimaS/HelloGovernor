using System;
using DG.Tweening;
using UnityEngine;
using Zenject;

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
        var start = currency.transform.position;
        var end = transform.position;
        var middle = (start + end) / 2f + Vector3.up * 0.6f;
        currency.transform.DOMove(middle, 0.3f).SetEase(Ease.InCubic).OnComplete(() =>
        {
            var t = 0f;
            DOTween.To(() => t, x => t = x, 1f, 0.3f).OnUpdate(() =>
            {
                currency.transform.position = Vector3.Lerp(middle, transform.position, t);
                currency.transform.localScale = Vector3.one * t;
            }).OnComplete(() =>
            {
                repository.GetData().money += currency.Amount;
                currencyPool.Pool(currency);
            });
        });
    }
}