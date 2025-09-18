using UnityEngine;

public class RotorBehaviour : MonoBehaviour
{
    public float rotatorSpeed = 1.0f;
    public Vector3 rotationVector = Vector3.zero;

    void Update()
    {
        transform.Rotate(rotationVector);
    }

}
