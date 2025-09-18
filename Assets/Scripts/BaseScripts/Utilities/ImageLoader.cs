using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public static class ImageLoader
{
    public static Texture LoadTexture(string p_fileName)
    {
        byte[] pngBytes = File.ReadAllBytes(p_fileName);
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(pngBytes);
        var fileNameStartIndex = (p_fileName.LastIndexOf('0') < -1) ? p_fileName.LastIndexOf('0') : p_fileName.LastIndexOf('\\');
        var fileName = p_fileName.Substring(fileNameStartIndex);
        tex.name = fileName;
        return tex;
    }

    public static Texture2D LoadTextureResized(string p_fileName)
    {
        byte[] pngBytes = File.ReadAllBytes(p_fileName);
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(pngBytes);

        // Resize the texture to 1/4 of its original size
        int newWidth = tex.width / 2;
        int newHeight = tex.height / 2;
        Texture2D resizedTex = ResizeTexture(tex, newWidth, newHeight);

        // Set the name of the resized texture
        var fileNameStartIndex = (p_fileName.LastIndexOf('0') < -1) ? p_fileName.LastIndexOf('0') : p_fileName.LastIndexOf('\\');
        var fileName = p_fileName.Substring(fileNameStartIndex);
        resizedTex.name = fileName;

        return resizedTex;
    }
    
    public static Texture2D LoadTextureResized(string p_fileName,int p_reduceTo = 2)
    {
        byte[] pngBytes = File.ReadAllBytes(p_fileName);
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(pngBytes);

        // Resize the texture to 1/4 of its original size
        int newWidth = tex.width / p_reduceTo;
        int newHeight = tex.height / p_reduceTo;
        Texture2D resizedTex = ResizeTexture(tex, newWidth, newHeight);

        // Set the name of the resized texture
        var fileNameStartIndex = (p_fileName.LastIndexOf('0') < -1) ? p_fileName.LastIndexOf('0') : p_fileName.LastIndexOf('\\');
        var fileName = p_fileName.Substring(fileNameStartIndex);
        resizedTex.name = fileName;

        return resizedTex;
    }

    private static Texture2D ResizeTexture(Texture2D sourceTex, int newWidth, int newHeight)
    {
        // Create a new texture with the desired size
        Texture2D resizedTex = new Texture2D(newWidth, newHeight);

        // Resize using bilinear filtering for better quality
        Color[] pixels = resizedTex.GetPixels(0);
        for (int y = 0; y < newHeight; y++)
        {
            for (int x = 0; x < newWidth; x++)
            {
                float u = x / (float)newWidth;
                float v = y / (float)newHeight;

                pixels[y * newWidth + x] = sourceTex.GetPixelBilinear(u, v);
            }
        }

        resizedTex.SetPixels(pixels, 0);
        resizedTex.Apply();

        return resizedTex;
    }

    public static Texture LoadTexture(string p_fileName, int p_sizeX, int p_sizeY)
    {
        byte[] pngBytes = File.ReadAllBytes(p_fileName);
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(pngBytes);
        var fileNameStartIndex = (p_fileName.LastIndexOf('0') < -1) ? p_fileName.LastIndexOf('0') : p_fileName.LastIndexOf('\\');
        var fileName = p_fileName.Substring(fileNameStartIndex);
        tex.name = fileName;
        tex.Reinitialize(p_sizeX, p_sizeY);
        return tex;
    }

    public static string[] FindImageFiles(string p_folderPath)
    {
        string[] fileTypes = { ".jpg", ".png" };
        var imagePathList = new List<string>();
        var di = new DirectoryInfo(p_folderPath);
        var list = new List<string>();
        foreach (var fileType in fileTypes)
        {
            foreach (var file in di.GetFiles($"*{fileType}", SearchOption.AllDirectories))
            {
                imagePathList.Add(file.ToString());
            }
        }

        return list.ToArray();
    }
    public static Sprite LoadSprite(string p_fileName)
    {
        // Load the image bytes and create the Texture2D
        byte[] pngBytes = File.ReadAllBytes(p_fileName);
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(pngBytes);

        // Create the sprite from the texture
        Rect spriteRect = new Rect(0, 0, tex.width, tex.height);
        Sprite newSprite = Sprite.Create(tex, spriteRect, new Vector2(0.5f, 0.5f));

        // Set the sprite name
        var fileNameStartIndex = (p_fileName.LastIndexOf('0') < -1) ? p_fileName.LastIndexOf('0') : p_fileName.LastIndexOf('\\');
        var fileName = p_fileName.Substring(fileNameStartIndex);
        newSprite.name = fileName;

        return newSprite;
    }

    public static async Task<Sprite> LoadSpriteAsync(string p_fileName)
    {
        return await Task.Run(() =>
        {
            // Load the image bytes and create the Texture2D
            byte[] pngBytes = File.ReadAllBytes(p_fileName);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(pngBytes);

            // Create the sprite from the texture
            Rect spriteRect = new Rect(0, 0, tex.width, tex.height);
            Sprite newSprite = Sprite.Create(tex, spriteRect, new Vector2(0.5f, 0.5f));

            // Set the sprite name using the file name without extension
            string fileName = Path.GetFileNameWithoutExtension(p_fileName);
            newSprite.name = fileName;

            return newSprite;
        });
    }

    public static async Task<Sprite> LoadSpriteFromBytesAsync(string p_fileName)
    {
        byte[] pngBytes;

        // Load the file asynchronously on a background thread
        pngBytes = await Task.Run(() => File.ReadAllBytes(p_fileName));

        // Now create the Texture2D and Sprite on the main thread
        return CreateSpriteFromBytes(pngBytes, p_fileName);
    }

    private static Sprite CreateSpriteFromBytes(byte[] pngBytes, string p_fileName)
    {
        // Create the texture and load image on the main thread
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(pngBytes);

        // Create the sprite from the texture
        Rect spriteRect = new Rect(0, 0, tex.width, tex.height);
        Sprite newSprite = Sprite.Create(tex, spriteRect, new Vector2(0.5f, 0.5f));

        // Set the sprite name using the file name without extension
        string fileName = Path.GetFileNameWithoutExtension(p_fileName);
        newSprite.name = fileName;

        return newSprite;
    }

    public static Sprite ConvertTextureToSprite(Texture2D texture)
    {
        // Define the full texture as the Rect for the sprite
        Rect spriteRect = new Rect(0, 0, texture.width, texture.height);

        // Set the pivot point (0.5f, 0.5f) places the pivot in the center of the sprite
        Vector2 pivot = new Vector2(0.5f, 0.5f);

        // Create and return the sprite
        return Sprite.Create(texture, spriteRect, pivot);
    }

}