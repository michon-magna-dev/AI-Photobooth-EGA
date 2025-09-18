using UnityEngine;

public class ResolutionManager : MonoBehaviour
{
    public int portraitWidth = 1080;  // Set your desired portrait width
    public int portraitHeight = 1920;  // Set your desired portrait height

    void Start()
    {
        SetPortraitResolution();
    }

    void SetPortraitResolution()
    {
        // Set the desired resolution
        Screen.SetResolution(portraitWidth, portraitHeight, true);
    }
}
