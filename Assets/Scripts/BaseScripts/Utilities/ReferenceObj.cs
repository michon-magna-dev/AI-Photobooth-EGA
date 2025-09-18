using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReferenceObj : MonoBehaviour
{
    public Image uiImage;
    public bool disableOnStart = false;

    private void OnValidate()
    {
        if (uiImage == null)
        {
            uiImage = GetComponent<Image>();
        }
    }

    void Start()
    {
#if !UNITY_EDITOR
    Destroy(gameObject);
#else
        uiImage.raycastTarget = false;
        if (disableOnStart)
            gameObject.SetActive(false);
#endif
    }

}