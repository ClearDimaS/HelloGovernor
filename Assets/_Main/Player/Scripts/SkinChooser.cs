using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

public class SkinChooser : MonoBehaviour
{
    [Inject] private CameraManager cameraManager;
    [Inject] private UI_Manager uiManager;
    [Inject] private PlayerSkinManager skinManager;
    [Inject] private PlayerDataRepository playerRepository;

    [SerializeField] private Camera cam;
    [SerializeField] private SwiperCyclicFacade swiper;

    private Transform[] skinParents;
    private bool isInit;

    private void Start()
    {
        cam.enabled = false;
        var panel = uiManager.GetPanel<SkinPreviewPanel>();
        panel.SetSwiper(swiper.Swiper);
        panel.saveEvent += Save;
        panel.pointerDownEvent += swiper.MouseDown;
        panel.dragEvent += swiper.Drag;
        panel.pointerUpEvent += swiper.MouseUp;
    }

    private void Save()
    {
        var data = playerRepository.GetData();
        data.skinIndex = swiper.ElementIndex;
        playerRepository.SetData(playerRepository.GetData());
        Hide();
    }

    [Button]
    public void Show()
    {
        cam.enabled = true;
        if (!isInit)
        {
            isInit = true;
            UniTask.WaitUntil(() => skinManager.Prefabs != null).ContinueWith(() =>
            {
                swiper.SetLayer(LayerMask.NameToLayer("SkinPreview"));
                swiper.Initialize(skinManager.Prefabs, playerRepository.GetData().skinIndex);
            });
        }
        uiManager.OpenPanel<SkinPreviewPanel>();
        cameraManager.SetActiveCamera(cam);
    }

    [Button]
    private void Hide()
    {
        cam.enabled = false;
        uiManager.ClosePanel<SkinPreviewPanel>();
        cameraManager.SetActiveCamera(cameraManager.OriginalCamera);
    }

    [Button]
    private void MoveLeft()
    {
        playerRepository.GetData().skinIndex--;
        swiper.MoveElements(-1, false);
    }
    
    [Button]
    private void MoveRight()
    {
        playerRepository.GetData().skinIndex++;
        playerRepository.GetData().skinIndex %= skinManager.SkinCount;
        swiper.MoveElements(1, false);
    }
}
