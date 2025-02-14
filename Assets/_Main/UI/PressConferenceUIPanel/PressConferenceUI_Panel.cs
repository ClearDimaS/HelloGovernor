using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PressConferenceUI_Panel : UI_Panel
{
    [SerializeField] private PressConferenceConfig conferenceConfig;
    
    [SerializeField] private Button answer1;
    [SerializeField] private Button answer2;

    [SerializeField] private TMP_Text previousText;
    [SerializeField] private TMP_Text answer1Text;
    [SerializeField] private TMP_Text answer2Text;

    private int curIndex = 0;
    protected override void OnAwake()
    {
        base.OnAwake();
        answer1.onClick.AddListener(() => Answer(0));
        answer2.onClick.AddListener(() => Answer(1));
    }

    private void Answer(int index)
    {
        if (curIndex < 3)
        {
            RefreshTexts();
        }
        else
        {
            Hide();
        }
    }

    public override void OnShow()
    {
        base.OnShow();
        curIndex = 0;
        RefreshTexts();
    }

    protected void RefreshTexts()
    {
        var datas = conferenceConfig.datas;
        var questionIndex = Random.Range(0, datas.Length);
        var data = datas[questionIndex];
        var otherRandomData = datas[Random.Range(0, datas.Length)];
        
        previousText.text = data.sentences[curIndex];
        answer1Text.text = data.sentences[curIndex+1];
        answer2Text.text = otherRandomData.sentences[Random.Range(0, otherRandomData.sentences.Length)];
    }
}