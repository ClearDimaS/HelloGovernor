using UnityEngine;

public class TimerTextMesh : TimerText
{
    [SerializeField] private TextMesh text;
    
    protected override Transform Transform => text.transform;
    
    protected override void ApplyText(string stringValue)
    {
        text.text = stringValue;
    }
}