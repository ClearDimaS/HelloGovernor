using System;
using DG.Tweening;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class PlayerController : Singleton<PlayerController>, 
    IMoneySpender, 
    IRepairer, 
    ICurrencyHolder, 
    IThiefBuster,
    IOperator, 
    IItemTaker
{
    [Inject] private PlayerDataRepository repository;
    [Inject] private CurrencyPool currencyPool;
    [Inject] private CurrencySingleStackPool currencyStackPool;
    [Inject] private GameConfig gameConfig;
    [Inject] private PlayerInput playerInput;

    [SerializeField] private Interactor interactor;
    [SerializeField] private Rigidbody rb;

    public IWishAssistant WishAssistant { get; private set; }
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
        WishAssistant = GetComponent<IWishAssistant>();
        playerInput.moveEvent += RequireMove;
    }

    private void Update()
    {
        if (Mathf.Abs(transform.position.y) > 0.2f)
        {
            var pos = transform.position;
            pos.y = 0;
            transform.position = pos;
        }
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
        currency.transform.SetParent(null, true);
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
            SoundManager.Instance.PlayerGetMoney();
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

    public void ReturnMoney(int stolenAmount, Transform from)
    {
        var reward = stolenAmount;
        var currency = currencyStackPool.GetElement();
        currency.transform.position = from.position + Vector3.up + new Vector3(1f, 0, 1f).AxisToRandomDir();
        currency.Initialize(reward);
        currency.AddForce(Vector3.up * 3 + new Vector3(1f, 0, 1f).AxisToRandomDir() * 2);
    }

    public bool CanAddItems(ItemsWishGranter granter)
    {
        return interactor.HasMorePlaceFor(granter);
    }

    public void AddItem(WishAssistantItem takeItem)
    {
        interactor.AddItem(takeItem);
    }
}