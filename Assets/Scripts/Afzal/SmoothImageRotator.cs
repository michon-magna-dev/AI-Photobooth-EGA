using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SmoothImageRotator : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationSpeed = 90f; // degrees per second
    public bool continuousRotation = true;
    public bool clockwise = true;

    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (continuousRotation)
        {
            RotateContinuously();
        }
    }

    // Continuous rotation
    void RotateContinuously()
    {
        float direction = clockwise ? -1f : 1f;
        rectTransform.Rotate(0, 0, rotationSpeed * direction * Time.deltaTime);
    }
}