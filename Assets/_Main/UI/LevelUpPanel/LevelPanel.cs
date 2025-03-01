using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Random = UnityEngine.Random;

public class FormatableText
{
    private TMP_Text text;
    private string formatString = "{0}";
    
    private int lastVal = -1;

    public FormatableText(TMP_Text text, string formatString)
    {
        this.text = text;
        this.formatString = formatString;
    }
    
    public void RefreshText(bool immediate, int newValue)
    {
        if (lastVal != newValue)
        {
            lastVal = newValue;
            if (immediate)
            {
                text.text = string.Format(formatString, newValue);
            }
            else
            {
                text.transform.DOScale(Vector3.one * 1.2f, 0.15f).OnComplete(() =>
                {
                    text.text = string.Format(formatString, newValue);
                    text.transform.DOScale(Vector3.one, 0.15f);
                });
            }
        }
    }

}

public class LevelPanel : UI_Panel
{
    [Inject] private PlayerController player;
    
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text expTextCur;
    [SerializeField] private TMP_Text expTextMax;

    [SerializeField] private float barSpeed = 3f;
    [SerializeField] private Image expFillImg;
    
    [SerializeField] private UI_ElementAnimator newLevelPanel;

    private FormatableText exp;
    private FormatableText expMax;
    private FormatableText level;
    
    protected override void OnAwake()
    {
        base.OnAwake();
        exp = new FormatableText(expTextCur, "{0}");
        expMax = new FormatableText(expTextMax, "{0}");
        level = new FormatableText(levelText, "{0}");
    }

    private void Start()
    {
        exp.RefreshText(true, player.Exp);
        expMax.RefreshText(true, player.MaxExp);
        level.RefreshText(true, player.Level);
    }
    
    [Button]
    private void AddExp()
    {
        player.Exp += Random.Range(1, 10);
    }

    private void Update()
    {
        exp.RefreshText(false, player.Exp);
        expMax.RefreshText(false, player.MaxExp);
        level.RefreshText(false, player.Level);
        
        var oldProgress = expFillImg.rectTransform.anchorMax.x;
        var targetProgress = player.Exp / (float)player.MaxExp;
        var newProgress =  Mathf.Lerp(oldProgress, targetProgress, Time.deltaTime * barSpeed);
        expFillImg.rectTransform.FillParent(newProgress);

        if (player.Exp > player.MaxExp)
        {
            Debug.Log($"increaseLevel");
            player.Level++;
            player.Exp = 0;
            player.MaxExp += 15;
        }
    }
}
