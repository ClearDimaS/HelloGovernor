using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class BankWishPanel : UI_Panel
{
    [SerializeField] private MiniGameUIRewardElement rewardElement;
    [SerializeField] private UIWishTimer timer;
    [SerializeField] private BankOptionUIElement option1;
    [SerializeField] private BankOptionUIElement option2;
    [SerializeField] private CanvasGroup content;
    [SerializeField] private MinigameResultPanel resultPanel;
    [SerializeField] private GameObject answersGO;
    [SerializeField] private TMP_Text answer1;
    [SerializeField] private TMP_Text answer2;
    [SerializeField] private GameObject answer1CorrectGO;
    [SerializeField] private GameObject answer2CorrectGO;
    
    private Func<bool> canShowNext;
    private BankGame bankGame;
    private bool isOverShown;

    private void Start()
    {
        resultPanel.Hide(true);
    }

    private void Update()
    {
        if (bankGame.IsOver())
        {
            if (!isOverShown)
            {
                isOverShown = true;
                resultPanel.gameObject.SetActive(true);
                resultPanel.Show(bankGame, Hide);
                content.alpha = 0f;
            }
        }
        else
        {
            timer.SetTimeLeft(Mathf.RoundToInt(bankGame.GetTimeLeft()));
        }
    }

    public void Init(BankGame bankGame, Func<bool> canShowNext)
    {
        answersGO.SetActive(false);
        this.canShowNext = canShowNext;
        resultPanel.gameObject.SetActive(false);
        content.alpha = 1f;
        isOverShown = false;
        this.bankGame = bankGame;
        rewardElement.Init(bankGame.GetReward());
        ShowNewOptions(true);
    }

    private void ShowNewOptions(bool first)
    {
        option1.gameObject.SetActive(true);
        option2.gameObject.SetActive(true);
        bankGame.GetNextOptions(out BankOption optionData1, out BankOption optionData2);
        ShowOptions(optionData1, optionData2);
    }

    private void ShowOptions(BankOption optionData1, BankOption optionData2)
    {
        var isPositiveFirst = UnityEngine.Random.Range(0, 1f) < 0.5f;
        option1.Init(optionData1, () => Select(optionData1), isPositiveFirst);
        option2.Init(optionData2, () => Select(optionData2), !isPositiveFirst);

        var reward1 = optionData1.GetReward();
        var reward2 = optionData2.GetReward();
        answer1CorrectGO.SetActive(reward1 > 0);
        answer2CorrectGO.SetActive(reward2 > 0);
        answer1.text = reward1 > 0 ? $"+{reward1}" : $"{reward1}";
        answer1.color = reward1 > 0 ? option1.ColorRight : option1.ColorWrong;
        answer2.text = reward2 > 0 ? $"+{reward2}" : $"{reward2}";
        answer2.color = reward2 > 0 ? option1.ColorRight : option1.ColorWrong;
    }

    private void Select(BankOption data)
    {
        option1.gameObject.SetActive(false);
        option2.gameObject.SetActive(false);
        answersGO.SetActive(true);
        bankGame.SetSelected(data);

        rewardElement.RefreshWithChange(bankGame.GetReward(), data.GetReward());
        if (bankGame.IsRunning)
        {
            UniTask.WaitUntil(() => canShowNext()).ContinueWith(() =>
            {
                answersGO.SetActive(false);
                ShowNewOptions(false);
            });
        }
    }
}