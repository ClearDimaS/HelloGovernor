using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatUI_Panel : UI_Panel
{
    [SerializeField] private ChatQuestionsConfig questionsConfig;
    
    [SerializeField] private Button answer1;
    [SerializeField] private Button answer2;

    [SerializeField] private TMP_Text questionText;
    [SerializeField] private TMP_Text answer1Text;
    [SerializeField] private TMP_Text answer2Text;

    protected override void OnAwake()
    {
        base.OnAwake();
        answer1.onClick.AddListener(Hide);
        answer2.onClick.AddListener(Hide);
    }

    public override void OnShow()
    {
        base.OnShow();
        var questions = questionsConfig.questions;
        var questionIndex = Random.Range(0, questions.Length);
        var question = questions[questionIndex];

        questionText.text = question.question;
        answer1Text.text = question.answer1;
        answer2Text.text = question.answer2;
    }
}