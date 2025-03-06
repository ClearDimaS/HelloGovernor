using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Random = UnityEngine.Random;

public class ChatMinigame : Minigame
{
    private int reward;
    
    public ChatMinigame(int reward)
    {
        this.reward = reward;
    }
    public override int GetReward()
    {
        return reward;
    }

    public override void Complete()
    {

    }
}
public class ChatUI_Panel : UI_Panel
{
    [Inject] protected WishesCollectionConfig wishesCollectionConfig;
    
    [SerializeField] private ChatQuestionsConfig questionsConfig;
    
    [SerializeField] private Button answer1;
    [SerializeField] private Button answer2;

    [SerializeField] private MinigameResultPanel resultElement;
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private TMP_Text answer1Text;
    [SerializeField] private TMP_Text answer2Text;

    protected ChatMinigame chatMinigame;
    
    protected override void OnAwake()
    {
        base.OnAwake();
        answer1.onClick.AddListener(() => resultElement.Show(chatMinigame, Hide));
        answer2.onClick.AddListener(() => resultElement.Show(chatMinigame, Hide));
    }

    private void Start()
    {
        resultElement.Hide(true);
        chatMinigame = new ChatMinigame(wishesCollectionConfig.chatReward);
    }

    public override void OnShow()
    {
        base.OnShow();
        resultElement.Hide(true);
        resultElement.gameObject.SetActive(false);
        var questions = questionsConfig.questions;
        var questionIndex = Random.Range(0, questions.Length);
        var question = questions[questionIndex];

        questionText.text = question.question;
        answer1Text.text = question.answer1;
        answer2Text.text = question.answer2;
    }
}