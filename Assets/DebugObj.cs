using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugObj : MonoBehaviour
{
    public bool showInEditorPlayMode = false;
    // Start is called before the first frame update
    void Start()
    {
#if !UNITY_EDITOR
        gameObject.SetActive(false);
#endif
    }

    // Update is called once per frame
    void Update()
    {

    }
}
