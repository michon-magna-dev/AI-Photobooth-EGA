using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Photo Manager for AI Photobooth - Handles camera operations and photo capture
/// Works independently and communicates with GameManager via events
/// </summary>
public class PhotoManager : MonoBehaviour
{
    [Header("Camera Settings")]
    public int cameraWidth = 1920;
    public int cameraHeight = 1080;
    public int targetFPS = 30;

    [Header("Photo Quality Settings")]
    [Range(1, 100)]
    public int jpegQuality = 85;
    public bool flipHorizontal = true; // Mirror the camera like a selfie

    // Events
    public System.Action<string, Texture2D> OnPhotoTaken;
    public System.Action<string> OnCameraError;

    // Private variables
    private WebCamTexture webCamTexture;
    private RawImage cameraFeedDisplay;
    private string sessionID;
    private string capturedPhotosPath;
    private bool isInitialized = false;
    private bool isTakingPhoto = false;

    // Camera device info
    private WebCamDevice[] availableDevices;
    private int currentDeviceIndex = 0;

    /// <summary>
    /// Initialize the PhotoManager with session ID
    /// </summary>
    public void Initialize(string sessionId)
    {
        sessionID = sessionId;
        SetupDirectories();
        InitializeCamera();
        isInitialized = true;
    }

    /// <summary>
    /// Setup directory structure for photos
    /// </summary>
    void SetupDirectories()
    {
        capturedPhotosPath = Path.Combine(Application.streamingAssetsPath, "CapturedPhotos", sessionID);
        Directory.CreateDirectory(capturedPhotosPath);
        Debug.Log($"Photos will be saved to: {capturedPhotosPath}");
    }

    /// <summary>
    /// Initialize webcam for photo capture
    /// </summary>
    void InitializeCamera()
    {
        availableDevices = WebCamTexture.devices;

        if (availableDevices.Length == 0)
        {
            Debug.LogError("No camera devices found!");
            OnCameraError?.Invoke("No camera devices found!");
            return;
        }

        // Log available cameras
        Debug.Log($"Found {availableDevices.Length} camera device(s):");
        for (int i = 0; i < availableDevices.Length; i++)
        {
            Debug.Log($"  {i}: {availableDevices[i].name} (Front: {availableDevices[i].isFrontFacing})");
        }

        // Try to find front-facing camera first, otherwise use the first available
        currentDeviceIndex = 0;
        for (int i = 0; i < availableDevices.Length; i++)
        {
            if (availableDevices[i].isFrontFacing)
            {
                currentDeviceIndex = i;
                break;
            }
        }

        StartCamera();
    }

    /// <summary>
    /// Start the camera with current device
    /// </summary>
    void StartCamera()
    {
        if (availableDevices.Length == 0) return;

        // Stop existing camera if running
        StopCamera();

        WebCamDevice selectedDevice = availableDevices[currentDeviceIndex];
        Debug.Log($"Starting camera: {selectedDevice.name}");

        webCamTexture = new WebCamTexture(selectedDevice.name, cameraWidth, cameraHeight, targetFPS);

        // Set the camera feed to the display if available
        if (cameraFeedDisplay != null)
        {
            cameraFeedDisplay.texture = webCamTexture;

            // Handle horizontal flipping for front-facing cameras
            //if (flipHorizontal && selectedDevice.isFrontFacing)
            //{
            //    cameraFeedDisplay.transform.localScale = new Vector3(-1, 1, 1);
            //}
            //else
            //{
            //    cameraFeedDisplay.transform.localScale = new Vector3(-1, 1, 1);
            //}
        }

        webCamTexture.Play();

        // Wait for camera to start and check if it's working
        StartCoroutine(CheckCameraStatus());
    }

    /// <summary>
    /// Check if camera started successfully
    /// </summary>
    IEnumerator CheckCameraStatus()
    {
        float timeout = 5f;
        float elapsed = 0f;

        while (elapsed < timeout)
        {
            if (webCamTexture != null && webCamTexture.isPlaying && webCamTexture.width > 16)
            {
                Debug.Log($"Camera started successfully: {webCamTexture.width}x{webCamTexture.height} @ {webCamTexture.requestedFPS}fps");
                yield break;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Camera failed to start
        Debug.LogError("Camera failed to start within timeout period");
        OnCameraError?.Invoke("Camera failed to start");
    }

    /// <summary>
    /// Stop the current camera
    /// </summary>
    void StopCamera()
    {
        if (webCamTexture != null)
        {
            webCamTexture.Stop();
            Destroy(webCamTexture);
            webCamTexture = null;
        }
    }

    /// <summary>
    /// Set the RawImage component to display camera feed
    /// </summary>
    public void SetCameraFeed(RawImage cameraDisplay)
    {
        cameraFeedDisplay = cameraDisplay;

        if (webCamTexture != null && cameraFeedDisplay != null)
        {
            cameraFeedDisplay.texture = webCamTexture;
        }
    }

    /// <summary>
    /// Switch to next available camera device
    /// </summary>
    public void SwitchCamera()
    {
        if (availableDevices.Length <= 1) return;

        currentDeviceIndex = (currentDeviceIndex + 1) % availableDevices.Length;
        StartCamera();
    }

    /// <summary>
    /// Take a photo and save it
    /// </summary>
    public void TakePhoto(string userName)
    {
        if (!isInitialized)
        {
            Debug.LogError("PhotoManager not initialized!");
            return;
        }

        if (isTakingPhoto)
        {
            Debug.LogWarning("Photo capture already in progress!");
            return;
        }

        if (webCamTexture == null || !webCamTexture.isPlaying)
        {
            Debug.LogError("Camera not available!");
            OnCameraError?.Invoke("Camera not available!");
            return;
        }

        StartCoroutine(CapturePhotoCoroutine(userName));
    }

    /// <summary>
    /// Coroutine to capture and save photo
    /// </summary>
    IEnumerator CapturePhotoCoroutine(string userName)
    {
        isTakingPhoto = true;

        // Wait for end of frame to ensure camera texture is ready
        yield return new WaitForEndOfFrame();

        try
        {
            // Create texture from camera
            Texture2D photo = new Texture2D(webCamTexture.width, webCamTexture.height, TextureFormat.RGB24, false);
            photo.SetPixels(webCamTexture.GetPixels());

            // Handle horizontal flipping if needed
            if (flipHorizontal && availableDevices[currentDeviceIndex].isFrontFacing)
            {
                FlipTextureHorizontally(photo);
            }

            photo.Apply();

            // Generate filename
            string timestamp = System.DateTime.Now.ToString("HHmmss");
            string fileName = $"{userName}_{timestamp}_{sessionID}.jpg";
            string filePath = Path.Combine(capturedPhotosPath, fileName);

            // Save photo with specified quality
            byte[] photoBytes = photo.EncodeToJPG(jpegQuality);
            File.WriteAllBytes(filePath, photoBytes);

            Debug.Log($"Photo saved: {filePath} ({photoBytes.Length} bytes)");

            // Create a copy for display (don't destroy the original yet)
            Texture2D displayTexture = new Texture2D(photo.width, photo.height, TextureFormat.RGB24, false);
            displayTexture.SetPixels(photo.GetPixels());
            displayTexture.Apply();

            // Notify listeners
            OnPhotoTaken?.Invoke(filePath, displayTexture);

            // Clean up original texture
            Destroy(photo);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error capturing photo: {e.Message}");
            OnCameraError?.Invoke($"Error capturing photo: {e.Message}");
        }
        finally
        {
            isTakingPhoto = false;
        }
    }

    /// <summary>
    /// Flip texture horizontally (for front-facing camera mirror effect)
    /// </summary>
    void FlipTextureHorizontally(Texture2D texture)
    {
        int width = texture.width;
        int height = texture.height;
        Color[] pixels = texture.GetPixels();
        Color[] flippedPixels = new Color[pixels.Length];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                flippedPixels[y * width + x] = pixels[y * width + (width - 1 - x)];
            }
        }

        texture.SetPixels(flippedPixels);
    }

    /// <summary>
    /// Get camera status information
    /// </summary>
    public string GetCameraStatus()
    {
        if (webCamTexture == null)
            return "Camera not initialized";

        if (!webCamTexture.isPlaying)
            return "Camera not playing";

        return $"Camera active: {webCamTexture.width}x{webCamTexture.height} @ {webCamTexture.requestedFPS}fps";
    }

    /// <summary>
    /// Get list of available camera devices
    /// </summary>
    public string[] GetAvailableCameras()
    {
        if (availableDevices == null) return new string[0];

        string[] cameraNames = new string[availableDevices.Length];
        for (int i = 0; i < availableDevices.Length; i++)
        {
            cameraNames[i] = availableDevices[i].name;
        }
        return cameraNames;
    }

    /// <summary>
    /// Check if camera is currently available and working
    /// </summary>
    public bool IsCameraReady()
    {
        return webCamTexture != null && webCamTexture.isPlaying && !isTakingPhoto;
    }

    /// <summary>
    /// Get the current camera texture (for external use)
    /// </summary>
    public WebCamTexture GetCameraTexture()
    {
        return webCamTexture;
    }

    /// <summary>
    /// Pause camera (useful for performance when not needed)
    /// </summary>
    public void PauseCamera()
    {
        if (webCamTexture != null && webCamTexture.isPlaying)
        {
            webCamTexture.Pause();
        }
    }

    /// <summary>
    /// Resume camera
    /// </summary>
    public void ResumeCamera()
    {
        if (webCamTexture != null && !webCamTexture.isPlaying)
        {
            webCamTexture.Play();
        }
    }

    /// <summary>
    /// Clean up resources
    /// </summary>
    void OnDestroy()
    {
        StopCamera();
    }

    /// <summary>
    /// Handle application pause (mobile)
    /// </summary>
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            PauseCamera();
        }
        else
        {
            ResumeCamera();
        }
    }

    /// <summary>
    /// Handle application focus (desktop)
    /// </summary>
    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            PauseCamera();
        }
        else
        {
            ResumeCamera();
        }
    }

    // Debug methods for testing
#if UNITY_EDITOR
    [ContextMenu("Test Take Photo")]
    void TestTakePhoto()
    {
        TakePhoto("TestUser");
    }

    [ContextMenu("Switch Camera")]
    void TestSwitchCamera()
    {
        SwitchCamera();
    }

    [ContextMenu("Print Camera Status")]
    void TestPrintStatus()
    {
        Debug.Log(GetCameraStatus());
        string[] cameras = GetAvailableCameras();
        Debug.Log($"Available cameras: {string.Join(", ", cameras)}");
    }
#endif
}