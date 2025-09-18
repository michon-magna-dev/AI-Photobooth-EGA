using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Video;

public class ScreenSaverAnimation : MonoBehaviour
{
    [SerializeField] AnimationManager animationManager;
    [SerializeField] PhotoGridManager photoGridManager;
    [SerializeField] VideoPlayer beyondFutures;

    [SerializeField] GameObject photoPreview2D;
    [Header("Overlay")]
    [SerializeField] GameObject overlaySprite;
    [Header("Timings")]
    [SerializeField] float timeShowBeyondFutures = 10;
    [SerializeField] float showYpoLogoTime = 3;
    [SerializeField] float show2ndOvelayTime = 3;

    private CancellationTokenSource _cancellationTokenSource;
    public bool isPlayingAnimation = false;

    #region Lifecycles
    void Start()
    {
        timeShowBeyondFutures = ConfigManager.GetInstance().GetFloatValue("SCREENSAVER_SHOW_BEYOND_LOGO_TIME");
        showYpoLogoTime = ConfigManager.GetInstance().GetFloatValue("SCREENSAVER_SHOW_YPO_LOGO_TIME");
        show2ndOvelayTime = ConfigManager.GetInstance().GetFloatValue("SCREENSAVER_SHOW_OVERLAY_LOGO_TIME");



        isPlayingAnimation = false;

        HUDManager.GetInstance().OnScreenChange += OnScreenSaverExit;
        _cancellationTokenSource = new CancellationTokenSource();
        ResetUI();

        //PlayAnimation();
        StartMainCoroutineAnimation();
    }

    #endregion

    #region Animation Functions

    public void StartMainCoroutineAnimation()
    {
        photoPreview2D.gameObject.SetActive(false);
        if (!isPlayingAnimation)
        {
            StartCoroutine(PlayAnimationCoroutine());
        }
    }

    public IEnumerator PlayAnimationCoroutine()
    {
        Debug.Log($"Screen Saver Animation Started");
        isPlayingAnimation = true;
        var hudManager = HUDManager.GetInstance();
        hudManager.ShowEnvironmentObjects(false);
        ResetOverlayOpacity();

        while (isPlayingAnimation)
        {
            beyondFutures.gameObject.SetActive(true);
            beyondFutures.Play();

            hudManager.ShowEnvironmentObjects(false);
            hudManager.ShowYPOMask(false);


            yield return new WaitForSeconds(timeShowBeyondFutures);

            Debug.Log($"SS: show Images");
            beyondFutures.gameObject.SetActive(false);
            hudManager.ShowEnvironmentObjects(true);
            ShowImages();
            ShowImageGradients(true);
            FadeOutImagesSequence(photoGridManager.GetPhotoList);
            //await FadeOutImagesSequence(photoGridManager.GetPhotoList);
            //await FadeOutImagesSequenceAsync(photoGridManager.GetPhotoList);

            yield return new WaitForSeconds(showYpoLogoTime);

            Debug.Log($"ss: show ypo logo");
            ShowImageGradients(false);

            hudManager.ShowYPOMask(true);
            SetPhotosMask(photoGridManager.GetPhotoList, SpriteMaskInteraction.VisibleInsideMask);

            Debug.Log($"ss: Show 2nd Overlay");
            StartFadeOverlay();// overlays new mask

            yield return new WaitForSeconds(show2ndOvelayTime);
            Debug.Log($"ss: reset:");
            //HideImages(photoGridManager.GetPhotoList);

            beyondFutures.gameObject.SetActive(true);
            hudManager.ShowYPOMask(false);
            hudManager.ShowEnvironmentObjects(true);
            ResetOverlayOpacity();
            ShowImages();

            //yield return new WaitForEndOfFrame();
        }
        Debug.Log($"ss: ss stopped");

        isPlayingAnimation = false;
    }

    #endregion

    #region Private Functions

    public void ResetUI()
    {
        beyondFutures.gameObject.SetActive(true);
        photoPreview2D.SetActive(true);
        beyondFutures.gameObject.SetActive(true);
        HUDManager.GetInstance().ShowEnvironmentObjects(false);
    }

    public void ResetOverlayOpacity()
    {
        //var fadeIn = overlaySprite.GetComponent<FadeInSprite>();
        //fadeIn.ResetOpacity();
        var fadeIn = overlaySprite.GetComponent<FadeInMask>();
        fadeIn.Reset();
    }

    #endregion

    #region Image Fading 
    public void FadeOutImagesSequence(GameObject[] p_photoList)
    {
        foreach (GameObject p in p_photoList)
        {
            //removed but can bring back later
            //if (p.transform.GetSiblingIndex() == animationManager.GetSelectedImagePreviewIndex)
            //{
            //    continue;
            //}
            var TargetPhoto = p.GetComponent<TargetPhoto>();
            TargetPhoto.FadePhotoImage(true);
        }
    }

    public async Task StartFadeOverlay()
    {
        var fadeIn = overlaySprite.GetComponent<FadeInMask>();
        await fadeIn.FadeIn();
    }

    public async Task FadeOutImagesSequenceAsync(GameObject[] p_photoList)
    {
        foreach (GameObject p in p_photoList)
        {
            if (p.transform.GetSiblingIndex() == animationManager.GetSelectedImagePreviewIndex)
            {
                continue;
            }
            var TargetPhoto = p.GetComponent<TargetPhoto>();
            TargetPhoto.gameObject.SetActive(true);
            await TargetPhoto.FadePhotoImage(true, 1);
            //await Task.Delay(100);
        }
    }

    public void FadeInImages(bool p_fadeIn = true)
    {
        foreach (GameObject p in photoGridManager.GetPhotoList)
        {
            if (p.transform.GetSiblingIndex() == animationManager.GetSelectedImagePreviewIndex)
            {
                continue;
            }
            var TargetPhoto = p.GetComponent<TargetPhoto>();
            TargetPhoto.FadePhotoImage(p_fadeIn);
        }
    }

    #endregion

    #region Image Object Functions

    public void ShowImageGradients(bool p_enable)
    {
        foreach (GameObject p in photoGridManager.GetPhotoList)
        {
            var TargetPhoto = p.GetComponent<TargetPhoto>();
            TargetPhoto.EnableGradient(p_enable);

            photoPreview2D.GetComponent<TargetPhoto>().EnableGradient(p_enable);
        }
    }

    public void ShowImages()
    {
        foreach (GameObject p in photoGridManager.GetPhotoList)
        {
            var TargetPhoto = p.GetComponent<TargetPhoto>();
            TargetPhoto?.gameObject.SetActive(true);
            TargetPhoto.SetImageOpacity(1);
        }
    }

    public void HideImages(GameObject[] p_photoList)
    {
        foreach (GameObject p in p_photoList)
        {
            var TargetPhoto = p.GetComponent<TargetPhoto>();
            TargetPhoto.FadePhotoImage(false);
        }
    }

    public void SetPhotosMask(GameObject[] p_photoList, SpriteMaskInteraction p_maskInteraction = SpriteMaskInteraction.None)
    {
        foreach (GameObject p in p_photoList)
        {

            var TargetPhoto = p.GetComponent<TargetPhoto>();
            TargetPhoto.SetMask(p_maskInteraction);
        }
    }

    #endregion

    #region Debug

    private void OnGUI()
    {
        if (DebugBehaviour.debugModeOn)
        {

            if (GUI.Button(new Rect(100, 100, 100, 100), "Play ScreenSaver"))
            {
                StartMainCoroutineAnimation();
            }

        }
    }

    #endregion

    public void OnScreenSaverExit(CURRENT_SCREEN p_currentScreen)
    {
        isPlayingAnimation = false;
        StopAllCoroutines();
        CancelScreenScaverAnimation();
        if (p_currentScreen.Equals(CURRENT_SCREEN.PICTURE_MODE))
        {
            HUDManager.GetInstance().ShowEnvironmentObjects(false);

        }
        //ResetUI();
        Debug.Log($"ScreenSaverANimation stopped");
    }

    void CancelScreenScaverAnimation()
    {
        if (_cancellationTokenSource != null)
        {
            _cancellationTokenSource.Cancel();
        }
    }

    void OnApplicationQuit()
    {
        CancelScreenScaverAnimation();
    }

}