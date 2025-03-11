using UnityEngine;

public class PressConferenceGame : Minigame
{
    protected PressConferenceConfig config;

    private bool firstIsCorrect;
    private int themeIndex;
    private int otherThemeIndex;
    private int curIndex = 0;
    private int correctIndex = 0;
    
    private WishGranterGameTimer timer;
    public int correctCounter { get; private set; }
    public int wrongCounter { get; private set; }

    public PressConferenceGame(PressConferenceConfig config, WishGranterGameTimer timer)
    {
        this.config = config;
        themeIndex = Random.Range(0, config.datas.Length);
        this.timer = timer;
        correctCounter = 0;
        wrongCounter = 0;
        RefreshAnswers();
    }

    public override int GetReward()
    {
        return (curIndex+1) * config.rewardPerAnswer;
    }

    public override void Complete()
    {
        timer.ResetToCooldown();
    }

    public void Answer(int index)
    {
        curIndex++;
        if (index == correctIndex || otherThemeIndex == themeIndex)
        {
            correctCounter++;
        }
        else
        {
            wrongCounter++;
        }

        RefreshAnswers();
    }

    private void RefreshAnswers()
    {
        correctIndex = 0;
        firstIsCorrect = Random.Range(0, 1f) < 0.5f;
        if (!firstIsCorrect)
        {
            correctIndex = 1;
        }

        otherThemeIndex = Random.Range(0, config.datas.Length);
    }

    public bool HasMoreQuestions()
    {
        return curIndex < config.datas[themeIndex].sentences.Length - 1;
    }

    public string GetPrevious()
    {
        var datas = config.datas;
        var data = datas[themeIndex];
        return data.sentences[curIndex];
    }
    public string GetQuestion1()
    {
        if (firstIsCorrect)
        {
            return GetCorrect();
        }

        return GetWrong();
    }

    public string GetQuestion2()
    {
        if (firstIsCorrect)
        {
            return GetWrong();
        }

        return GetCorrect();
    }
    
    private string GetWrong()
    {
        var datas = config.datas;
        var otherRandomData = datas[otherThemeIndex];
        return otherRandomData.sentences[Random.Range(0, otherRandomData.sentences.Length)];
    }

    private string GetCorrect()
    {
        var datas = config.datas;
        var data = datas[themeIndex];
        return data.sentences[curIndex+1];
    }
}