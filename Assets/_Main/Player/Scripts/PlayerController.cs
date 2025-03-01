using System;
using DG.Tweening;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class PlayerController : Singleton<PlayerController>, 
    IMoneySpender, 
    IRepairer, 
    ICurrencyHolder, 
    IOperator, 
    IItemTaker,
    IWishAssistant
{
    [Inject] private SoundManager soundManager;
    [Inject] private PlayerDataRepository repository;
    [Inject] private CurrencyPool currencyPool;
    [Inject] private CurrencySingleStackPool currencyStackPool;
    [Inject] private GameConfig gameConfig;
    [Inject] private PlayerInput playerInput;

    [SerializeField] private Interactor interactor;
    [SerializeField] private Rigidbody rb;
    [field: SerializeField] public PlayerXPController XP_Controller { get; private set; }
    
    public IWishAssistant WishAssistant { get; private set; }
    private int frameRequiredDelta;

    private Vector3 spawnPlace;
    private Vector3 delta;
    
    public Transform Root => transform;
    public Transform TransformRoot => transform;

    public void Spend(int diff)
    {
        repository.Money -= diff;
    }

    public int MaxToSpend()
    {
        return repository.Money;
    }

    private void Awake()
    {
        WishAssistant = GetComponent<IWishAssistant>();
        playerInput.moveEvent += RequireMove;
    }

    private void Start()
    {
        spawnPlace = transform.position;
    }

    private void Update()
    {
        if (transform.position.y < -2f)
        {
            transform.position = spawnPlace;
            rb.velocity = Vector3.zero;
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
        var diff = endPos - startPos;
        diff.y = 0f;
        var rightDir = Vector3.Cross((diff).normalized, Vector3.up);
        var middlePos = (startPos + endPos) / 2f + Vector3.up * gameConfig.moneyGainFlyHeight + rightDir * Random.Range(-1f, 1f);
        
        var startRot = currency.transform.rotation;
        var middleRot = Quaternion.Euler(Random.Range(0, 360f), Random.Range(0, 360f), Random.Range(0, 360f));
        var endRot = Quaternion.Euler(Random.Range(0, 360f), Random.Range(0, 360f), Random.Range(0, 360f));

        var startScale = currency.transform.localScale;
        currency.transform.DORotateQuaternion(middleRot, gameConfig.moneyFlyTime1).SetEase(gameConfig.moneyFlyEase1);
        currency.transform.DOMove(middlePos, gameConfig.moneyFlyTime1).OnComplete(() =>
        {
            var t = 0f;
            soundManager.PlayerGetMoney();
            DOTween.To(() => t, x => t = x, 1f, gameConfig.moneyFlyTime2).OnUpdate(() =>
            {
                currency.transform.rotation = Quaternion.Lerp(middleRot, endRot, t);
                currency.transform.position = Vector3.Lerp(middlePos, transform.position, t);
                currency.transform.localScale = startScale * Mathf.Lerp(1f, 0.4f, t);
            }).OnComplete(() =>
            {
                repository.Money += currency.Amount;
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

    public bool CanAddItems()
    {
        return interactor.HasMorePlaceFor();
    }

    public void AddItem(GenericCitizenItem takeItem)
    {
        interactor.AddItem(takeItem);
    }

    public GenericCitizenItem RemoveItem()
    {
        return interactor.RemoveItem() as GenericCitizenItem;
    }
    
    public bool HasAnyItems()
    {
        return interactor.HasAnyItem();
    }

    public bool HasItemOfType(GenericCitizenItem prefab)
    {
        return interactor.HasItemOfType(prefab.GetData().key);
    }

    public void AddBoughtBuilding(UpgradableObject upgradableObject)
    {
        XP_Controller.AddXP(gameConfig.purchaseXP, upgradableObject.BuyPlace);
    }
}