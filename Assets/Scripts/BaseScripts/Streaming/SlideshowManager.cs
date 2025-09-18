using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SlideshowManager : MonoBehaviour
{
    private static SlideshowManager _instance;
    public static SlideshowManager GetInstance() => _instance;

    [SerializeField] TouchDetector m_touchManager;
    [SerializeField] RawImage m_imageCenter;
    [SerializeField] List<string> m_imagePathList;
    [Header("Folder")]
    [Tooltip("folder found in streaming assets")]
    [SerializeField] string photosFolderPath = "\\PhotosTaken";
    [SerializeField] string[] fileTypes = { ".jpg", ".png" };
    [Header("Slideshow")]
    [SerializeField] int playerIndex = 0;
    [SerializeField] float currentTime = 0;
    [SerializeField] float m_slideshowIntervalSec = 5.0f; // Adjust this value for your desired time interval.

    private Coroutine autoAdvanceCoroutine;

    #region LifeCycles

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

    void Start()
    {
        playerIndex = 0;
        photosFolderPath = Application.streamingAssetsPath + ConfigManager.GetInstance().GetStringValue("PHOTOTAKEN_FOLDER_PATH");
        //PopulateList();
        m_slideshowIntervalSec = ConfigManager.GetInstance().GetFloatValue("SLIDESHOW_INTERVAL_SEC");
        FindImageFiles(photosFolderPath);
        LoadPhoto();

        m_touchManager.OnPressLeft += OnPressLeft;
        m_touchManager.OnPressRight += OnPressRight;
        StartAutoAdvanceCoroutine();
    }

    private void OnDestroy()
    {
        m_touchManager.OnPressLeft -= OnPressLeft;
        m_touchManager.OnPressRight -= OnPressRight;
    }

    #endregion

    void StartAutoAdvanceCoroutine()
    {
        if (autoAdvanceCoroutine != null)
        {
            StopCoroutine(autoAdvanceCoroutine);
        }
        autoAdvanceCoroutine = StartCoroutine(AutoAdvanceCoroutine());
    }

    IEnumerator AutoAdvanceCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(m_slideshowIntervalSec);
            OnPressRight();
        }
    }

    public void ReLoadImages(bool p_startAtEnd)
    {
        gameObject.SetActive(true);
        var latestImagePath = ScreenShotEditable.GetInstance().GetImageTaken();
        FindImageFiles(photosFolderPath);
        LoadPhoto(latestImagePath);
        StartCoroutine(AutoAdvanceCoroutine());
    }

    void OnPressLeft()
    {
        playerIndex--;
        if (playerIndex < 0)
        {
            playerIndex = m_imagePathList.Count - 1;
        }
        LoadPhoto();
        StartAutoAdvanceCoroutine();
    }

    void OnPressRight()
    {
        playerIndex++;
        if (playerIndex >= m_imagePathList.Count)
        {
            playerIndex = 0;
        }

        LoadPhoto();
        StartAutoAdvanceCoroutine();
    }

    #region  Load Photos

    void LoadPhoto()
    {
        m_imageCenter.texture = ImageLoader.LoadTexture(m_imagePathList[playerIndex]);
        //m_imageCenter.GetComponent<FadeInImage>().StartFading();
    }
    void LoadPhoto(int p_index)
    {
        m_imageCenter.texture = ImageLoader.LoadTexture(m_imagePathList[p_index]);
        //m_imageCenter.GetComponent<FadeInImage>().StartFading();
    }
    void LoadPhoto(string p_path)
    {
        m_imageCenter.texture = ImageLoader.LoadTexture(p_path);
        //m_imageCenter.GetComponent<FadeInImage>().StartFading();
    }



    #endregion

    #region File Methods

    void PopulateList()
    {
        m_imagePathList = new List<string>();
        string[] imgFiles = Directory.GetFiles(photosFolderPath, "*.jpg");

        foreach (var filePath in imgFiles)
        {
            m_imagePathList.Add(filePath);
        }
    }

    public string[] FindImageFiles(string p_folderPath)
    {
        m_imagePathList = new List<string>();
        var di = new DirectoryInfo(p_folderPath);
        var list = new List<string>();
        foreach (var fileType in fileTypes)
        {
            foreach (var file in di.GetFiles($"*{fileType}", SearchOption.AllDirectories))
            {
                m_imagePathList.Add(file.ToString());
            }
        }

        return list.ToArray();
    }

    #endregion
}
