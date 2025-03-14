using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyStackCapacityIndicator : CulledBehaviour
{
    [SerializeField] protected TextMesh[] curTexts;
    [SerializeField] protected TextMesh[] maxTexts;
    [SerializeField] protected GameObject content;
    [SerializeField] protected SpriteFillerHorizontal filler;
    
    private CurrencyStackBehaviour currencyStack;
    private Dictionary<int, string> countsDict = new ();

    private int lastCount = -1;
    private int lastMaxCount = -1;
    
    protected override void OnAwake()
    {
        base.OnAwake();
        currencyStack = GetComponentInParent<CurrencyStackBehaviour>();
        if (currencyStack == null)
        {
            Debug.LogError($"currency null at: {transform.GetHierarchyString()}");
        }
    }

    protected override void OnUpdate(bool visible)
    {
        base.OnUpdate(visible);
        if (visible)
        {
            var curCount = currencyStack.GetMoney();
            if (curCount != lastCount)
            {
                lastCount = curCount;
                string stringValue = GetStringValue(lastCount);
                foreach (var curText in curTexts)
                {
                    curText.text = stringValue;
                }

                filler.Fill((lastCount / (float)lastMaxCount), 0.3f);
            }
        
            var maxCount = currencyStack.GetMaxMoney();
            if (maxCount != lastMaxCount)
            {
                lastMaxCount = maxCount;
                string stringValue = GetStringValue(maxCount);
                foreach (var maxText in maxTexts)
                {
                    maxText.text = stringValue;
                }
                
                filler.fillAmount = (lastCount / (float)lastMaxCount);
            }

            var show = curCount > 0;
            content.SetActiveOnce(show);
        }
    }

    private string GetStringValue(int count)
    {
        if (countsDict.ContainsKey(count))
        {
            return countsDict[count];
        }
        else
        {
            var stringValue = Price.ToMoneyString(count);
            countsDict[count] = stringValue;
            return stringValue;
        }
    }
}
