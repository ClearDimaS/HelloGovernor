using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class SettingsButton : MonoBehaviour
{
    [Inject] private CacheManager cacheManager;

    [SerializeField] private Button button;
    [SerializeField] private bool isSound;
    
    [SerializeField] private Image bgButton;
    [SerializeField] private Image iconButton;

    [SerializeField] private Sprite onBg;
    [SerializeField] private Sprite offBg;
    
    [SerializeField] private Sprite onIcon;
    [SerializeField] private Sprite offIcon;

    private int state = -1;
    
    void Start()
    {
        RefreshState();
        button.onClick.AddListener(() =>
        {
            if (isSound)
            {
                cacheManager.IsSoundOn = !cacheManager.IsSoundOn;
            }
            else
            {
                cacheManager.IsVibrationsOn = !cacheManager.IsVibrationsOn;
            }
        });
    }

    void Update()
    {
        RefreshState();
    }

    private void RefreshState()
    {
        var newState = IsOn() ? 1 : 0;
        if (newState != state)
        {
            state = newState;
            bgButton.sprite = state == 0 ? offBg : onBg;
            iconButton.sprite = state == 0 ? offIcon : onIcon;
        }
    }

    private bool IsOn()
    {
        if (isSound)
        {
            return cacheManager.IsSoundOn;
        }
        return cacheManager.IsVibrationsOn;
    }
}
