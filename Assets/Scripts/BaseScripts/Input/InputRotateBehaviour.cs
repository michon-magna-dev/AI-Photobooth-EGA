using UnityEngine;

public class InputRotateBehaviour : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 100.0f;
    [SerializeField] float rotationSpeedMultiplier = 1;
    [Range(0, 1)]
    [SerializeField] float rotationDamping = 0.95f; // Adjust this value to control damping
    [SerializeField] Vector2 lastInputPosition;
    [SerializeField] float currentRotationSpeed;

    private void Start()
    {
        rotationDamping = ConfigManager.GetInstance().GetFloatValue("INPUT_ROTATION_DAMPING");
        rotationSpeed = ConfigManager.GetInstance().GetFloatValue("INPUT_ROTATION_SWIPE_VALUE");
        rotationSpeedMultiplier = ConfigManager.GetInstance().GetFloatValue("INPUT_ROTATION_MULTIPLIER");
    }


    private void LateUpdate()
    {
        // Check for touch input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Check for the beginning of a swipe
            if (touch.phase == TouchPhase.Began)
            {
                lastInputPosition = touch.position;
                currentRotationSpeed = 0.0f; // Reset rotation speed
            }
            // Check for the end of a swipe
            else if (touch.phase == TouchPhase.Moved)
            {
                // Calculate the swipe direction
                Vector2 swipeDirection = touch.position - lastInputPosition;

                // Calculate rotation based on swipe direction
                float rotationAmount = -swipeDirection.x * rotationSpeed * rotationSpeedMultiplier * Time.deltaTime;

                // Add the rotation to the current speed (dampened)
                currentRotationSpeed += rotationAmount;

                // Dampen the rotation speed
                currentRotationSpeed *= rotationDamping;

                // Rotate the object around its up axis (Y-axis)
                transform.Rotate(Vector3.up, currentRotationSpeed * Time.deltaTime, Space.World);

                // Update the last input position
                lastInputPosition = touch.position;
            }
            if (touch.phase == TouchPhase.Ended)
            {
                lastInputPosition = Vector2.zero;
            }
        }
        // Check for mouse input
        else if (Input.GetMouseButtonDown(0))
        {
            lastInputPosition = Input.mousePosition;
            currentRotationSpeed = 0.0f; // Reset rotation speed
        }
        else if (Input.GetMouseButton(0))
        {
            // Calculate the mouse swipe direction
            Vector2 mouseSwipeDirection = (Vector2)Input.mousePosition - lastInputPosition;

            // Calculate rotation based on mouse swipe direction
            float rotationAmount = -mouseSwipeDirection.x * rotationSpeed * rotationSpeedMultiplier * Time.deltaTime;

            // Add the rotation to the current speed (dampened)
            currentRotationSpeed += rotationAmount;

            // Dampen the rotation speed
            currentRotationSpeed *= rotationDamping;

            // Rotate the object around its up axis (Y-axis)
            transform.Rotate(Vector3.up, currentRotationSpeed * Time.deltaTime, Space.World);

            // Update the last input position
            lastInputPosition = Input.mousePosition;
        }
         if (Input.GetMouseButtonUp(0))
        {
            lastInputPosition = Vector2.zero;
        }
    }
}