using System.Collections.Generic;
using UnityEngine;

public class OperatableFillPlace : CulledBehaviour
{
    [SerializeField] protected float fillTime = 1f;
    [SerializeField] protected TextMesh maxText;
    [SerializeField] protected TextMesh curText;
    [SerializeField] protected Transform fillPlace;
    [SerializeField] protected float radius = 1.3f;
    [field: SerializeField] public int Max { get; private set; }
    [SerializeField] protected TimerBase timerBase;
    
    public bool CanFill => currentCount < Max;
    public bool IsEmpty => currentCount == 0;

    protected OperatableWithItems granter;
    protected float fillProgress;
    protected List<IOperator> operators = new ();
    protected int currentCount => granter.GetItemsCount();
    protected Dictionary<int, string> stringsDict = new ();
    protected int lastServed;
    private void Start()
    {
        if (Max <= 0)
        {
            return;
        }
        granter = GetComponentInParent<OperatableWithItems>(true);
        curText.text = GetMaxString();
        maxText.text = GetCurString();
        var operatable = GetComponentInParent<UpgradableBuilding>();
        timerBase.SetIcon(operatable.GetPurchaseIcon());
        if (granter == null)
        {
            Debug.LogError($"granter null at: {transform.name}");
        }
    }

    protected override void OnUpdate(bool visible)
    {
        base.OnUpdate(visible);
        
        if (granter == null)
        {
           return;
        }
        if (visible)
        {
            var show = currentCount < Max;
            if (show != fillPlace.gameObject.activeSelf)
            {
                fillPlace.gameObject.SetActive(show);
            }
            curText.text = GetMaxString();
            maxText.text = GetCurString();
        }
        if (lastServed != granter.ServedCounter)
        {
            lastServed = granter.ServedCounter;
        }
        var isFilling = false;
        foreach (var @operator in operators)
        {
            var diff = @operator.Root.position - fillPlace.position;
            diff.y = 0f;
            if (diff.sqrMagnitude < radius * radius)
            {
                isFilling = true;
            }
        }

        if (isFilling && CanFill)
        {
            fillProgress += Time.deltaTime / fillTime;
        }
        else
        {
            fillProgress = 0f;
        }
        if (fillProgress >= 1f)
        {
            fillProgress -= 1f;
            granter.AddItem();
        }

        var showTimer = CanFill;
        if (showTimer != timerBase.gameObject.activeSelf)
        {
            timerBase.gameObject.SetActive(showTimer);
        }
        timerBase.SetProgress(fillProgress);
    }

    private string GetMaxString()
    {
        return GetString(Max);
    }

    private string GetCurString()
    {
        return GetString(currentCount);
    }
    
    private string GetString(int count)
    {
        if (!stringsDict.ContainsKey(count))
        {
            stringsDict[count] = count.ToString();
        }
        return stringsDict[count];
    }

    public Transform GetPlace()
    {
        return fillPlace;
    }

    public void Add(IOperator @operator)
    {
       operators.Add(@operator);
    }
}