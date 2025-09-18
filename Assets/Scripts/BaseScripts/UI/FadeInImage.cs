using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class FadeInImage : MonoBehaviour
{
    public float fadeInDuration = 1.0f;
    private RawImage image;

    private void Start()
    {
        image = GetComponent<RawImage>();
        //FadeIn();
    }

    public void StartFading()
    {
        Color originalColor = image.color;
        originalColor.a = 0f;
        image.color = originalColor;
    }

    public void FadeIn()
    {
        image.DOFade(1f, fadeInDuration);
    }
}
