
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using ZXing.Common;

public class QrGenerationHandler : MonoBehaviour
{
    [Header("QR Code Settings")]
    public string qrText = "Hello from Unity!";
    public int qrSize = 256;

    [Header("UI Reference")]
    public RawImage targetRawImage; 

    void Start()
    {
        GenerateAndDisplayQR(qrText);
    }

    private void Update()
    {
        // For testing: regenerate QR code when 'G' is pressed
        if (Input.GetKeyDown(KeyCode.F6))
        {
            GenerateAndDisplayQR(qrText);
        }
    }

    [ContextMenu("Generate QR")]
    public void TestQRGeneration()
    {
        GenerateAndDisplayQR(qrText);
    }

    public void GenerateAndDisplayQR(string text)
    {
        Texture2D qrTexture = GenerateQRCodeTexture(text, qrSize);
        if (targetRawImage != null)
        {
            targetRawImage.texture = qrTexture;
            //targetRawImage.SetNativeSize(); // Optional: makes RawImage match texture size
        }
    }

    public Texture2D GenerateQRCodeTexture(string textForEncoding, int size)
    {
        // Setup the QR code writer
        var writer = new BarcodeWriterPixelData
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new EncodingOptions
            {
                Height = size,
                Width = size,
                Margin = 0, // no white border
                PureBarcode = true
            }
        };

        var pixelData = writer.Write(textForEncoding);

        // Create texture and apply transparency to white pixels
        Texture2D texture = new Texture2D(pixelData.Width, pixelData.Height, TextureFormat.RGBA32, false);
        Color32[] pixels = new Color32[pixelData.Pixels.Length / 4];

        for (int i = 0; i < pixels.Length; i++)
        {
            byte r = pixelData.Pixels[i * 4];
            bool isBlack = r < 128;
            pixels[i] = isBlack ? Color.black : new Color(0, 0, 0, 0); // transparent for white
        }

        texture.SetPixels32(pixels);
        texture.Apply();

        return texture;
    }
}
