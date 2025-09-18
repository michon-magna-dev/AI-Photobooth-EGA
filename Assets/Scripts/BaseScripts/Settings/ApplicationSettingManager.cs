using System;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public enum Setting
{
    Value,
    Bool
}

[Serializable]
public class SettingType
{
    public string name;
    public string key;
    public string value;
    public Setting type;
}
public class ApplicationSettingManager : MonoBehaviour
{
    private static ApplicationSettingManager _instance;
    public static ApplicationSettingManager GetInstance() => _instance;
    [SerializeField] GameObject m_settingsWindow;
    [SerializeField] GameObject m_logsWindow;
    [SerializeField] Button m_hiddenButton;
    [SerializeField] SettingType[] m_settings;

    [Header("Prefab")]
    [SerializeField] Transform m_settingsContainer;
    [SerializeField] GameObject m_settingValPrefab;
    [SerializeField] GameObject m_settingBoolPrefab;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        GetPlayerPrefSettings();
        InitSettings();
        ShowSettings(false);
        ShowLogs(false);
    }

    void GetPlayerPrefSettings()
    {
#if UNITY_EDITOR
        DeleteAllPlayerPrefs();
#endif
        foreach (var setting in m_settings)
        {
            var defaultValue = setting.value;
            var value = GetSettingValue(setting.key);

            bool isNotNull = !value.Equals(null);
            bool isNotEmpty = !value.Equals("");

            bool prefExists = isNotNull && isNotEmpty;

            if (prefExists)
            {
                Debug.Log($"Found Player Pref : {setting.key}, {GetSettingValue(setting.key)}");
            }
            else
            {
                Debug.Log($"PREF IS NULL : {setting.key}, {GetSettingValueFromList(setting.key).Equals(null)}");
                SaveSettingValue(setting.key, setting.value);
            }
            setting.value = prefExists ? GetSettingValue(setting.key) : defaultValue;
        }
    }

    void InitSettings()
    {
        for (int settingIndex = 0; settingIndex < m_settings.Length; settingIndex++)
        {
            var setting = m_settings[settingIndex];
            var settingField = Instantiate(setting.type.Equals(Setting.Value) ? m_settingValPrefab : m_settingBoolPrefab, m_settingsContainer);
            settingField.GetComponentInChildren<TextMeshProUGUI>().text = setting.name;

            var inputField = settingField.GetComponentInChildren<TMP_InputField>();
            inputField.contentType = TMP_InputField.ContentType.DecimalNumber;
            inputField.text = setting.value;

            inputField.onValueChanged.AddListener((newValue) =>
            {
                SaveSettingValue(setting.key, newValue);
                //Debug.LogAssertion($"Saved New Value : key = {setting.key} :  val = {newValue}");
                //Debug.LogAssertion($"Test: New Value : val= {GetSettingValue(setting.key)}");
            });
        }
    }

    #region UI
    public void ShowSettings()
    {
        m_settingsWindow.SetActive(!m_settingsWindow.activeInHierarchy);
    }

    public void ShowLogs()
    {
        m_logsWindow.SetActive(!m_logsWindow.activeInHierarchy);
    }

    public void ShowSettings(bool p_show)
    {
        m_settingsWindow.SetActive(p_show);
    }

    public void ShowLogs(bool p_show)
    {
        m_logsWindow.SetActive(p_show);
    }
    #endregion

    #region Public Getters

    public string GetSettingValueFromList(string p_key)
    {
        SettingType setting = m_settings.FirstOrDefault(settingItem => settingItem.key == p_key);
        return setting?.value;
    }

    #endregion

    #region Player Prefs
    public string GetSettingValue(string p_key)
    {
        return PlayerPrefs.GetString(p_key);
    }

    private void SaveSettingValue(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
    }
    #endregion

#if UNITY_EDITOR

    #region Tools
    [MenuItem("Window/Delete PlayerPrefs (All)")]
    static void DeleteAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
    #endregion
#endif

}