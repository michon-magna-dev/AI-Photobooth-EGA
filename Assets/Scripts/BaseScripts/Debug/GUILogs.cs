using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO.IsolatedStorage;

public class GUILogs : MonoBehaviour
{
    public GameObject textPrefab;
    public Transform contentParent;
    public string output = "";
    public string stack = "";
    public int what;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Log");
            Debug.LogError("error");

            int[] arr = new int[5];
            for (int i = 0; i < 10; i++)
            {
                Debug.Log(arr[i]);
            }
        }
    }

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        Color textColor = Color.white;
        switch (type)
        {
            case LogType.Error:
                textColor = Color.red;
                break;
            case LogType.Assert:
                textColor = Color.yellow;
                break;
            case LogType.Warning:
                textColor = Color.red;
                break;
            case LogType.Log:
                textColor = Color.white;
                break;
            case LogType.Exception:
                textColor = Color.red;
                break;
            default:
                break;
        }

        var log = Instantiate(textPrefab, contentParent);
        var textComponent = log.GetComponent<TextMeshProUGUI>();

        textComponent.text = $"{type}: {logString}";
        textComponent.color = textColor;

        output = logString;
        stack = stackTrace;

    }
}