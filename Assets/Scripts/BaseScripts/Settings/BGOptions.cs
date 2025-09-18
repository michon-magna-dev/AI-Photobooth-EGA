using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BGOptions : MonoBehaviour
{
    public KeyCode rotate90Key = KeyCode.R;  
    public KeyCode rotate180Key = KeyCode.E; 
    public RectTransform imageToRotate;
    private int currentRotation = 0; 

    private void Update()
    {
        // Rotate 90 degrees when the 'rotate90Key' is pressed.
        if (Input.GetKeyDown(rotate90Key))
        {
            currentRotation += 90;
            if (currentRotation > 270)
            {
                currentRotation = 0;
            }
            RotateImageTo(currentRotation);
        }

        if (Input.GetKeyDown(rotate180Key))
        {
            currentRotation += 180;
            if (currentRotation > 270)
            {
                currentRotation = 0;
            }
            RotateImageTo(currentRotation);
        }
    }

    private void RotateImageTo(float angle)
    {
        if (imageToRotate != null)
        {
            imageToRotate.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}

