using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BankWishPanel : UI_Panel
{
    [SerializeField] private MiniGameUIRewardElement rewardElement;
    [SerializeField] private UIWishTimer timer;
    [SerializeField] private BankOptionUIElement option1;
    [SerializeField] private BankOptionUIElement option2;
    [SerializeField] private CanvasGroup content;
    [SerializeField] private MinigameResultPanel resultPanel;

    private Func<bool> canShowNext;
    private Action onAnswer;
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

    public void Init(BankGame bankGame, Action onAnswer, Func<bool> canShowNext)
    {
        this.onAnswer = onAnswer;
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
        if (!first)
        {
            onAnswer?.Invoke();
        }
    }

    private void ShowOptions(BankOption optionData1, BankOption optionData2)
    {
        var isPositiveFirst = UnityEngine.Random.Range(0, 1f) < 0.5f;
        option1.Init(optionData1, () => Select(optionData1), isPositiveFirst);
        option2.Init(optionData2, () => Select(optionData2), !isPositiveFirst);
    }

    private void Select(BankOption data)
    {
        option1.gameObject.SetActive(false);
        option2.gameObject.SetActive(false);
        bankGame.SetSelected(data);
        rewardElement.RefreshWithChange(bankGame.GetReward(), data.GetReward());
        if (bankGame.IsRunning)
        {
            UniTask.WaitUntil(() => canShowNext()).ContinueWith(() =>
            {
                ShowNewOptions(false);
            });
        }
    }
}