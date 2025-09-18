using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangleAlphaLoop : MonoBehaviour
{
    MeshRenderer render;
    [Range(0, 0.10f)]
    public float alpha;
    public float randomSpeed;

    public float maxAlpha = 0.05f;
    public float minAlpha = 0.1f;

    public Color currentRenderColor;
    public bool useAlphaLoop = true;
    
    void Start()
    {
        render = GetComponent<MeshRenderer>();
        randomSpeed = Random.Range(0.05f, 0.1f);
        Color currentRenderColor = render.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate alpha using Mathf.Sin to create a smooth wave from 0 to 1.
        //alpha = Mathf.Sin(Time.time * randomSpeed) * 0.25f + 0.25f;

        //// Create a new color with the calculated alpha.
        //Color currentRenderColor = render.material.color;
        //Color newColor = new Color(currentRenderColor.r, currentRenderColor.g, currentRenderColor.b, alpha);

        //// Set the new color to the material.
        //render.material.SetColor("_Color", newColor);
        float calcAlpha = alpha;
        if (useAlphaLoop)
            calcAlpha = Mathf.Lerp(minAlpha, maxAlpha, Mathf.Sin(Time.time * randomSpeed) * 0.25f + 0.25f);

        // Calculate alpha using Mathf.Sin to create a smooth wave from 0 to 1.
        Color currentRenderColor = render.material.color;
        Color newColor = new Color(currentRenderColor.r, currentRenderColor.g, currentRenderColor.b, calcAlpha);

        // Set the new color to the material.
        render.material.SetColor("_BaseColor", newColor);
    }
}
