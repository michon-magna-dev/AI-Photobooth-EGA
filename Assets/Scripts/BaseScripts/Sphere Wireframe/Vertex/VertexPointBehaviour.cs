using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexPointBehaviour : MonoBehaviour
{
    public VertexPoints m_vertexPoint;

    public Vector3 startPoint;   // The starting position
    public Vector3 endPoint;     // The ending position

    public float lerpSpeed = 1.0f; // The speed of the interpolation

    public float speedRangeMinValue = 0.8f;  // The minimum value
    public float speedRangeMaxValue = 1.0f;  // The maximum value

    public float minValue = 0.8f;  // The minimum value
    public float maxValue = 1.0f;  // The maximum value
    
    public float t;

    private void Start()
    {
        startPoint = m_vertexPoint.GetSurfacePoints()[transform.GetSiblingIndex()];
        endPoint = m_vertexPoint.GetOutwardSurfacePoints()[transform.GetSiblingIndex()];
        t = 0;

        //lerpSpeed = Random.Range(0.1f, 0.2f);
        lerpSpeed = Random.Range(speedRangeMinValue, speedRangeMaxValue);
    }

    private void FixedUpdate()
    {
        startPoint = m_vertexPoint.GetSurfacePoints()[transform.GetSiblingIndex()];
        endPoint = m_vertexPoint.GetOutwardSurfacePoints()[transform.GetSiblingIndex()];
        
        t = Mathf.PingPong(Time.time * lerpSpeed, maxValue - minValue) + minValue;

        transform.position = Vector3.Lerp(startPoint, endPoint, t);
    }
}