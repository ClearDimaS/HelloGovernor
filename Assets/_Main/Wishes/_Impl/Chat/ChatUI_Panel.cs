using UnityEngine;
using UnityEngine.UI;

public class ChatUI_Panel : UI_Panel
{
    [SerializeField] private Button endButton;

    protected override void OnAwake()
    {
        base.OnAwake();
        endButton.onClick.AddListener(Hide);
    }
}