using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Zenject;

public class WishGranterComboWombo : CulledBehaviour
{
    [Inject] private PlayerController player;

    [SerializeField] protected GameObject content;
    [SerializeField] protected TextMesh[] comboTexts;
    [SerializeField] protected Transform[] scales;

    protected int comboCounter = 0;
    protected WishGranter wishGranter;
    protected Dictionary<int, string> stringsDict = new ();

    protected override void OnAwake()
    {
        base.OnAwake();
        wishGranter = GetComponentInParent<WishGranter>();
        if (wishGranter == null)
        {
            Destroy(gameObject);
        }
        wishGranter.SubscribePlayerProcess(AddCombo);
    }

    protected override void OnUpdate(bool visible)
    {
        base.OnUpdate(visible);
        var show = comboCounter > 0;
        content.SetActiveOnce(show);
    }

    private void AddCombo(Transform place)
    {
        comboCounter++;
        var reward = 10;
        var models = 2;
        var combo = comboCounter;
        player.ReturnMoney(reward, place, models);
        foreach (var comboText in comboTexts)
        {
            comboText.transform.DOScale(Vector3.one * 1.15f, 0.15f).OnComplete(() =>
            {
                comboText.text = GetString(combo);
                comboText.transform.DOScale(Vector3.one, 0.15f);
            });   
        }

        foreach (var scale in scales)
        {
            scale.transform.DOScale(Vector3.one * 1.15f, 0.15f).OnComplete(() =>
            {
                scale.transform.DOScale(Vector3.one, 0.15f);
            }); 
        }
    }
    
    private string GetString(int count)
    {
        if (!stringsDict.ContainsKey(count))
        {
            stringsDict[count] = count.ToString();
        }
        return stringsDict[count];
    }
}
