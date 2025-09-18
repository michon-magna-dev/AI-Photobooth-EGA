using DG.Tweening;
using UnityEngine;

public class FadeInSprite : MonoBehaviour
{
    public float fadeInDuration = 1.0f;
    private SpriteRenderer image;

    private void Start()
    {
        image = GetComponent<SpriteRenderer>();
        Color originalColor = image.color;
        originalColor.a = 0f;
        image.color = originalColor;

        
    }

    public void StartFading()
    {
        Color originalColor = image.color;
        originalColor.a = 0f;
        image.color = originalColor;
    }

    public void ResetOpacity()
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