using System;
using UnityEngine;

public abstract class TutorialStep
{
    private float progress = -1;
    private string title;
    protected string progressText;
    
    public string GetTitle()
    {
        if (string.IsNullOrEmpty(title))
        {
            title = CreateTitle();
        }
        return title;
    }

    public string GetProgressText()
    {
        if (progress != GetProgress())
        {
            progressText = CreateProgressText();
        }
        return progressText;
    }
    
    public abstract float GetProgress();
    
    public abstract Transform GetCameraTarget();
    
    public abstract Transform GetArrowTarget();
    
    protected abstract string CreateProgressText();
    
    protected abstract string CreateTitle();

    public bool IsCompleted()
    {
        return progress >= 1f || ;
    }
}