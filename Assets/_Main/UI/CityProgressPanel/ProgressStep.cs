using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CityProgressStep : MonoBehaviour
{
    [SerializeField] private TMP_Text stageNumberText;
    [SerializeField] private Image icon;

    public void Init(Sprite sprite, int stageNumber)
    {
        icon.sprite = sprite;
        stageNumberText.text = stageNumber.ToString();
    }
}
