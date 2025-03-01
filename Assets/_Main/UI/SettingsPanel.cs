using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class SettingsPanel : UI_Panel
{
    [SerializeField] private Button openButton;
    [SerializeField] private GameObject content;
    [SerializeField] private Button playButton;
    [SerializeField] private Button playButtonBG;

    public static bool IsPause { get; private set; }
    private void Start()
    {
        IsPause = false;
        content.gameObject.SetActive(false);

        openButton.onClick.AddListener(() =>
        {
            IsPause = true;
            Time.timeScale = 0.2f;
            content.gameObject.SetActive(true);
        });
        playButton.onClick.AddListener(() =>
        {
            IsPause = false;
            Time.timeScale = 1f;
            content.gameObject.SetActive(false);
        });
        playButtonBG.onClick.AddListener(() =>
        {
            IsPause = false;
            Time.timeScale = 1f;
            content.gameObject.SetActive(false);
        });
    }
}