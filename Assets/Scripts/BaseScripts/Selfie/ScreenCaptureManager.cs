using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ScreenCaptureManager : MonoBehaviour
{
    //private static ScreenCaptureManager _instance;
    //public static ScreenCaptureManager GetInstance() => _instance;
    //[SerializeField] string screenShotFolderName;

    //[SerializeField] CountdownTimer countdownTimer;
    //[Space]
    //[SerializeField] string m_imageTakenPath;
    //[SerializeField] RawImage m_imagePreview;
    //[SerializeField] RawImage m_imagePreview2;
    //public string GetImageTakenPath()
    //{
    //    return m_imageTakenPath;
    //}
    //private void Awake()
    //{
    //    if (_instance != null)
    //    {
    //        Destroy(this.gameObject);
    //    }
    //    else
    //    {
    //        _instance = this;
    //    }
    //}
    //private void OnEnable()
    //{
    //    m_imagePreview.gameObject.SetActive(false);
    //    m_imagePreview2.gameObject.SetActive(false);
    //}

    //private void Start()
    //{
    //    screenShotFolderName = ConfigManager.GetInstance().GetStringValue("PHOTOTAKEN_FOLDER_PATH");

    //    countdownTimer.OnCountdownEnded += ScreenCapture;
    //    countdownTimer.OnCountdownEnded -= ScreenCapture;
    //}

    //void Update()
    //{
    //    DebugScreenShot();
    //}


    //private void ScreenCapture()
    //{
    //    string folderPath = Application.streamingAssetsPath + $"/{screenShotFolderName}/";
    //    if (!Directory.Exists(folderPath))
    //        Directory.CreateDirectory(folderPath);
    //    //todo: Change Later
    //    var userDetails = NetworkServiceManager.GetInstance().GetUserDetails();
    //    //var screenshotName = "Screenshot_" + System.DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") + ".png";
    //    var screenshotName = $"{userDetails[0]}_{userDetails[3]}" + ".png";
    //    UnityEngine.ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(folderPath, screenshotName), 1);
    //    Debug.Log(folderPath + screenshotName);
    //}

    //void DebugScreenShot()
    //{
    //    if (Input.GetKeyDown(KeyCode.E))
    //    {
    //        //ScreenCapture();

    //        OnClickScreenCaptureButton();
    //    }
    //}

    //public void OnClickScreenCaptureButton()
    //{
    //    StartCoroutine(CaptureScreen());
    //}

    //public IEnumerator CaptureScreen()
    //{
    //    // Wait till the last possible moment before screen rendering to hide the UI
    //    yield return null;

    //    // Wait for screen rendering to complete
    //    yield return new WaitForEndOfFrame();

    //    // Take screenshot
    //    string folderPath = Application.streamingAssetsPath + $"/{screenShotFolderName}/";
    //    if (!Directory.Exists(folderPath))
    //        Directory.CreateDirectory(folderPath);

    //    var userDetails = NetworkServiceManager.GetInstance().GetUserDetails();
    //    //var screenshotName = "Screenshot_" + System.DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") + ".png";
    //    var screenshotName = $"{userDetails[0]}_{userDetails[3]}" + ".png"; UnityEngine.ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(folderPath, screenshotName), 1);

    //    m_imageTakenPath = System.IO.Path.Combine(folderPath, screenshotName);
    //    Debug.Log(folderPath + screenshotName);
    //    yield return new WaitForSeconds(0.5f);
    //    Debug.LogAssertion("Reached Here");

    //    LoadImagePreview();
    //    yield return null;

    //    NetworkServiceManager.GetInstance().SendUDPMessage("Picture Taken");

    //}

    //void LoadImagePreview()
    //{
    //    Debug.LogAssertion("Final Image Loaded");
    //    var imgRaw = ImageLoader.LoadTexture(m_imageTakenPath);
    //    m_imagePreview.gameObject.SetActive(true);
    //    m_imagePreview2.gameObject.SetActive(true);

    //    m_imagePreview.texture = imgRaw;
    //    m_imagePreview2.texture = imgRaw;

    //}


}