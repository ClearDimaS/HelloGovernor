using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class AddedMoneyDisplayer : MonoBehaviour
{
    [Inject] private PlayerDataRepository playerRepository;
    [Inject] private CameraManager cameraManager;
    
    [SerializeField] private float moveTime = 1f;
    [SerializeField] private float fadeDelaay = 0.8f;
    public Transform startPlace;
    public Transform endPlace;
    public float changePause = 0.5f;
    public AddedMoneyElement[] elements;
    public Queue<AddedMoneyElement> queue = new();

    private float lastChangeTime;
    private int moneyChange;
    private int lastMoney;
    
    private void Awake()
    {
        foreach (var element in elements)
        {
            queue.Enqueue(element); 
        }
    }

    private void Start()
    {
        lastMoney = playerRepository.Money;
    }

    private void Update()
    {
        if (playerRepository.Money != lastMoney)
        {
            if (lastMoney < playerRepository.Money)
            {
                moneyChange += playerRepository.Money - lastMoney;
                lastChangeTime = Time.time;
            }
            lastMoney = playerRepository.Money;
        }

        if (moneyChange > 0 && Time.time - lastChangeTime > changePause)
        {
            var showChange = moneyChange;
            moneyChange = 0;
            var element = GetQueueElement();
            element.Show(showChange, startPlace, endPlace, 
                moveTime, 
                fadeDelaay, 
                cameraManager, 
                () =>
            {
                queue.Enqueue(element);
            });
        }
    }

    private AddedMoneyElement GetQueueElement()
    {
        return queue.Dequeue();
    }
}
