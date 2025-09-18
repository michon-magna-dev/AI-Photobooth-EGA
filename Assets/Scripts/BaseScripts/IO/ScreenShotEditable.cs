using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class ScreenShotEditable : MonoBehaviour
{
    private static ScreenShotEditable _instance;
    public static ScreenShotEditable GetInstance() => _instance;

    [SerializeField] CountdownTimer countdownTimer;

    public Camera cameraWithUI;
    public Camera cameraWithoutUI;
    public int captureWidth; // Set your desired width here.
    public int captureHeight; // Set your desired height here.
    public int printResolutionWidth;
    public int printResolutionHeight;
    public int horizontalOffset;
    public int verticalOffset;
    public bool captureUI = true;

    //[SerializeField] RawImage m_imagePreview;
    //[SerializeField] RawImage m_imagePreview2;
    [SerializeField] RawImage[] m_imagePreviews;

    [SerializeField] SpriteRenderer m_image2DObj;

    public bool capturing = false;
    [SerializeField] string m_photoFileName;
    [SerializeField] string m_imageTakenPath;
    [SerializeField] string m_photoPublicPath;
    [SerializeField] string screenShotFolderName;
    public KeyCode debugKey = KeyCode.E;

    public bool useForwardSlash = false;
    public Action OnCapturedPhoto;


    public string GetImageTaken()
    {
        return m_imageTakenPath;
    }

    private void OnValidate()
    {
        //if (!Directory.Exists(screenShotFolderName))
        //{
        //    var newFolderDir = new DirectoryInfo(Application.persistentDataPath + "/" + "Uploads");
        //    m_imageTakenPath = newFolderDir.FullName;
        //}
    }

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        screenShotFolderName = ConfigManager.GetInstance().GetStringValue("PHOTOTAKEN_FOLDER_PATH");
        countdownTimer.OnCountdownEnded += CaptureScreen;
        countdownTimer.OnCountdownEnded -= CaptureScreen;

        horizontalOffset = (printResolutionWidth - captureWidth) / 2;
        verticalOffset = (printResolutionHeight - captureHeight) / 2;
    }

    void Update()
    {
        DebugScreenShot();
        if (Input.GetKeyDown(KeyCode.Slash))
        {
            SwitchSlashConfig();
        }
    }

    public void CaptureScreen()
    {
        if (capturing)
        {
            Debug.LogWarning("Capture is already in progress.");
            return;
        }

        //StartCoroutine(PerformCapture());
        StartCoroutine(PerformCaptureCustom());
    }

    private IEnumerator PerformCaptureCustom()
    {
        capturing = true;

        // Disable/enable the UI camera as needed.
        //cameraWithUI.enabled = captureUI;
        //cameraWithoutUI.enabled = !captureUI;

        yield return new WaitForEndOfFrame();

        // Create a texture to hold the capture at the original camera dimensions.
        RenderTexture renderTexture = new RenderTexture(captureWidth, captureHeight, 24);
        cameraWithUI.targetTexture = renderTexture;
        cameraWithUI.Render();

        // Read pixels from the render texture at the original dimensions.
        RenderTexture.active = renderTexture;
        Texture2D screenshot = new Texture2D(captureWidth, captureHeight, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0, 0, captureWidth, captureHeight), 0, 0);
        screenshot.Apply();

        // Create a new Texture2D to resize the screenshot to 1824x2736 without cropping.
        Texture2D resizedScreenshot = new Texture2D(printResolutionWidth, printResolutionHeight, TextureFormat.RGB24, false);
        resizedScreenshot.SetPixels(horizontalOffset, verticalOffset, captureWidth, captureHeight, screenshot.GetPixels()); // Adjust the parameters to position the 1080x1920 screenshot within the 1824x2736 texture.
        resizedScreenshot.Apply();

        //// Create a texture to hold the capture.
        //RenderTexture renderTexture = new RenderTexture(captureWidth, captureHeight, 24);
        //cameraWithUI.targetTexture = renderTexture;
        //cameraWithUI.Render();

        //// Read pixels from the render texture.
        //RenderTexture.active = renderTexture;
        //Texture2D screenshot = new Texture2D(captureWidth, captureHeight, TextureFormat.RGB24, false);
        //screenshot.ReadPixels(new Rect(0, 0, captureWidth, captureHeight), 0, 0);
        //screenshot.Apply();

        // Reset camera settings.
        cameraWithUI.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);

        // Save the captured image as a JPEG file.
        byte[] bytes = screenshot.EncodeToJPG();

        var userDetails = NetworkServiceManager.GetInstance().GetUserDetails();
        //var screenshotName = "Screenshot_" + System.DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") + ".png";

        string streamingAssetsFolderPath = Application.streamingAssetsPath + $"{screenShotFolderName}/";
        string publicFolderPath = m_photoPublicPath;

        var dateTime = System.DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss");
        var screenshotName = "Screenshot_" + dateTime + ".png";

        //var screenshotName = $"{userDetails[0]}_{userDetails[3]}" + ".png";
        //var screenshotName = $"Test" + ".png"; UnityEngine.ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(folderPath, screenshotName), 1);

        //saves one in streaming Assets
        m_photoFileName = "Screenshot_" + dateTime;
        SaveInPath(bytes, streamingAssetsFolderPath, screenshotName);
        //saves one in public for sharing
        SaveInPath(bytes, publicFolderPath, screenshotName);

        yield return new WaitForSeconds(0.5f);
        LoadImagePreview();

        yield return null;
        NetworkServiceManager.GetInstance().SendUDPMessage("Picture Taken");
        OnCapturedPhoto?.Invoke();

        capturing = false;
    }

    public void SaveInPath(byte[] p_imageByts, string p_folderPath, string p_screenshotName)
    {
        m_imageTakenPath = System.IO.Path.Combine(p_folderPath, p_screenshotName);
        //m_imageTakenPath = m_imageTakenPath.Replace("/", "\\");
        if (useForwardSlash)
        {
            m_imageTakenPath = m_imageTakenPath.Replace(@"\", "/");
        }
        else
        {
            m_imageTakenPath = m_imageTakenPath.Replace(@"/", @"\");
        }

        Debug.LogAssertion($"Saved Image Path: {m_imageTakenPath}");

        System.IO.File.WriteAllBytes(m_imageTakenPath, p_imageByts);
    }

    public void SwitchSlashConfig()
    {
        useForwardSlash = !useForwardSlash;
        Debug.LogAssertion($"Use Forward Slash : {useForwardSlash}");
    }

    void DebugScreenShot()
    {
        if (Input.GetKeyDown(debugKey))
        {
            //ScreenCapture();
            CaptureScreen();
        }
    }

    void LoadImagePreview()
    {
        Debug.LogAssertion("Final Image Loaded");
        var imgRaw = ImageLoader.LoadTexture(m_imageTakenPath);

        //m_imagePreview.gameObject.SetActive(true);
        foreach (var img in m_imagePreviews)
        {
            img.gameObject.SetActive(true);
            img.texture = imgRaw;
        }
        //m_imagePreview2.gameObject.SetActive(true);

        //m_imagePreview.texture = imgRaw;
        //m_imagePreview2.texture = imgRaw;


        m_image2DObj.sprite = ImageLoader.ConvertTextureToSprite((Texture2D)imgRaw);

    }

    public string GetLatestImageFileName()
    {
        return m_photoFileName;
    }
}
