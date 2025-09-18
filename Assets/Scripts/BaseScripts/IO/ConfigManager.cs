using UnityEngine;
using System.IO;

public class ConfigManager : MonoBehaviour
{
    private static ConfigManager _instance;

    [Header("File")]
    [SerializeField] string m_configPath = "\\config.txt";
    [SerializeField] string m_excelPath = "\\content.csv";
   
    public static ConfigManager GetInstance()
    {
        return _instance;
    }

    void Awake()
    {
        if (_instance == null) { _instance = this; } else { Destroy(gameObject); }
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        if (!Directory.Exists(m_configPath))
        {
            Debug.LogError($"Warning: {m_configPath} does not exists");
        }
    }

    #region Getters
    public int GetValue(string p_id)
    {
        string path = Application.streamingAssetsPath + m_configPath;
        //Debug.Log($"path: found {path}");
        StreamReader reader = new StreamReader(path);

        int m_count = File.ReadAllLines(path).Length;

        string[] linesRead = File.ReadAllLines(path);

        foreach (var line in linesRead)
        {
            line.Trim(' ');
            var lineSplit = line.Split('=');

            if (lineSplit[0].Equals(p_id) || lineSplit[0].Contains(p_id))
            {
                return int.Parse(lineSplit[1]);
            }
        }
        reader.Close();
        Debug.LogAssertion("No Data Found");
        return -1;
    }

    public bool GetBool(string p_id)
    {
        string path = Application.streamingAssetsPath + m_configPath;
        StreamReader reader = new StreamReader(path);

        int m_count = File.ReadAllLines(path).Length;
        string[] linesRead = File.ReadAllLines(path);

        foreach (var line in linesRead)
        {
            line.Trim(' ');
            var lineSplit = line.Split('=');

            if (lineSplit[0].Equals(p_id) || lineSplit[0].Contains(p_id))
            {
                return lineSplit[1].Equals("true");
            }
        }
        reader.Close();
        Debug.LogAssertion("No Data Found");
        return false;
    }

    public string GetStringValue(string p_id)
    {
        string path = Application.streamingAssetsPath + m_configPath;
        //Debug.Log($"path: found {path}");
        StreamReader reader = new StreamReader(path);

        int m_count = File.ReadAllLines(path).Length;

        string[] linesRead = File.ReadAllLines(path);

        foreach (var line in linesRead)
        {
            line.Trim(' ');
            var lineSplit = line.Split('=');

            if (lineSplit[0].Equals(p_id) || lineSplit[0].Contains(p_id))
            {
                return lineSplit[1];
            }
        }

        reader.Close();
        Debug.LogAssertion($"No Data Found for ID:{p_id}");
        return "";
    }

    public float GetFloatValue(string p_id)
    {
        string path = Application.streamingAssetsPath + m_configPath;

        StreamReader reader = new StreamReader(path);

        int m_count = File.ReadAllLines(path).Length;

        string[] linesRead = File.ReadAllLines(path);

        foreach (var line in linesRead)
        {
            line.Trim(' ');
            var lineSplit = line.Split('=');
            if (lineSplit[0].Equals(p_id) || lineSplit[0].Contains(p_id))
            {
                return float.Parse(lineSplit[1]);
            }
        }
        reader.Close();
        return -1;
    }

    public string[] TopicPath()
    {
        string path = Application.streamingAssetsPath + m_excelPath;

        StreamReader reader = new StreamReader(path);

        string[] linesRead = File.ReadAllLines(path);
        Debug.Log(linesRead);

        return linesRead;
    }
   
    #endregion
}