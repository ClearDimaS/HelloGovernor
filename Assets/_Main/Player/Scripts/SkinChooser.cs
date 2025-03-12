using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

public class SkinChooser : MonoBehaviour
{
    [Inject] private GameConfig gameConfig;
    [Inject] private CameraManager cameraManager;
    [Inject] private UI_Manager uiManager;
    [Inject] private PlayerSkinManager skinManager;
    [Inject] private PlayerDataRepository playerRepository;
    [Inject] private CacheManager cacheManager;
    
    [SerializeField] private Camera cam;
    [SerializeField] private SwiperCyclicFacade swiper;
    private SkinBehaviour[] skinBehaviours;
    public int SkinIndex { get; private set; }
    public int CurrentPrice => skinManager.GetPrice(SkinIndex);
    public bool WasOpen { get; set; }

    private bool isInit;

    private void Start()
    {
        SkinIndex = playerRepository.SkinIndex;
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
        WasOpen = true;
        cam.enabled = true;
        if (!isInit)
        {
            isInit = true;
            UniTask.WaitUntil(() => skinManager.Prefabs != null).ContinueWith(() =>
            {
                swiper.SetLayer(LayerMask.NameToLayer("SkinPreview"));
                swiper.Initialize(skinManager.Prefabs, playerRepository.SkinIndex);
                skinBehaviours = swiper.GetComponentsInChildren<SkinBehaviour>();
            });
        }

        if (skinBehaviours == null)
        {
            UniTask.WaitUntil(() => skinBehaviours != null).ContinueWith(() =>
            {
                InitSkins();
            });
        }
        else
        {
            InitSkins();
        }
        uiManager.OpenPanel<SkinPreviewPanel>();
        cameraManager.SetActiveCamera(cam);
    }

    private void InitSkins()
    {
        for (int i = 0; i < skinBehaviours.Length; i++)
        {
            var skin = skinBehaviours[i];
            var levelToUnlock = i / gameConfig.skinsPerLevel;
            skin.SetLocked(levelToUnlock > cacheManager.LevelIndex);
        }
    }

    public bool IsCurrentLocked()
    {
        var levelToUnlock = SkinIndex / gameConfig.skinsPerLevel;
        return levelToUnlock > cacheManager.LevelIndex;
    }

    public int GetCurrentLevelIndexToUnlock()
    {
        var levelToUnlock = SkinIndex / gameConfig.skinsPerLevel;
        return levelToUnlock;
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

    [SerializeField] private Sprite tutorialIcon;
    public Sprite GetTutorialIcon()
    {
       return tutorialIcon;
    }
}
