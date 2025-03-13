using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class SwiperNextPreviousButtons : MonoBehaviour
{
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;

    private ElementsSwiper pageSwiper;
    private bool isInit;
    private bool isSubscribed;

    private void OnEnable()
    {
        Init();
    }

    private void Init()
    {
        if (isInit)
        {
            return;
        }

        isInit = true;
        if (pageSwiper == null)
        {
            pageSwiper = GetComponent<ElementsSwiper>();   
        }
        if (nextButton != null)
            nextButton.onClick.AddListener(Next);
        if (previousButton != null)
            previousButton.onClick.AddListener(Previous);
        if (pageSwiper != null)
        {
            SubscribeSwiper();
        }
    }

    public void SetSwiper(ElementsSwiper swiper)
    {
        isSubscribed = false;
        pageSwiper = swiper;
        SubscribeSwiper();
    }
    
    private void SubscribeSwiper()
    {
        if (isSubscribed)
        {
            return;
        }

        isSubscribed = true;
        pageSwiper.Element.Subscribe(RefreshButtons).AddTo(this);
        pageSwiper.ElementAddCommand.Subscribe(_ => OnElementsChange()).AddTo(this);
        pageSwiper.ElementRemoveCommand.Subscribe(_ => OnElementsChange()).AddTo(this);
    }
    
    private void Next()
    {
        pageSwiper.MoveElements(1, false);
    }

    private void Previous()
    {
        pageSwiper.MoveElements(-1, false);
    }

    private void Start()
    {
        OnElementsChange();
    }

    private void OnElementsChange()
    {
        if(pageSwiper != null)
            RefreshButtons(pageSwiper.CurElementNumber);
    }

    private void RefreshButtons(int pageIndex)
    {

    }
}