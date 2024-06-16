using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class DayTimePanel : UI_Panel
{
    [Inject] private DayTimeManager dayTimeManager;
    
    [SerializeField] private RectTransform dayNightCircle;
    [SerializeField] private Image bg;
    [SerializeField] private Color bgNight;
    [SerializeField] private Color bgDay;
    [SerializeField] private TMP_Text clockTime;

    private int lastMM = -1;
    
    private void Update()
    {
        var up = Quaternion.Euler(0, 0, -dayTimeManager.GetDaytT() * 360f) * Vector3.up;
        dayNightCircle.localRotation = Quaternion.LookRotation(Vector3.forward, up);
        var isNight = dayTimeManager.IsLampsEnabled;
        bg.color = isNight ? bgNight : bgDay;
        var mins = Mathf.RoundToInt(dayTimeManager.GetDaytT() * 24 * 60);
        var hh = mins / 60;
        var mm = mins % 60;
        if (mm != lastMM)
        {
            var add = mins > 779 ? "PM" : "AM";
            if (mins > 779)
            {
                hh -= 12;
            }

            var mmString = mm.ToString();
            if (mmString.Length < 2)
            {
                mmString = "0" + mmString;
            }
            clockTime.text = $"{hh}:{mmString}\n{add}";
            lastMM = mm;
        }
    }
}