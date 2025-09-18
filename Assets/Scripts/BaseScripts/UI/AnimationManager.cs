using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class AnimationManager : MonoBehaviour
{
    [SerializeField] PhotoGridManager photoGridManager;
    [SerializeField] ScreenShotEditable screenCapture;
    [SerializeField] VertexPoints sphereVertexHandler;
    [SerializeField] UploadImage imageUploadHandler;
    private CancellationTokenSource _cancellationTokenSource;

    [Space]
    [Header("ANIMATION OBJS")]
    [Space]
    [Space]
    [Header("Image Preview")]
    [SerializeField] Button takePictureButton;
    [SerializeField] GameObject photoPreview2D;
    [SerializeField] Vector3 photoPreviewDefaultPosition = Vector3.one;
    [SerializeField] Vector3 photoPreviewDefaultScale = Vector3.one;
    [SerializeField] GameObject ledUIObjects;
    [Header("Zoom Out")]
    Vector3 originalScale;
    [SerializeField] Vector3 zoomedOutTargetTransform = new(0.5f, 0.5f, 0.5f);
    //[SerializeField] float spinTimeLimit = 3f;
    [Header("Image Grid")]
    [SerializeField] int imgGridIndex = 160 / 2;
    [Header("Overlay")]
    [SerializeField] GameObject overlaySprite;
    [Space]
    [Header("Animation Timings")]
    [SerializeField] float imgPreviewTimeLimit = 3f;
    [SerializeField] float sphereSpinDisplayTime = 6;
    [SerializeField] float overlayFadeDelayTime = 5;

    public int GetSelectedImagePreviewIndex => imgGridIndex;
    public bool debugFade = true;

    #region LifeCycles
    void Start()
    {
        imgPreviewTimeLimit = ConfigManager.GetInstance().GetFloatValue("SELFIE_ANIM_PREVIEW_TIME");
        sphereSpinDisplayTime = ConfigManager.GetInstance().GetFloatValue("SELFIE_ANIM_SPHERE_SPIN_TIME");
        overlayFadeDelayTime = ConfigManager.GetInstance().GetFloatValue("SELFIE_ANIM_OVERLAY_FADE_IN_TIME");
        imgGridIndex = ConfigManager.GetInstance().GetValue("PREVIEW_IMAGE_GRID_INDEX");

        _cancellationTokenSource = new CancellationTokenSource();
        screenCapture.OnCapturedPhoto += OnPhotoCaptured;
        Debug.Log($"Screen Resolution used: {Screen.width}x{Screen.height}");
        //ShowEnvironmentObjects(false);
        HideImages();

        Reset();
        originalScale = photoPreview2D.transform.localScale;
    }

    private void Update()
    {
#if UNITY_EDITOR

        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log($"Set Photos to Sphere..");
            SetPhotosToSphere(photoGridManager.GetPhotoList);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log($"Set Photos to Grid..");
            SetPhotosToGrid(photoGridManager.GetPhotoList);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            Debug.Log($"Playing Animation..");
            PlayAnimation();
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log($"Fading Images..");
            FadeInImages(debugFade);
        }
#endif

    }

    public void FadeInImages(bool p_fadeIn = true)
    {

        foreach (GameObject p in photoGridManager.GetPhotoList)
        {
            if (p.transform.GetSiblingIndex() == imgGridIndex)
            {
                continue;
            }
            var TargetPhoto = p.GetComponent<TargetPhoto>();
            if (!TargetPhoto.isActiveAndEnabled)
            {
                TargetPhoto.gameObject.SetActive(true);
            }
            TargetPhoto.FadePhotoImage(p_fadeIn);
        }
    }

    public void HideImages()
    {
        foreach (GameObject p in photoGridManager.GetPhotoList)
        {
            var TargetPhoto = p.GetComponent<TargetPhoto>();
            TargetPhoto.SetImageOpacity(0);
        }
    }

    //private void OnDisable()
    //{
    //    photoPreview2D.transform.localScale = originalScale;
    //}

    //private void OnApplicationQuit()
    //{
    //    photoPreview2D.transform.localScale = originalScale;
    //}

    #endregion

    private void OnPhotoCaptured()
    {
        PlayAnimation();
        Debug.Log($"Playing Animation..");
    }

    public async void PlayAnimation()
    {
        takePictureButton.interactable = false;
        var hudManager = HUDManager.GetInstance();
        Debug.Log($"PhotoTaken");
        HideImages();
        Reset();
        hudManager.ShowYPOMask(false);
        hudManager.ShowFinalPreviewImages(true);
        hudManager.ShowEnvironmentObjects(true);
        ShowImageGradients(true);

        photoPreview2D.SetActive(true);
        ledUIObjects.SetActive(false);

        await Task.Delay(TimeSpan.FromSeconds(1));
        ShowImages();
        FadeInImages(true);

        //stay for n seconds
        Debug.Log($"Staying for n seconds");
        await Task.Delay(TimeSpan.FromSeconds(imgPreviewTimeLimit));

        // Zoom Out
        Debug.Log($"Zooming out..");
        await ZoomOutPhoto();

        // Set Target
        Debug.Log($"Set Photo Obj Target position to Sphere");
        SetImagePreviewToSphere();

        // Start Tornado Spin
        Debug.Log($"Spinning Sphere");
        SetPhotosToSphere(photoGridManager.GetPhotoList);

        //Back To Grid
        await Task.Delay(TimeSpan.FromSeconds(sphereSpinDisplayTime));
        SetPhotosToGrid(photoGridManager.GetPhotoList);
        SetImagePreviewToGrid();

        //Show Overlay
        await Task.Delay(TimeSpan.FromSeconds(overlayFadeDelayTime));
        ShowImageGradients(false);
        hudManager.ShowYPOMask(true);
        SetPhotosMask(photoGridManager.GetPhotoList, SpriteMaskInteraction.VisibleInsideMask);

        //Overlay Fade In
        await StartFadeOverlay();
        await Task.Delay(TimeSpan.FromSeconds(2f));
        takePictureButton.interactable = true;

        hudManager.ShowScreen(CURRENT_SCREEN.RETAKE_MODE);
        hudManager.ShowFinalPreviewImages(true);

        SetImagePreviewToGrid();

        imageUploadHandler.StartUpload();

        Debug.Log("Play Anim Async task stopped.");
    }

    public void DisableTakePicture(bool p_enable = true)
    {
        takePictureButton.interactable = p_enable;

    }

    public void ShowImages()
    {
        foreach (GameObject p in photoGridManager.GetPhotoList)
        {
            var TargetPhoto = p.GetComponent<TargetPhoto>();
            TargetPhoto.SetImageOpacity(1);
        }
    }

    public void ShowImageGradients(bool p_enable)
    {
        photoPreview2D.GetComponent<TargetPhoto>().EnableGradient(p_enable);
        foreach (GameObject p in photoGridManager.GetPhotoList)
        {
            var TargetPhoto = p.GetComponent<TargetPhoto>();
            TargetPhoto.EnableGradient(p_enable);
        }
    }

    public async Task ZoomOutPhoto()
    {
        Vector3 targetScale = zoomedOutTargetTransform;

        float duration = 1.5f;
        float elapsed = 0f;

        originalScale = photoPreview2D.transform.localScale;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            photoPreview2D.transform.localScale = Vector3.Lerp(originalScale, targetScale, progress);
            await Task.Yield();
        }
        photoPreview2D.transform.localScale = targetScale;
    }

    #region Image Manipulations

    #region Selfie Photo Preview

    public void SetImagePreviewToSphere()
    {
        var surfacePoints = sphereVertexHandler.GetPointTransforms();
        var photo = photoPreview2D.GetComponent<TargetPhoto>();
        photo.SetToSphereVertex(surfacePoints[3]);
    }

    public void SetImagePreviewToGrid()
    {
        var chosenPhoto = photoGridManager.GetPhotoList.ElementAt(imgGridIndex);
        var gridPosition = chosenPhoto.GetComponent<TargetPhoto>().GridPosition;
        chosenPhoto.SetActive(false);

        var TargetPhoto = photoPreview2D.GetComponent<TargetPhoto>();
        TargetPhoto.SetGridPosition(gridPosition);
        TargetPhoto.ReturnToGrid();
    }

    #endregion

    public void SetPhotosToSphere(GameObject[] p_photoList)
    {
        var surfacePoints = sphereVertexHandler.GetPointTransforms();
        int index = 0;
        foreach (GameObject photoObj in p_photoList)
        {
            if (index > p_photoList.Length || index > surfacePoints.Length)
                break;
            var photo = photoObj.GetComponent<TargetPhoto>();
            photo.SetToSphereVertex(surfacePoints[index]);
            index++;
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

    public void SetPhotosToGrid(GameObject[] p_photoList)
    {
        foreach (GameObject p in p_photoList)
        {
            var TargetPhoto = p.GetComponent<TargetPhoto>();
            TargetPhoto.ReturnToGrid();
        }
    }
    #endregion

    public async Task StartFadeOverlay()
    {
        //var fadeIn = overlaySprite.GetComponent<FadeInSprite>();
        //fadeIn.FadeIn();   

        var fadeIn = overlaySprite.GetComponent<FadeInMask>();
        await fadeIn.FadeIn();
    }

    public void ResetOverlayOpacity()
    {
        var fadeIn = overlaySprite.GetComponent<FadeInMask>();
        fadeIn.Reset();
    }

    public void Reset()
    {
        photoPreview2D.GetComponent<TargetPhoto>().StopMoving();
        photoPreview2D.transform.position = photoPreviewDefaultPosition;
        photoPreview2D.transform.localScale = photoPreviewDefaultScale;
        ResetOverlayOpacity();
    }


}