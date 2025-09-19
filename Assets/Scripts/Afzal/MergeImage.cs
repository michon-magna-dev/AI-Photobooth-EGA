using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class MergeImage : MonoBehaviour
{
    [Header("Merge Settings")]
    public Texture2D overlayTexture;
    public int overlayX = 0;
    public int overlayY = 0;
    public int targetDPI = 300;
    public string outputFolder = "FinalPrintOut";
    public int jpegQuality = 100;

    //Dev note: If changing overlayTexture, make sure to enable Read/Write
    public int targetWidth = 1200; // final pixel width
    public int targetHeight = 1800; // final pixel height

    [Header("Debug")]
    public RawImage previewImage; // Optional: for previewing merged result

    private int finalPrintCount = 0;

    private void Start()
    {
        finalPrintCount = PlayerPrefs.GetInt("finalPrintCount", 0);

        // Create output directory if it doesn't exist
        string outputPath = Path.Combine(Application.streamingAssetsPath, outputFolder);
        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }
    }

    /// <summary>
    /// Merges a background image with overlay and saves it with proper DPI
    /// </summary>
    /// <param name="backgroundImagePath">Path to the background image</param>
    /// <param name="imageIndex">Index for unique naming</param>
    /// <returns>Path to the merged image file</returns>
    public string MergeAndSaveImage(string backgroundImagePath, int imageIndex = 0)
    {
        if (!File.Exists(backgroundImagePath))
        {
            Debug.LogError($"Background image not found: {backgroundImagePath}");
            return null;
        }

        // Load background texture
        Texture2D backgroundTexture = LoadTextureFromFile(backgroundImagePath);
        if (backgroundTexture == null)
        {
            Debug.LogError($"Failed to load background texture from: {backgroundImagePath}");
            return null;
        }

        // Create merged texture
        Texture2D mergedTexture = null;
        if (overlayTexture != null)
        {
            mergedTexture = AddOverlayToTexture(backgroundTexture, overlayTexture, overlayX, overlayY);
        }
        else
        {
            // If no overlay, just resize for proper DPI
            //mergedTexture = ResizeTextureForDPI(backgroundTexture, targetDPI);
            mergedTexture = ResizeToFixed(backgroundTexture, targetWidth, targetHeight);
        }

        // Save the merged image
        string savedPath = SaveMergedImage(mergedTexture, imageIndex);

        // Update preview if available
        if (previewImage != null)
        {
            previewImage.texture = mergedTexture;
        }

        // Clean up
        if (backgroundTexture != null)
            DestroyImmediate(backgroundTexture);

        // Don't destroy mergedTexture if it's being used for preview
        if (previewImage == null && mergedTexture != null)
            DestroyImmediate(mergedTexture);

        return savedPath;
    }

    /// <summary>
    /// Merges multiple images in batch
    /// </summary>
    /// <param name="imagePaths">List of image paths to merge</param>
    /// <returns>List of merged image paths</returns>
    public List<string> MergeMultipleImages(List<string> imagePaths)
    {
        List<string> mergedPaths = new List<string>();

        for (int i = 0; i < imagePaths.Count; i++)
        {
            string mergedPath = MergeAndSaveImage(imagePaths[i], i);
            if (!string.IsNullOrEmpty(mergedPath))
            {
                mergedPaths.Add(mergedPath);
            }
        }

        return mergedPaths;
    }

    /// <summary>
    /// Loads texture from file path
    /// </summary>
    private Texture2D LoadTextureFromFile(string filePath)
    {
        try
        {
            byte[] rawData = File.ReadAllBytes(filePath);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(rawData);
            return texture;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error loading texture: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Adds overlay to background texture with proper alpha blending
    /// </summary>
    private Texture2D AddOverlayToTexture(Texture2D background, Texture2D overlay, int startX, int startY)
    {
        // Ensure proper DPI resolution
        //Texture2D resizedBackground = ResizeTextureForDPI(background, targetDPI);
        Texture2D resizedBackground = ResizeToFixed(background, targetWidth, targetHeight);

        //Texture2D newTexture = new Texture2D(resizedBackground.width, resizedBackground.height, TextureFormat.RGB24, false);
        Texture2D newTexture = new Texture2D(targetWidth, targetHeight, TextureFormat.RGBA32, false);

        // Copy background pixels
        Color[] backgroundPixels = resizedBackground.GetPixels();
        newTexture.SetPixels(backgroundPixels);

        // Apply overlay with bounds checking
        if (overlay != null)
        {
            for (int x = 0; x < overlay.width && (startX + x) < newTexture.width; x++)
            {
                for (int y = 0; y < overlay.height && (startY + y) < newTexture.height; y++)
                {
                    // Skip if overlay position is outside bounds
                    if (startX + x < 0 || startY + y < 0) continue;

                    Color overlayColor = overlay.GetPixel(x, y);
                    if (overlayColor.a > 0) // Only apply non-transparent pixels
                    {
                        Color backgroundColor = newTexture.GetPixel(startX + x, startY + y);
                        Color finalColor = Color.Lerp(backgroundColor, overlayColor, overlayColor.a);
                        newTexture.SetPixel(startX + x, startY + y, finalColor);
                    }
                }
            }
        }

        newTexture.Apply();

        // Clean up resized background if it's different from original
        if (resizedBackground != background)
            DestroyImmediate(resizedBackground);

        return newTexture;
    }

    /// <summary>
    /// Resizes texture to achieve target DPI
    /// </summary>
    /// 
    //private Texture2D ResizeTextureForDPI(Texture2D originalTexture, int targetDPI)
    //{
    //    // Calculate target dimensions for specified DPI
    //    // Assuming 72 DPI as baseline (standard screen DPI)
    //    float scaleFactor = (float)targetDPI / 72f;
    //    int targetWidth = Mathf.RoundToInt(originalTexture.width * scaleFactor);
    //    int targetHeight = Mathf.RoundToInt(originalTexture.height * scaleFactor);

    //    // If the texture is already the right size, return original
    //    if (targetWidth == originalTexture.width && targetHeight == originalTexture.height)
    //    {
    //        return originalTexture;
    //    }

    //    // Create new texture with target dimensions
    //    Texture2D resizedTexture = new Texture2D(targetWidth, targetHeight, TextureFormat.RGB24, false);

    //    // Bilinear resize for better quality
    //    for (int x = 0; x < targetWidth; x++)
    //    {
    //        for (int y = 0; y < targetHeight; y++)
    //        {
    //            float u = (float)x / targetWidth;
    //            float v = (float)y / targetHeight;
    //            Color pixel = originalTexture.GetPixelBilinear(u, v);
    //            resizedTexture.SetPixel(x, y, pixel);
    //        }
    //    }

    //    resizedTexture.Apply();
    //    return resizedTexture;
    //}


    private Texture2D ResizeToFixed(Texture2D original, int width, int height)
    {
        Texture2D resized = new Texture2D(width, height, TextureFormat.RGBA32, false);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float u = (float)x / (width - 1);
                float v = (float)y / (height - 1);
                Color c = original.GetPixelBilinear(u, v);
                resized.SetPixel(x, y, c);
            }
        }
        resized.Apply();
        return resized;
    }

    /// <summary>
    /// Saves merged image to disk
    /// </summary>
    //private string SaveMergedImage(Texture2D texture, int imageIndex)
    //{
    //    try
    //    {
    //        byte[] bytes = texture.EncodeToJPG(jpegQuality);

    //        finalPrintCount++;
    //        PlayerPrefs.SetInt("finalPrintCount", finalPrintCount);

    //        string filename = $"PhotoBooth_Merged_{finalPrintCount}_{imageIndex}.jpg";
    //        string outputPath = Path.Combine(Application.streamingAssetsPath, outputFolder);
    //        string filePath = Path.Combine(outputPath, filename);

    //        File.WriteAllBytes(filePath, bytes);

    //        Debug.Log($"Merged image saved: {filePath}");
    //        return filePath;
    //    }
    //    catch (Exception ex)
    //    {
    //        Debug.LogError($"Error saving merged image: {ex.Message}");
    //        return null;
    //    }
    //}
    private bool savePngWithDpi = true; // set true to write PNG with pHYs metadata
    private string SaveMergedImage(Texture2D texture, int imageIndex)
    {
        try
        {
            finalPrintCount++;
            PlayerPrefs.SetInt("finalPrintCount", finalPrintCount);

            string baseName = $"PhotoBooth_Merged_{finalPrintCount}_{imageIndex}";
            string outputPath = Path.Combine(Application.streamingAssetsPath, outputFolder);
            if (!Directory.Exists(outputPath)) Directory.CreateDirectory(outputPath);

            // First save JPG temporarily  
            byte[] jpgBytes = texture.EncodeToJPG(jpegQuality);
            string jpgFilename = baseName + ".jpg";
            string jpgPath = Path.Combine(outputPath, jpgFilename);
            File.WriteAllBytes(jpgPath, jpgBytes);

            // Encode PNG  
            byte[] pngBytes = texture.EncodeToPNG();

            // Insert pHYs chunk for DPI metadata  
            int dpi = 300;
            byte[] pngWithPhys = InsertPngPhysChunk(pngBytes, dpi);

            string pngFilename = baseName + ".png";
            string pngPath = Path.Combine(outputPath, pngFilename);
            File.WriteAllBytes(pngPath, pngWithPhys);

            // Delete the JPG file after PNG is created successfully  
            if (File.Exists(jpgPath))
            {
                File.Delete(jpgPath);
                Debug.Log($"Deleted temporary JPG: {jpgPath}");
            }

            Debug.Log($"Merged image saved (PNG with 300 DPI): {pngPath}");
            return pngPath; // Always return PNG path for printing  
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error saving merged image: {ex.Message}");
            return null;
        }
    }
    /// <summary>  
    /// Inserts a pHYs chunk (pixels per unit) into a PNG byte array before the first IDAT chunk.  
    /// </summary>  
    private byte[] InsertPngPhysChunk(byte[] pngBytes, int dpi)
    {
        if (pngBytes == null || pngBytes.Length < 8) return pngBytes;

        const int PNG_SIG_LENGTH = 8;

        // Compute pixels per meter  
        double ppmDouble = dpi / 0.0254; // dpi * 39.37007874015748  
        uint ppm = (uint)Math.Round(ppmDouble);

        // pHYs data: 4 bytes X (big-endian), 4 bytes Y, 1 byte unit (1 = meter)  
        byte[] physData = new byte[9];
        WriteUIntToBytesBigEndian(ppm, physData, 0);
        WriteUIntToBytesBigEndian(ppm, physData, 4);
        physData[8] = 1; // unit specifier: 1 = meter  

        // Build pHYs chunk: length(4) type(4) data(9) crc(4)  
        byte[] physType = System.Text.Encoding.ASCII.GetBytes("pHYs");
        uint physCrc = Crc32(ConcatArrays(physType, physData));
        byte[] physChunk = new byte[4 + 4 + physData.Length + 4];
        WriteUIntToBytesBigEndian((uint)physData.Length, physChunk, 0);
        Array.Copy(physType, 0, physChunk, 4, 4);
        Array.Copy(physData, 0, physChunk, 8, physData.Length);
        WriteUIntToBytesBigEndian(physCrc, physChunk, 8 + physData.Length);

        // Find insertion point: before the first IDAT chunk  
        int idx = PNG_SIG_LENGTH;
        while (idx + 8 < pngBytes.Length)
        {
            uint chunkLen = ReadUIntFromBytesBigEndian(pngBytes, idx);
            string chunkType = System.Text.Encoding.ASCII.GetString(pngBytes, idx + 4, 4);
            if (chunkType == "IDAT")
            {
                // Insert physChunk here  
                byte[] result = new byte[pngBytes.Length + physChunk.Length];
                Array.Copy(pngBytes, 0, result, 0, idx);
                Array.Copy(physChunk, 0, result, idx, physChunk.Length);
                Array.Copy(pngBytes, idx, result, idx + physChunk.Length, pngBytes.Length - idx);
                return result;
            }
            long advance = 4 + 4 + chunkLen + 4;
            idx += (int)advance;
        }

        // If IDAT not found, append physChunk before EOF  
        byte[] fallbackResult = new byte[pngBytes.Length + physChunk.Length];
        Array.Copy(pngBytes, 0, fallbackResult, 0, pngBytes.Length);
        Array.Copy(physChunk, 0, fallbackResult, pngBytes.Length, physChunk.Length);
        return fallbackResult;
    }
    private static void WriteUIntToBytesBigEndian(uint value, byte[] buffer, int offset)
    {
        buffer[offset + 0] = (byte)((value >> 24) & 0xFF);
        buffer[offset + 1] = (byte)((value >> 16) & 0xFF);
        buffer[offset + 2] = (byte)((value >> 8) & 0xFF);
        buffer[offset + 3] = (byte)(value & 0xFF);
    }
    private static uint ReadUIntFromBytesBigEndian(byte[] buffer, int offset)
    {
        return ((uint)buffer[offset] << 24) | ((uint)buffer[offset + 1] << 16) |
               ((uint)buffer[offset + 2] << 8) | ((uint)buffer[offset + 3]);
    }
    private static byte[] ConcatArrays(byte[] a, byte[] b)
    {
        byte[] r = new byte[a.Length + b.Length];
        Array.Copy(a, 0, r, 0, a.Length);
        Array.Copy(b, 0, r, a.Length, b.Length);
        return r;
    }
    /// <summary>  
    /// CRC32 calculation for PNG chunk CRC  
    /// </summary>  
    private static uint Crc32(byte[] bytes)
    {
        uint crc = 0xFFFFFFFF;
        uint[] table = crcTable;
        for (int i = 0; i < bytes.Length; i++)
        {
            byte index = (byte)((crc ^ bytes[i]) & 0xFF);
            crc = (crc >> 8) ^ table[index];
        }
        return crc ^ 0xFFFFFFFF;
    }
    // Precomputed CRC table  
    private static readonly uint[] crcTable = MakeCrcTable();

    private static uint[] MakeCrcTable()
    {
        uint[] table = new uint[256];
        const uint poly = 0xEDB88320;
        for (uint i = 0; i < 256; i++)
        {
            uint c = i;
            for (int k = 0; k < 8; k++)
            {
                if ((c & 1) != 0) c = poly ^ (c >> 1);
                else c >>= 1;
            }
            table[i] = c;
        }
        return table;
    }


    //////////////////////////////////////////
    /// <summary>
    /// Legacy method for compatibility with old MergeImage script
    /// </summary>
    public void MergeImageAndSaveTo(string filename)
    {
        MergeAndSaveImage(filename, 0);
    }

    /// <summary>
    /// Legacy method - loads texture from folder
    /// </summary>
    public Texture2D TextureFromFolder(string filename)
    {
        return LoadTextureFromFile(filename);
    }

    /// <summary>
    /// Legacy method - adds watermark
    /// </summary>
    public Texture2D AddWatermark(Texture2D background, Texture2D watermark, int startX, int startY)
    {
        return AddOverlayToTexture(background, watermark, startX, startY);
    }

    /// <summary>
    /// Legacy method - saves image
    /// </summary>
    public void SaveImage()
    {
        // This method is kept for compatibility but doesn't do anything
        // as the new workflow handles saving automatically
        Debug.LogWarning("SaveImage() is deprecated. Use MergeAndSaveImage() instead.");
    }

    /// <summary>
    /// Updates overlay settings at runtime
    /// </summary>
    public void UpdateOverlaySettings(Texture2D newOverlay, int x, int y, int dpi = 300)
    {
        overlayTexture = newOverlay;
        overlayX = x;
        overlayY = y;
        targetDPI = dpi;
    }

    /// <summary>
    /// Gets the current print count
    /// </summary>
    public int GetPrintCount()
    {
        return finalPrintCount;
    }

    /// <summary>
    /// Resets the print count
    /// </summary>
    public void ResetPrintCount()
    {
        finalPrintCount = 0;
        PlayerPrefs.SetInt("finalPrintCount", 0);
    }
}