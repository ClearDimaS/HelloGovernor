using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PressConferenceUI_Panel : UI_Panel
{
    [SerializeField] private MiniGameUIRewardElement miniGameUIRewardElement;
    [SerializeField] private Button answer1;
    [SerializeField] private Button answer2;

    [SerializeField] private TMP_Text previousText;
    [SerializeField] private TMP_Text answer1Text;
    [SerializeField] private TMP_Text answer2Text;
    [SerializeField] private TMP_Text correctCounterText;
    [SerializeField] private TMP_Text wrongCounterText;
    [SerializeField] private MinigameResultPanel resultPanel;
    [SerializeField] private UI_ElementAnimator questionsPanel;

    protected PressConferenceGame game;
    
    protected override void OnAwake()
    {
        base.OnAwake();
        answer1.onClick.AddListener(() => Answer(0));
        answer2.onClick.AddListener(() => Answer(1));
    }

    private void Answer(int index)
    {
        var oldReward = game.GetReward();
        game.Answer(index);
        var reward = game.GetReward();
        miniGameUIRewardElement.RefreshWithChange(reward, reward-oldReward);

        correctCounterText.text = game.correctCounter.ToString();
        wrongCounterText.text = game.wrongCounter.ToString();
        
        if (game.HasMoreQuestions())
        {
            RefreshTexts();
        }
        else
        {
            resultPanel.Show(game, Hide);
            questionsPanel.AnimateBack();
        }
    }

    public void Init(PressConferenceGame game)
    {
        this.game = game;
    }

    public override void OnShow()
    {
        base.OnShow();
        miniGameUIRewardElement.Init(0);
        resultPanel.Hide(true);
        questionsPanel.Animate(instant:true);
        RefreshTexts();
    }

    protected void RefreshTexts()
    {
        previousText.text = game.GetPrevious();
        answer1Text.text = game.GetQuestion1();
        answer2Text.text =  game.GetQuestion2();
    }
}