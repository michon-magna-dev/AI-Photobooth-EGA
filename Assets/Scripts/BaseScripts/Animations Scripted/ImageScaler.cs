using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageScaler : MonoBehaviour
{
    public Image image; // Reference to the Image component you want to scale
    public float targetScale = 2f; // The target scale you want to lerp to
    public float lerpDuration = 1f; // The duration of the lerp
    public float delayDuration = 2f; // The delay before lerping back to the original size

    private Vector3 startScale; // Store the initial scale of the image

    void Start()
    {
        // Get the initial scale of the image
        startScale = image.transform.localScale;

        // Start the coroutine when the script is initialized
        //StartCoroutine(LerpScale());
    }

    public void ScaleImage(float p_duration)
    {
        StopCoroutine("LerpScale");
        StartCoroutine(LerpScale(p_duration));
    }

    public void ScaleImage(float target, float p_duration)
    {
        StopCoroutine("LerpToScale");
        StartCoroutine(LerpToScale(target, p_duration));
    }

    public void ReturnToOriginal()
    {
        StopCoroutine("LerpScale");
        StartCoroutine(LerpToScale(startScale.x, lerpDuration));
    }

    IEnumerator LerpScale(float p_duration)
    {
        // Lerp to the target scale
        yield return LerpToScale(targetScale, lerpDuration);

        // Wait for the specified delay duration
        yield return new WaitForSeconds(p_duration);

        // Lerp back to the original scale
        yield return LerpToScale(startScale.x, lerpDuration);
    }

    IEnumerator LerpToScale(float target, float duration)
    {
        // Time elapsed during the lerp
        float elapsedTime = 0f;

        // Lerp until the elapsed time reaches the specified duration
        while (elapsedTime < duration)
        {
            // Calculate the lerp factor based on the elapsed time
            float t = elapsedTime / duration;

            // Lerp the scale of the image
            image.transform.localScale = Vector3.Lerp(startScale, new Vector3(target, target, 1f), t);

            // Increment the elapsed time
            elapsedTime += Time.deltaTime;

            // Wait for the next frame
            yield return null;
        }

        // Ensure the image reaches the exact target scale
        image.transform.localScale = new Vector3(target, target, 1f);
    }
}
