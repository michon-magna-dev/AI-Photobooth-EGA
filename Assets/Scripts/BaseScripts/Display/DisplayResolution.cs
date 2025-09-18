using UnityEngine;

public enum DISPLAY_CAMERA
{
    WALL,
    FLOOR
}

public class DisplayResolution : MonoBehaviour
{
    [SerializeField] Camera m_camera;
    [Space]
    [SerializeField] DISPLAY_CAMERA m_CameraName = DISPLAY_CAMERA.WALL;
    [SerializeField] bool m_isScaled = false;
    [Space]
    [SerializeField] int m_targetDisplay = 0;
    [SerializeField] int m_paddingX = 0;
    [SerializeField] int m_paddingY = 0;
    [Space]
    [SerializeField] int m_screenHeight = 1920;
    [SerializeField] int m_screenWidth = 1080;
    [SerializeField] KeyCode m_resetKey = KeyCode.F1;

    private void OnValidate()
    {
        m_camera = GetComponent<Camera>();
    }

    void Start()
    {
        m_targetDisplay = ConfigManager.GetInstance().GetValue($"{m_CameraName}_SCREEN_TARGET_DISPLAY");
        m_isScaled = ConfigManager.GetInstance().GetBool($"{m_CameraName}_SCREEN_SCALED");

        m_paddingX = ConfigManager.GetInstance().GetValue($"{m_CameraName}_SCREEN_PADDINGX");
        m_paddingY = ConfigManager.GetInstance().GetValue($"{m_CameraName}_SCREEN_PADDINGY");

        m_screenHeight = ConfigManager.GetInstance().GetValue($"{m_CameraName}_SCREEN_HEIGHT");
        m_screenWidth = ConfigManager.GetInstance().GetValue($"{m_CameraName}_SCREEN_WIDTH");

        m_camera = GetComponent<Camera>();
        m_camera.targetDisplay = m_targetDisplay;

        //ResetScreen();
        Invoke("ResetScreen", 1f);
    }

    void Update()
    {
        if (Input.GetKeyDown(m_resetKey))
        {
            ResetScreen();
        } 
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    void ResetScreen()
    {
        if (!m_isScaled)
            return;

        //m_camera.pixelRect = new Rect(m_paddingX, Screen.height - m_screenHeight, m_screenWidth, m_screenHeight);
        m_camera.pixelRect = new Rect(m_paddingX, m_paddingY, m_screenWidth, m_screenHeight);
    }

}