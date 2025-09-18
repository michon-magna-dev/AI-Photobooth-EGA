using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FadeInMask : MonoBehaviour
{
    public float duration = 1f; // Duration of the fade-in effect in seconds
    private Material material;
    private Color originalColor;

    private void Start()
    {
        // Get the material of the sprite
        material = GetComponent<SpriteRenderer>().material;

        // Store the original color and set the alpha to 0 for full transparency at the start
        originalColor = material.color;
        material.color = new Color(1f, 1f, 1f, 0f);
        Reset();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O)) 
        {
            FadeIn();
        }
    }

    public async Task FadeIn(float p_fadeDuration = 1)
    {
        float elapsedTime = 0f;

        while (elapsedTime < p_fadeDuration)
        {
            float alpha = Mathf.Clamp01(elapsedTime / p_fadeDuration);
            material.color = new Color(1f, 1f, 1f, alpha);
            elapsedTime += Time.deltaTime;
            //Debug.Log($"Fade Alpha {alpha}");
            await Task.Yield();
        }

        // Ensure the final alpha is set to 1 after the fade-in is complete
        material.color = new Color(1f, 1f, 1f, 1f);
    }

    public void Reset()
    {
        if (material == null)
        {
            material = GetComponent<SpriteRenderer>().material;
        }
        material.color = new Color(1f, 1f, 1f, 0f);
    }
}

