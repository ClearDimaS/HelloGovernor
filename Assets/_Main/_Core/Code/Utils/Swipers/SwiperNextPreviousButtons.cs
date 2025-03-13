using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class SwiperNextPreviousButtons : MonoBehaviour
{
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;

    private ElementsSwiper pageSwiper;

    private void Awake()
    {
        pageSwiper = GetComponent<ElementsSwiper>();
        if (nextButton)
            nextButton.onClick.AddListener(Next);
        if (previousButton)
            previousButton.onClick.AddListener(Previous);
        if (pageSwiper != null)
        {
            SubscribeSwiper();
        }
    }

    public void SetSwiper(ElementsSwiper swiper)
    {
        pageSwiper = swiper;
        SubscribeSwiper();
    }
    
    private void SubscribeSwiper()
    {
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