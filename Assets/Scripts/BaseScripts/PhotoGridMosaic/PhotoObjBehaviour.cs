using UnityEngine;

public class PhotoObjBehaviour : MonoBehaviour
{
    [SerializeField] GameObject lookatTarget;
    [SerializeField] Vector3 lookAtOffset;

    void Start()
    {

    }
    
    void LateUpdate()
    {
        if (lookatTarget != null)
            transform.LookAt(lookatTarget.transform.position + lookAtOffset);
    }

}