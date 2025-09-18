using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSphere : MonoBehaviour
{
    [SerializeField] Vector3 rotationSpeed = new Vector3(0, 30, 0); // Rotation speed in degrees per second
    [SerializeField] float rotationSpeedMultiplier = 1; // Rotation speed in degrees per second

    private void Start()
    {
        rotationSpeedMultiplier = ConfigManager.GetInstance().GetFloatValue("AUTO_ROTATION_MULTIPLIER");
    }

    void Update()
    {
        transform.Rotate(rotationSpeed * rotationSpeedMultiplier * Time.deltaTime);
    }

}