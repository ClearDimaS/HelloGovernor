using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine.Playables;
using UnityEngine.UI;
using UnityEngine.Video;
using Zenject;
using Object = UnityEngine.Object;
using DG.Tweening;
using UnityEngine.Localization;

public class LoadingScreen : Singleton<LoadingScreen>
{
    [SerializeField] protected GraphicRaycaster raycaster;
    [SerializeField] protected CanvasGroup blockerCanvas;
    [SerializeField] private RectMask2D rectMask2D;
    [SerializeField] private TMP_Text loadedNameText;
    [SerializeField] private LocalizedAssetTable[] localizedAssetTable;
    
    private int loadedScene = -1;
    
    protected override void OnCreated()
    {
        base.OnCreated();
        Object.DontDestroyOnLoad(gameObject);
    }
    
    private void Start()
    {
        blockerCanvas.alpha = 1f;
        raycaster.enabled = true;
        loadedNameText.text = string.Empty;

        LoadMainScene();
    }

    private async UniTask LoadMainScene()
    {
        await LoadSceneAsync(1, null, 0.75f);

        await WaitOperation(() => true, () =>1f, 0.75f, 1f);

        while (LocalizationManager.Instance.CurrentLocale.Value == null)
        {
            await UniTask.Yield();
        }
        await blockerCanvas.Fade(0.5f, 0.5f, 0f);
        raycaster.enabled = false;
    }

    private async UniTask LoadSceneAsync(int sceneIndex, Action onDone, float max)
    {
        blockerCanvas.interactable = true;
        blockerCanvas.blocksRaycasts = true;
        await PreloadLoadingScreen(0.1f, 0.25f);

        var min = 0.25f;
        if (loadedScene >= 0)
        {
            min = 0.5f;
            var asyncUnload = SceneManager.UnloadSceneAsync(loadedScene);
            if (asyncUnload != null)
            {
                await WaitOperation(() => asyncUnload.isDone, () => asyncUnload.progress, 0.25f, 0.5f);   
            }
        }
        
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
        await AsyncLoad(asyncLoad, onDone, min, max);
        loadedScene = sceneIndex;
    }
    
    private async UniTask PreloadLoadingScreen(float midProgress, float maxProgress)
    {
        raycaster.enabled = true;
        rectMask2D.FillTo(ERectMaskAxis.X_Positive, 0, 0f);
        if (blockerCanvas.alpha < 1f)
        {
            rectMask2D.FillTo(ERectMaskAxis.X_Positive, midProgress, 0.2f);
            await blockerCanvas.Fade(0.2f, 0f, 1f);
        }
        rectMask2D.FillTo(ERectMaskAxis.X_Positive, maxProgress, 0.3f);
        await UniTask.WaitForSeconds(0.3f);
    }

    private async UniTask AsyncLoad(AsyncOperation asyncLoad, Action onDone, float min, float max)
    {
        await WaitOperation(() => asyncLoad.isDone, () => asyncLoad.progress, min, max);

        if (max >= 1f)
        {
            await blockerCanvas.Fade(0.5f, 0.5f, 0f);
        }

        blockerCanvas.interactable = false;
        blockerCanvas.blocksRaycasts = false;
        onDone?.Invoke();
    }

    private async UniTask WaitOperation(Func<bool> isDone, Func<float> getProgress, float min, float max)
    {
        while (!isDone())
        {
            rectMask2D.FillTo(ERectMaskAxis.X_Positive, min + getProgress() * (max - min), 0f);
            await UniTask.Yield();
        }
        
        rectMask2D.FillTo(ERectMaskAxis.X_Positive, max, 0.2f);
        await UniTask.WaitForSeconds(0.2f);
    }
}
