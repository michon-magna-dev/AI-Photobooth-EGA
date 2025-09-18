using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardEnabler : MonoBehaviour
{
    [SerializeField] Transform keyboardTransform;
    [SerializeField] GameObject oskObj;

    [SerializeField] float yMinPosition = 1;
    [SerializeField] float yMaxPosition = 0;

    [SerializeField] private RectTransform uiElement;  // The UI element to slide
    [SerializeField] private float slideSpeed = 5.0f;  // Speed of the sliding animation
    [SerializeField] private float hiddenYPosition = -200.0f; // Off-screen position
    [SerializeField] private float visibleYPosition = 0.0f;   // On-screen position

    private bool isVisible = false; // Is the UI element currently visible?

    void Start()
    {
        keyboardTransform = this.transform;
        if (uiElement != null)
        {
            uiElement.anchoredPosition = new Vector2(uiElement.anchoredPosition.x, hiddenYPosition);
        }
    }


    void Update()
    {
        // Slide the UI element to its target position
        float targetY = isVisible ? visibleYPosition : hiddenYPosition;
        uiElement.anchoredPosition = Vector2.Lerp(
            uiElement.anchoredPosition,
            new Vector2(uiElement.anchoredPosition.x, targetY),
            slideSpeed * Time.deltaTime
        );
    }

    // Show the UI element
    public void Show()
    {
        isVisible = true;
    }

    // Hide the UI element
    public void Hide()
    {
        isVisible = false;
    }

    // Toggle visibility of the UI element
    public void ToggleVisibility()
    {
        isVisible = !isVisible;
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(0, 150, 50, 50), "Show Keyboard"))
            Show();   
        if (GUI.Button(new Rect(0, 200, 50, 50), "Hide Keyboard"))
            Hide();
    }
}