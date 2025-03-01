using UnityEngine;
using UnityEngine.UI;

public class RectScroller : MonoBehaviour
{
    public RectTransform rectTransform;
    
    public float scrollSpeedX = 1f;     // Speed of scrolling along the X axis
    public float scrollSpeedY = 1f;     // Speed of scrolling along the Y axis

    private Vector2 screenSize;

    void Start()
    {
        // Get the size of the screen in world units
        screenSize = new Vector2(rectTransform.rect.width, rectTransform.rect.height);
        rectTransform.anchorMin = Vector2.one / 2f;
        rectTransform.anchorMax = Vector2.one / 2f;
        rectTransform.sizeDelta = screenSize * 3f;
    }

    void Update()
    {
        // Calculate the new position based on speed and time
        float deltaX = scrollSpeedX * Time.deltaTime;
        float deltaY = scrollSpeedY * Time.deltaTime;

        // Update the position of the RectTransform
        rectTransform.anchoredPosition += new Vector2(deltaX, deltaY);

        // Check if the RectTransform has moved beyond the screen size
        if (Mathf.Abs(rectTransform.anchoredPosition.x) > screenSize.x)
        {
            // Reset position to zero
            rectTransform.anchoredPosition -= Mathf.Sign(scrollSpeedX) * new Vector2(screenSize.x, 0);
        }
        if (Mathf.Abs(rectTransform.anchoredPosition.y) > screenSize.y)
        {
            // Reset position to zero
            rectTransform.anchoredPosition -= Mathf.Sign(scrollSpeedY) * new Vector2(0, screenSize.y);
        }
    }
}