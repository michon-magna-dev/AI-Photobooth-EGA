using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// Print Test Manager - Test printing functionality with last saved images
/// Now supports both local PrintHelper and Python server printing
/// </summary>
public class PrintTestManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Button btnRefreshImages;
    public Button btnPrintSelected;
    public Button btnPrintAll;
    public Button btnPrintViaPython;
    public Button btnPrintAllViaPython;
    public Button btnCreateTestImage;
    public Dropdown imageDropdown;
    public RawImage previewImage;
    public Text statusText;
    public Text imageInfoText;

    [Header("Print Settings")]
    public string printerName = "Canon SELPHY CP1500";
    public string printScaleMode = "fill"; // "fill" or "fit"
    public bool printLandscape = false;    // false = portrait (1200x1800)

    [Header("Python Server Settings")]
    public string pythonServerURL = "http://localhost:5000";
    public bool usePythonPrinting = true; // Default to Python printing

    [Header("Folder Paths")]
    public string processedPhotosPath = @"D:\Unity\ai_photobooth_multiple_user\Assets\StreamingAssets\ProcessedPhotos";
    public bool useRelativeToStreamingAssets = true; // If true, uses StreamingAssets/ProcessedPhotos instead

    private List<string> availableImages = new List<string>();
    private string selectedImagePath = "";

    void Start()
    {
        SetupUI();
        RefreshImageList();
    }

    void SetupUI()
    {
        if (btnRefreshImages != null)
            btnRefreshImages.onClick.AddListener(RefreshImageList);

        if (btnPrintSelected != null)
            btnPrintSelected.onClick.AddListener(PrintSelectedImage);

        if (btnPrintAll != null)
            btnPrintAll.onClick.AddListener(PrintAllImages);

        if (btnPrintViaPython != null)
            btnPrintViaPython.onClick.AddListener(PrintSelectedViaPython);

        if (btnPrintAllViaPython != null)
            btnPrintAllViaPython.onClick.AddListener(PrintAllViaPython);

        if (btnCreateTestImage != null)
            btnCreateTestImage.onClick.AddListener(CreateTestImage);

        if (imageDropdown != null)
            imageDropdown.onValueChanged.AddListener(OnImageSelected);

        UpdateStatus("Print Test Manager initialized");
    }

    public void RefreshImageList()
    {
        availableImages.Clear();

        string searchPath = useRelativeToStreamingAssets
            ? Path.Combine(Application.streamingAssetsPath, "ProcessedPhotos")
            : processedPhotosPath;

        UpdateStatus($"Searching for images in: {searchPath}");

        if (!Directory.Exists(searchPath))
        {
            UpdateStatus($"Directory not found: {searchPath}");
            return;
        }

        // Get all JPG files recursively
        try
        {
            string[] jpgFiles = Directory.GetFiles(searchPath, "*.jpg", SearchOption.AllDirectories);
            string[] jpegFiles = Directory.GetFiles(searchPath, "*.jpeg", SearchOption.AllDirectories);

            availableImages.AddRange(jpgFiles);
            availableImages.AddRange(jpegFiles);

            // Sort by last modified time (newest first)
            availableImages = availableImages
                .OrderByDescending(f => File.GetLastWriteTime(f))
                .ToList();

            UpdateImageDropdown();
            UpdateStatus($"Found {availableImages.Count} images");

            if (availableImages.Count > 0)
            {
                LoadPreviewImage(availableImages[0]);
            }
        }
        catch (Exception ex)
        {
            UpdateStatus($"Error scanning directory: {ex.Message}");
        }
    }

    void UpdateImageDropdown()
    {
        if (imageDropdown == null) return;

        imageDropdown.options.Clear();

        foreach (string imagePath in availableImages)
        {
            string fileName = Path.GetFileName(imagePath);
            string folderName = Path.GetFileName(Path.GetDirectoryName(imagePath));
            string displayName = $"{folderName}/{fileName}";

            imageDropdown.options.Add(new Dropdown.OptionData(displayName));
        }

        imageDropdown.value = 0;
        imageDropdown.RefreshShownValue();

        if (availableImages.Count > 0)
        {
            selectedImagePath = availableImages[0];
        }
    }

    public void OnImageSelected(int index)
    {
        if (index >= 0 && index < availableImages.Count)
        {
            selectedImagePath = availableImages[index];
            LoadPreviewImage(selectedImagePath);
        }
    }

    void LoadPreviewImage(string imagePath)
    {
        if (previewImage == null) return;

        try
        {
            if (File.Exists(imagePath))
            {
                byte[] imageData = File.ReadAllBytes(imagePath);
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(imageData);
                previewImage.texture = texture;

                // Update image info
                FileInfo fileInfo = new FileInfo(imagePath);
                string info = $"Size: {texture.width}x{texture.height}\n" +
                             $"File: {fileInfo.Length / 1024}KB\n" +
                             $"Modified: {fileInfo.LastWriteTime:yyyy-MM-dd HH:mm:ss}";

                if (imageInfoText != null)
                    imageInfoText.text = info;

                UpdateStatus($"Loaded: {Path.GetFileName(imagePath)}");
            }
            else
            {
                UpdateStatus($"File not found: {imagePath}");
            }
        }
        catch (Exception ex)
        {
            UpdateStatus($"Error loading image: {ex.Message}");
        }
    }

    // Local PrintHelper methods (original functionality)
    public void PrintSelectedImage()
    {
        if (string.IsNullOrEmpty(selectedImagePath))
        {
            UpdateStatus("No image selected");
            return;
        }

        PrintImageLocal(selectedImagePath);
    }

    public void PrintAllImages()
    {
        if (availableImages.Count == 0)
        {
            UpdateStatus("No images to print");
            return;
        }

        UpdateStatus($"Printing {availableImages.Count} images locally...");

        int successCount = 0;
        foreach (string imagePath in availableImages)
        {
            try
            {
                PrintImageLocal(imagePath, showStatus: false);
                successCount++;

                // Small delay between prints to avoid overwhelming the printer
                System.Threading.Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to print {Path.GetFileName(imagePath)}: {ex.Message}");
            }
        }

        UpdateStatus($"Local print batch completed: {successCount}/{availableImages.Count} successful");
    }

    void PrintImageLocal(string imagePath, bool showStatus = true)
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        try
        {
            if (showStatus)
                UpdateStatus($"Printing locally: {Path.GetFileName(imagePath)}");

            PrintHelper.PrintImage(imagePath, printerName, printScaleMode, printLandscape);

            if (showStatus)
                UpdateStatus($"Local print job sent: {Path.GetFileName(imagePath)}");
        }
        catch (Exception ex)
        {
            string errorMsg = $"Local print failed: {ex.Message}";
            UpdateStatus(errorMsg);
            Debug.LogError(errorMsg);
            throw;
        }
#else
        UpdateStatus("Local printing only supported on Windows builds");
#endif
    }

    // Python server printing methods (new functionality)
    public void PrintSelectedViaPython()
    {
        if (string.IsNullOrEmpty(selectedImagePath))
        {
            UpdateStatus("No image selected");
            return;
        }

        StartCoroutine(PrintImageViaPython(selectedImagePath));
    }

    public void PrintAllViaPython()
    {
        if (availableImages.Count == 0)
        {
            UpdateStatus("No images to print");
            return;
        }

        StartCoroutine(PrintAllImagesViaPython());
    }

    IEnumerator PrintImageViaPython(string imagePath, bool showStatus = true)
    {
        if (showStatus)
            UpdateStatus($"Printing via Python: {Path.GetFileName(imagePath)}");

        var payload = new PrintRequest
        {
            image_path = imagePath,
            printer_name = printerName,
            scale_mode = printScaleMode,
            landscape = printLandscape
        };

        string jsonData = JsonUtility.ToJson(payload);

        using (UnityWebRequest webRequest = new UnityWebRequest($"{pythonServerURL}/print", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    PrintResponse response = JsonUtility.FromJson<PrintResponse>(webRequest.downloadHandler.text);
                    if (response.success)
                    {
                        if (showStatus)
                            UpdateStatus($"Python print successful: {Path.GetFileName(imagePath)}");
                    }
                    else
                    {
                        UpdateStatus($"Python print failed: {response.message}");
                    }
                }
                catch (Exception ex)
                {
                    UpdateStatus($"Error parsing print response: {ex.Message}");
                }
            }
            else
            {
                UpdateStatus($"Python print request failed: {webRequest.error}");
            }
        }
    }

    IEnumerator PrintAllImagesViaPython()
    {
        UpdateStatus($"Printing {availableImages.Count} images via Python...");

        int successCount = 0;
        foreach (string imagePath in availableImages)
        {
            yield return StartCoroutine(PrintImageViaPython(imagePath, showStatus: false));
            successCount++; // Note: This doesn't check actual success, just completion

            // Small delay between prints
            yield return new WaitForSeconds(1f);
        }

        UpdateStatus($"Python print batch completed: {successCount}/{availableImages.Count} processed");
    }

    void UpdateStatus(string message)
    {
        if (statusText != null)
            statusText.text = $"[{DateTime.Now:HH:mm:ss}] {message}";

        Debug.Log($"PrintTestManager: {message}");
    }

    // Test methods for inspector buttons
    [ContextMenu("Test - Refresh Images")]
    public void TestRefreshImages()
    {
        RefreshImageList();
    }

    [ContextMenu("Test - Print Selected Local")]
    public void TestPrintSelectedLocal()
    {
        PrintSelectedImage();
    }

    [ContextMenu("Test - Print Selected Python")]
    public void TestPrintSelectedPython()
    {
        PrintSelectedViaPython();
    }

    [ContextMenu("Test - Create Test Image")]
    public void CreateTestImage()
    {
        CreateTest4x6Image();
    }

    void CreateTest4x6Image()
    {
        try
        {
            // Create a 1200x1800 test image (4x6 at 300 DPI, portrait)
            int width = printLandscape ? 1800 : 1200;
            int height = printLandscape ? 1200 : 1800;

            Texture2D testTexture = new Texture2D(width, height, TextureFormat.RGB24, false);

            // Fill with gradient and test pattern
            Color[] pixels = new Color[width * height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float r = (float)x / width;
                    float g = (float)y / height;
                    float b = 0.5f;

                    // Add border to test full-bleed
                    if (x < 50 || x >= width - 50 || y < 50 || y >= height - 50)
                    {
                        pixels[y * width + x] = Color.red; // Red border
                    }
                    else
                    {
                        pixels[y * width + x] = new Color(r, g, b, 1f);
                    }
                }
            }

            testTexture.SetPixels(pixels);
            testTexture.Apply();

            // Save test image
            string testDir = useRelativeToStreamingAssets
                ? Path.Combine(Application.streamingAssetsPath, "ProcessedPhotos", "TestImages")
                : Path.Combine(processedPhotosPath, "TestImages");

            Directory.CreateDirectory(testDir);

            string testPath = Path.Combine(testDir, $"test_4x6_{width}x{height}_{DateTime.Now:yyyyMMdd_HHmmss}.jpg");
            byte[] jpgData = testTexture.EncodeToJPG(92);
            File.WriteAllBytes(testPath, jpgData);

            UpdateStatus($"Test image created: {testPath}");

            // Clean up
            DestroyImmediate(testTexture);

            // Refresh the list to include the new test image
            RefreshImageList();
        }
        catch (Exception ex)
        {
            UpdateStatus($"Failed to create test image: {ex.Message}");
        }
    }

    // Public methods for external scripts
    public void SetPrinterName(string newPrinterName)
    {
        printerName = newPrinterName;
        UpdateStatus($"Printer set to: {printerName}");
    }

    public void SetPrintMode(string mode)
    {
        printScaleMode = mode;
        UpdateStatus($"Print mode set to: {printScaleMode}");
    }

    public void SetOrientation(bool landscape)
    {
        printLandscape = landscape;
        UpdateStatus($"Print orientation: {(landscape ? "Landscape" : "Portrait")}");
    }

    public void SetPythonServerURL(string url)
    {
        pythonServerURL = url;
        UpdateStatus($"Python server URL set to: {pythonServerURL}");
    }

    public List<string> GetAvailableImages()
    {
        return new List<string>(availableImages);
    }

    public string GetSelectedImagePath()
    {
        return selectedImagePath;
    }

    // Data classes for JSON serialization
    [Serializable]
    public class PrintRequest
    {
        public string image_path;
        public string printer_name;
        public string scale_mode;
        public bool landscape;
    }

    [Serializable]
    public class PrintResponse
    {
        public bool success;
        public string message;
    }
}