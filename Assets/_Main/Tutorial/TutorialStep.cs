using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public abstract class TutorialStep
{
    [Inject] protected TutorialsConfig tutorialsConfig;
    [Inject] protected PlayerDataRepository playerRepository;

    protected float lastProgress = -1;
    private float progress = -1;
    private string title;
    protected string progressText;
    public virtual bool UseCache => true;

    protected string _key
    {
        get
        {
            if (string.IsNullOrEmpty(_keyCache))
            {
                _keyCache = CreateKey();
            }

            return _keyCache;
        }
    }

    protected string _keyCache;
    
    public TutorialStep(PlayerDataRepository playerRepository, TutorialsConfig tutorialsConfig)
    {
        this.tutorialsConfig = tutorialsConfig;
        this.playerRepository = playerRepository;
    }

    protected abstract string CreateKey();

    public virtual void Start()
    {
        
    }
    public void GetTitle(bool needRecreate, Action<string> handler)
    {
        title = CreateTitle();
        handler(title);
    }

    public string GetProgressText()
    {
        if (lastProgress != GetProgress())
        {
            lastProgress = GetProgress();
            progressText = CreateProgressText();
        }
        return progressText;
    }

    public void UpdateProgress()
    {
        UpdateProgress_Internal();
        progress = GetProgress();
    }
    protected abstract void UpdateProgress_Internal();
    
    public abstract float GetProgress();
    
    public abstract Transform GetCameraTarget();
    
    public abstract Transform GetArrowTarget();
    
    protected abstract string CreateProgressText();
    
    protected abstract string CreateTitle();

    public void SaveAsCompleted()
    {
        if (!playerRepository.IsTutorialCompleted(GetKey()))
        {
            playerRepository.SetTutorialCompleted(GetKey());
        }
    }
    public bool IsCompleted()
    {
        return progress >= 1f || (UseCache && playerRepository.IsTutorialCompleted(GetKey()));
    }

    private string GetKey()
    {
        return _key;
    }

    public abstract Sprite GetTutorialIcon();
}