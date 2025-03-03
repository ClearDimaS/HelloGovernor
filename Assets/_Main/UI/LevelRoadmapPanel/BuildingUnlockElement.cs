using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingUnlockElement : MonoBehaviour
{
    [SerializeField] private Image icon;

    public void Init(Sprite sprite, bool bought)
    {
        icon.sprite = sprite;
    }
}