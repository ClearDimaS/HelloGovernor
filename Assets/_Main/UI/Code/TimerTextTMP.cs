using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public abstract class TimerText : MonoBehaviour
{
    [SerializeField] private float scaleTime = 0.1f;
    [SerializeField] private float scale = 1.15f;

    protected abstract Transform Transform { get; }
    private int lastValue = -1;
    private static Dictionary<int, string> timeStrings = new();
    
    private string GetTimeString(int v)
    {
        if (!timeStrings.ContainsKey(v))
        {
            var str = v.ToString();
            if (str.Length == 1)
            {
                str = "0" + str;
            }

            timeStrings[v] = str;
        }

        return timeStrings[v];
    }

    public void SetValue(float value)
    {
        SetValue(Mathf.RoundToInt(value));
    }
    public void SetValue(int value)
    {
        if (lastValue != value)
        {
            var stringValue = GetTimeString(value);
            lastValue = value;
            Transform.DOScale(Vector3.one * scale, scaleTime).OnComplete(() =>
            {
                ApplyText(stringValue);
                Transform.DOScale(Vector3.one, scaleTime);
            });
        }
    }

    protected abstract void ApplyText(string value);
}
public class TimerTextTMP : TimerText
{
    [SerializeField] private TMP_Text text;
    
    protected override Transform Transform => text.transform;
    
    protected override void ApplyText(string stringValue)
    {
        text.text = stringValue;
    }
}