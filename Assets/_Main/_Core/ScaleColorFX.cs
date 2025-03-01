using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScaleColorFX : MonoBehaviour
{
    [SerializeField] private Color color = Color.white;
    [SerializeField] private float scaleTime = 0.5f, scalePressed = 0.9f;
    [SerializeField] private Image maskIcon, colorIcon = null;

    private Selectable selectable;
    private Transform target;
    private Color invisible = new Color(0,0,0,0);

    private void Awake()
    {
        if (transform.parent == null)
            return;
        SubscribeTrigger();
        SubscribeButton();
        SubscribeToggle();
    }

    private void OnEnable()
    {
        target.transform.localScale = Vector3.one;
    }

    private void Start()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        var rect = target.GetComponent<RectTransform>();
        var oldPivot = rect.pivot;

        rect.pivot = Vector2.one / 2f;
        rect.anchoredPosition += Vector2.Scale(Vector2.one / 2f - oldPivot, new Vector2(rect.rect.width, rect.rect.height));
        OnPointerUp();
        transform.SetAsLastSibling();
    }
    private void SubscribeTrigger()
    {
        var parentTrigger = transform.parent.GetComponentInParent<EventTrigger>();
        if (parentTrigger == null)
            return;
        var image = parentTrigger.GetComponent<Image>();
        if (image == null)
            return;

        maskIcon.sprite = image.sprite;
        maskIcon.preserveAspect = image.preserveAspect;

        var onPointerUp = new EventTrigger.Entry() { eventID = EventTriggerType.PointerUp };
        onPointerUp.callback.AddListener((eventData) => OnPointerUp());

        var onPointerDown = new EventTrigger.Entry() { eventID = EventTriggerType.PointerDown };
        onPointerDown.callback.AddListener((eventData) => OnPointerDown());

        parentTrigger.triggers.Add(onPointerUp);
        parentTrigger.triggers.Add(onPointerDown);

        target = parentTrigger.transform;
        selectable = null;
    }

    private void SubscribeButton()
    {
        var parentButton = transform.parent.GetComponentInParent<EventfullButton>();
        if (parentButton == null)
            return;
        if (parentButton.image != null)
        {
            maskIcon.sprite = parentButton.image.sprite;
            maskIcon.preserveAspect = parentButton.image.preserveAspect;
            maskIcon.type = parentButton.image.type;
            maskIcon.pixelsPerUnitMultiplier = parentButton.image.pixelsPerUnitMultiplier;
        }

        parentButton.OnPointerDownCommand.Subscribe(_ => OnPointerDown()).AddTo(this);
        parentButton.OnPointerUpCommand.Subscribe(_ => OnPointerUp()).AddTo(this);
        target = parentButton.transform;
        selectable = parentButton;
    }

    private void SubscribeToggle()
    {
        var parentToggle = transform.parent.GetComponentInParent<EventfullToggle>();
        if (parentToggle == null)
            return;
        if (parentToggle.image != null)
        {
            maskIcon.sprite = parentToggle.image.sprite;
            maskIcon.preserveAspect = parentToggle.image.preserveAspect;
        }

        parentToggle.OnPointerDownCommand.Subscribe(_ => OnPointerDown()).AddTo(this);
        parentToggle.OnPointerUpCommand.Subscribe(_ => OnPointerUp()).AddTo(this);
        target = parentToggle.transform;
        selectable = parentToggle;
    }

    private void OnPointerDown()
    {
        if (selectable != null && !selectable.interactable)
        {
            target.transform.DOShakeScale(0.15f, strength: 0.15f, randomness:45f, vibrato:2).OnComplete(() =>
            {
                target.transform.DOScale(Vector3.one, 0.05f);
            });
            target.transform.DOShakeRotation(0.15f, strength: new Vector3(0, 0, 15f), randomness:45f, vibrato:3).OnComplete(() =>
            {
                target.transform.DOLocalRotateQuaternion(Quaternion.identity, 0.05f);
            });
            return;
        }
        target.transform.DOComplete(true);
        if (maskIcon)
            maskIcon.gameObject.SetActive(true);
        target.transform.DOScale(Vector3.one * scalePressed, scaleTime);
        if (maskIcon.sprite != null)
        {
            colorIcon.color = color;
        }
    }

    private void OnPointerUp()
    {
        if (selectable != null && !selectable.interactable)
            return;
        target.transform.DOScale(Vector3.one, scaleTime).OnComplete(() => 
        {
            if(maskIcon)
                maskIcon.gameObject.SetActive(false);
        });
        if (maskIcon.sprite != null)
        {
            colorIcon.color = invisible;
        }
    }
}