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

    public int SkinIndex { get; private set; }
    public int CurrentPrice => skinManager.GetPrice(SkinIndex);

    private Transform[] skinParents;
    private bool isInit;

    private void Start()
    {
        SkinIndex = playerRepository.GetData().skinIndex;
        cam.enabled = false;
        var panel = uiManager.GetPanel<SkinPreviewPanel>();
        panel.SetSwiper(swiper.Swiper);
        panel.saveEvent += Save;
        panel.pointerDownEvent += swiper.MouseDown;
        panel.dragEvent += swiper.Drag;
        panel.pointerUpEvent += swiper.MouseUp;
    }

    private void Update()
    {
        SkinIndex = swiper.ElementIndex % skinManager.SkinCount;;
    }

    private void Save()
    {
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
        SkinIndex--;
        swiper.MoveElements(-1, false);
    }
    
    [Button]
    private void MoveRight()
    {
        SkinIndex++;
        SkinIndex %= skinManager.SkinCount;
        swiper.MoveElements(1, false);
    }
}
