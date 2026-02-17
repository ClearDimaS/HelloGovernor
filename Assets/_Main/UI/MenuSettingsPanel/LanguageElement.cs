using System;
using UnityEngine;
using UnityEngine.UI;

public class LanguageElement : MonoBehaviour
{
    [SerializeField] private Button selectButton;
    [SerializeField] private Image iconImage;
    [SerializeField] private GameObject selectedGO;

    private Action onSelect;
    public string Code { get; private set; }

    private void Start()
    {
        selectButton.onClick.AddListener(() => onSelect?.Invoke());
    }

    public void Init(string code, Action onSelect, Sprite icon)
    {
        Code = code;
        iconImage.sprite = icon;
        this.onSelect = onSelect;
    }

    public void SetSelected(bool isSelected)
    {
        selectedGO.SetActiveOnce(isSelected);
    }
}