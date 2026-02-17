using System;
using UnityEngine;
using Zenject;

public class InputTutorialElement : MonoBehaviour
{
    [Inject] private PlayerInput playerInput;
    
    [SerializeField] private GameObject tutorialGO_Joystick;
    [SerializeField] private GameObject tutorialGO_KeyBoard;
    [SerializeField] private float moveDurationToComplete = 1f;
    
    private PlayerPrefsIntRepository playerPrefsIntRepository;

    private float isMovingTime;
    
    private bool isDone
    {
        get => playerPrefsIntRepository.Get() == 0 ? false : true;
        set => playerPrefsIntRepository.Set(value ? 1 : 0);
    }
    private void Start()
    {
        playerPrefsIntRepository = new PlayerPrefsIntRepository("Tutorial_input");
        if (!isDone)
        {
            ShowTutorial();
        }
        else
        {
            gameObject.SetActiveOnce(false);
        }
    }

    private void Update()
    {
        if (playerInput.IsMoving)
        {
            isMovingTime += Time.deltaTime;
        }

        if (!isDone && isMovingTime > moveDurationToComplete)
        {
            isDone = true;
            gameObject.SetActiveOnce(false);
        }
    }

    private void ShowTutorial()
    {
        #if UNITY_IOS || UNITY_ANDROID
        tutorialGO_Joystick.SetActiveOnce(true);
        tutorialGO_KeyBoard.SetActiveOnce(false);
        #else
        tutorialGO_Joystick.SetActiveOnce(false);
        tutorialGO_KeyBoard.SetActiveOnce(true);
        #endif
    }
}