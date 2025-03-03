using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
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
    [Inject] private UI_Manager uiManager;
    [Inject] private PlayerController player;

    [SerializeField] private Button showRoadMapButton;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text expTextCur;
    [SerializeField] private TMP_Text expTextMax;

    [SerializeField] private float barSpeed = 3f;
    [SerializeField] private Image expFillImg;
    
    private int lastLevelIndex;
    private PlayerXPController xpController => player.XP_Controller;
    private FormatableText exp;
    private FormatableText expMax;
    private FormatableText level;
    private LevelRoadmapPanel roadmapPanel;
    
    public RectTransform XP_Place => expTextCur.rectTransform;

    protected override void OnAwake()
    {
        base.OnAwake();
        exp = new FormatableText(expTextCur, "{0}");
        expMax = new FormatableText(expTextMax, "{0}");
        level = new FormatableText(levelText, "{0}");
        showRoadMapButton.onClick.AddListener(() =>
        {
            roadmapPanel.Show(false);
        });
    }

    private void Start()
    {
        exp.RefreshText(true, xpController.Exp);
        expMax.RefreshText(true, xpController.MaxExp);
        level.RefreshText(true, xpController.LevelIndex+1);
        lastLevelIndex = xpController.LevelIndex;
        roadmapPanel = uiManager.GetPanel<LevelRoadmapPanel>();
    }

    private void Update()
    {
        exp.RefreshText(false, xpController.Exp);
        expMax.RefreshText(false, xpController.MaxExp);
        level.RefreshText(false, xpController.LevelIndex+1);
        
        var oldProgress = expFillImg.rectTransform.anchorMax.x;
        var targetProgress = xpController.Exp / (float)xpController.MaxExp;
        var newProgress =  Mathf.Lerp(oldProgress, targetProgress, Time.deltaTime * barSpeed);
        expFillImg.rectTransform.FillParent(newProgress);

        if (lastLevelIndex != xpController.LevelIndex)
        {
            lastLevelIndex = xpController.LevelIndex;
            roadmapPanel.Show(true);
        }
    }
}
