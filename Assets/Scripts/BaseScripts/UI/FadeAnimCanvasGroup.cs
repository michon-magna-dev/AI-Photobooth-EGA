using System;
using System.Threading.Tasks;
using UnityEngine;

public class FadeAnimCanvasGroup : MonoBehaviour
{
    private static FadeAnimCanvasGroup _instance;
    public static FadeAnimCanvasGroup Instance => _instance;

    public CanvasGroup overlayImage;
    public float timeToFade = 2;
    public bool isFading = false;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        isFading = false;
        overlayImage.alpha = 0;

        //sceneHandler.sceneLoadRequested += SceneRequested;
        //sceneHandler.sceneCompleted += SceneCompleted;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            FadeIn();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            FadeOut();
        }
    }

    private void SceneCompleted()
    {
        FadeOut();
    }

    private void SceneRequested()
    {
        FadeIn();
    }

    public async void FadeIn()
    {
        if (!isFading)
        {
            isFading = true;
            await StartFadeInAsync();
            isFading = false;
            await StartFadeOutAsync();
            Debug.Log($"FadeIn Finished");
        }
    }

    public async void FadeOut()
    {
        if (!isFading)
        {
            isFading = true;
            await StartFadeOutAsync();
            isFading = false;
            Debug.Log($"FadeOut Finished");
        }

    }

    public async void FadeOut(float p_delay = 0)
    {
        if (!isFading)
        {
            isFading = true;
            await StartFadeOutAsync(p_delay);
            isFading = false;
        }
    }

    public async Task FadeOutTask()
    {
        if (!isFading)
        {
            isFading = true;
            await StartFadeOutAsync();
            isFading = false;
        }
    }

    private async Task StartFadeInAsync()
    {
        float elapsedTime = 0f;
        //Color targetColor = overlayImage.color;
        float targetAlpha = 1;
        //targetColor.a = 1f; // Set alpha to 1 for fully visible

        while (elapsedTime < timeToFade)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(overlayImage.alpha, targetAlpha, elapsedTime / timeToFade);
            overlayImage.alpha =  alpha;
            await Task.Yield();
        }
        //await Task.Delay(TimeSpan.FromSeconds(1));
     
        overlayImage.alpha = 1;
    }

    private async Task StartFadeOutAsync(float p_delayBeforeFade = 0)
    {
        await Task.Delay(TimeSpan.FromSeconds(p_delayBeforeFade));

        float elapsedTime = 0f;
        float targetAlpha = 0;

        while (elapsedTime < timeToFade)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(overlayImage.alpha, targetAlpha, elapsedTime / timeToFade);
            overlayImage.alpha = alpha;
            await Task.Yield();
        }
        overlayImage.alpha = 0;
    }

}