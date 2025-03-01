using UniRx;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EventfullToggle : Toggle
{
    public ReactiveCommand OnPointerDownCommand = new ReactiveCommand();
    public ReactiveCommand OnPointerUpCommand = new ReactiveCommand();

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        OnPointerDownCommand.Execute();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        OnPointerUpCommand.Execute();
    }
}