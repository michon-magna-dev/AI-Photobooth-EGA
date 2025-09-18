using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FloatingLookAt : MonoBehaviour
{
    [SerializeField] Transform m_target;
    [SerializeField] Vector3 lookOffset;

    void Update()
    {
        transform.LookAt(Camera.main.transform, Vector3.up);
        transform.rotation = Quaternion.Euler(transform.eulerAngles + lookOffset);
    }

}