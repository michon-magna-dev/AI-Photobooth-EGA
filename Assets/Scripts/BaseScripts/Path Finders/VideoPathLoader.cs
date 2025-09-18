using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.IO;

public class VideoPathLoader : MonoBehaviour
{
    private static VideoPathLoader _instance;
    public static VideoPathLoader GetInstance() => _instance;

    [SerializeField] public string m_path = "Videos";
    [SerializeField] string[] m_filePaths;
    [SerializeField] string[] m_fileType = { ".mp4" };
    const string m_folderNameConfig = "VIDEO_FOLDER";
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        CheckFiles();
    }

    private void OnValidate()
    {
        FindFiles();
    }

    private void CheckFiles()
    {
        Debug.Log($"Video Paths FInding Started");
        m_path = ConfigManager.GetInstance().GetStringValue(m_folderNameConfig);
        DontDestroyOnLoad(this);
        FindFiles();
    }

    public string[] GetPaths() => m_filePaths;

    public string[] FindImageFiles(string p_folderPath)
    {
        Debug.Log($"Path to Find Images: " + p_folderPath);
        Debug.Log(Directory.Exists(Application.streamingAssetsPath));
        Debug.Assert(Directory.Exists(p_folderPath));

        var di = new DirectoryInfo(p_folderPath);
        var list = new List<string>();
        foreach (var fileType in m_fileType)
        {
            Debug.Log($"Finding {fileType}");
            foreach (var file in di.GetFiles($"*{fileType}", SearchOption.AllDirectories))
            {
                list.Add(file.ToString());
                Debug.Log($"{file}");
            }
        }

        return list.ToArray();
    }

    [ContextMenu("Find Files")]
    public void FindFiles()
    {
        m_filePaths = FindImageFiles($"{Application.streamingAssetsPath}/{m_path}/");
    }

}