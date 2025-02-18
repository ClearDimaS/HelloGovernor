using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BankRewardElement : MonoBehaviour
{
    [SerializeField] private TMP_Text rewardText;

    [SerializeField] private float moveDuration = 1f;
    [SerializeField] private float fadeDelay = 0.8f;
    [SerializeField] private TMP_Text changeText;
    [SerializeField] private CanvasGroup changeRoot;
    [SerializeField] private RectTransform changeStartPlace;
    [SerializeField] private RectTransform changeEndPlace;

    public void Init(int reward)
    {
        rewardText.text = reward.ToString();
        changeRoot.alpha = 0f;
    }

    public void RefreshWithChange(int totall, int change)
    {
        rewardText.transform.DOScale(Vector3.one * 1.15f, 0.15f).OnComplete(() =>
        {
            rewardText.text = totall.ToString();
            rewardText.transform.DOScale(Vector3.one, 0.15f);
        });
        rewardText.text = totall.ToString();
        changeText.text = change.ToString();
        
        changeRoot.transform.position = changeStartPlace.position;
        changeRoot.transform.rotation = changeStartPlace.rotation;
        var t = 0f;

        DOTween.To(() => t, x => t = x, 1f, moveDuration).OnUpdate(() =>
        {
            var pos = Vector3.Lerp(changeStartPlace.position, changeEndPlace.position, t);
            var rot = Quaternion.Lerp(changeStartPlace.rotation, changeEndPlace.rotation, t);
            changeRoot.transform.position = pos;
            changeRoot.transform.rotation = rot;
        });
        
        changeRoot.alpha = 1f;
        changeRoot.DOFade(0f, moveDuration - fadeDelay).SetDelay(fadeDelay);
    }
}
public class BankWishPanel : UI_Panel
{
    [SerializeField] private BankRewardElement rewardElement;
    [SerializeField] private BankWishTimer timer;
    [SerializeField] private BankOptionUIElement option1;
    [SerializeField] private BankOptionUIElement option2;

    private BankGame bankGame;

    private void Update()
    {
        timer.SetTimeLeft(Mathf.RoundToInt(bankGame.GetTimeLeft()));
    }

    public void Init(BankGame bankGame)
    {
        this.bankGame = bankGame;
        rewardElement.Init(bankGame.GetReward());
        ShowNewOptions();
    }

    private void ShowNewOptions()
    {
        bankGame.GetNextOptions(out BankOption optionData1, out BankOption optionData2);
        ShowOptions(optionData1, optionData2);
    }

    private void ShowOptions(BankOption optionData1, BankOption optionData2)
    {
        option1.Init(optionData1, () => Select(optionData1));
        option2.Init(optionData2, () => Select(optionData1));
    }

    private void Select(BankOption data)
    {
        bankGame.SetSelected(data);
        rewardElement.RefreshWithChange(data.GetReward(), bankGame.GetReward());
        if (bankGame.IsRunning)
        {
            ShowNewOptions();
        }
    }
}