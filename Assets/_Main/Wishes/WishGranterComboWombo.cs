using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Zenject;

[Serializable]
public class ComboLevel
{
    public float multiplier;
    public int maxCounter;
    public int models;
}
public class WishGranterComboWombo : CulledBehaviour
{
    [Inject] private PlayerController player;

    [SerializeField] protected ParticleSystem comboIncreasePS;
    [SerializeField] protected ParticleSystem comboMaxPS;
    [SerializeField] protected GameObject content;
    [SerializeField] protected TextMesh[] comboTexts;
    [SerializeField] protected Transform[] scales;
    [SerializeField] protected SpriteFillerHorizontal filler;
    [SerializeField] protected ComboLevel[] comboDatas;
    [SerializeField] protected float fillTimer = 0.6f;
    [SerializeField] protected float decreaseCounterTime = 0.6f;
    [SerializeField] protected float decreaseCounterDelay = 2f;

    protected float decreaseTimer;
    protected float lastTimeProcessed;
    protected int comboLevel;
    protected int comboCounter = 0;
    protected WishGranter wishGranter;
    protected Dictionary<float, string> stringsDict = new ();

    protected override void OnAwake()
    {
        base.OnAwake();
        wishGranter = GetComponentInParent<WishGranter>();
        if (wishGranter == null)
        {
            Destroy(gameObject);
            return;
        }
        wishGranter.SubscribePlayerProcess(AddCombo);
        transform.position = wishGranter.GetMiddleProcessPlace();
        comboMaxPS.Stop();
        comboMaxPS.transform.position = transform.position;
    }

    protected override void OnUpdate(bool visible)
    {
        base.OnUpdate(visible);
        if (visible)
        {
            var show = comboCounter > 0 || comboLevel > 0;
            content.SetActiveOnce(show);

            if (Time.time - lastTimeProcessed > decreaseCounterDelay && 
                (comboCounter >= 0 || comboLevel >= 0))
            {
                decreaseTimer += Time.deltaTime;
            }
            else
            {
                decreaseTimer = 0f;
            }

            if (comboCounter > 0 || comboLevel > 0)
            {
                if (comboCounter > 0)
                {
                    var comboLevelData = comboDatas[comboLevel];
                    var nextDecreaseCounter = comboCounter - 1;
                    if (Time.time - lastTimeProcessed <= decreaseCounterDelay)
                    {
                        nextDecreaseCounter = comboCounter;
                    }
                    var nextFill = (nextDecreaseCounter) / (float)comboLevelData.maxCounter;
                    if (comboLevel >= comboDatas.Length - 1)
                    {
                        nextFill = 1f - decreaseTimer / decreaseCounterTime;
                    }
                    if (!filler.IsMoving)
                    {
                        filler.fillAmount = Mathf.Lerp(filler.fillAmount, nextFill, Time.deltaTime);
                    }
                }

                if (decreaseTimer > decreaseCounterTime)
                {
                    decreaseTimer = 0f;
                    comboCounter--;
                    if (comboCounter <= 0)
                    {
                        if (comboLevel > 0)
                        {
                            comboLevel--;
                            comboLevel = Mathf.Max(0, comboLevel);
                            var comboLevelData = comboDatas[comboLevel];
                            comboCounter = comboLevelData.maxCounter;
                            RefreshComboCounter();   
                        }
                    }
                }
            }
        }
    }

    private void AddCombo(Transform place)
    {
        lastTimeProcessed = Time.time;
        var comboLevelData = comboDatas[comboLevel];
        var rewardBase = wishGranter.GetRewardWithMultipliers();
        var reward = Mathf.RoundToInt(rewardBase * comboLevelData.multiplier)-rewardBase;
        var models = comboLevelData.models;
        player.ReturnMoney(reward, place, models);
        
        if (comboLevel >= comboDatas.Length - 1)
        {
            return;
        }
        comboCounter++;
        filler.Fill(comboCounter/(float)comboLevelData.maxCounter, fillTimer);
        if (comboCounter >= comboLevelData.maxCounter)
        {
            comboIncreasePS.Play();
            comboLevel++;
            comboCounter = 1;
            RefreshComboCounter();
        }
    }

    private void RefreshComboCounter()
    {
        var comboLevelData = comboDatas[comboLevel];
        var multiplier = comboLevelData.multiplier;
        foreach (var comboText in comboTexts)
        {
            comboText.transform.DOScale(Vector3.one * 1.15f, 0.15f).OnComplete(() =>
            {
                comboText.text = GetString(multiplier);
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

        if (comboLevel >= comboDatas.Length - 1)
        {

        }
        else
        {
            filler.fillAmount = comboCounter/(float)comboLevelData.maxCounter;
            filler.StopFill();
        }
    }

    private string GetString(float count)
    {
        if (!stringsDict.ContainsKey(count))
        {
            stringsDict[count] = count.ToString("0.0");
        }
        return stringsDict[count];
    }
}
