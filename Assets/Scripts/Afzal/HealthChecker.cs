using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;


public class HealthChecker : MonoBehaviour
{
    [Header("UI")]
    public GameObject serverImage;

    [Header("Server Settings")]
    public string serverUrl = "http://localhost:5000"; // adjust port if different  
    public float checkInterval = 5f; // check every 5 seconds  
    public float timeoutSeconds = 3f;

    [Header("Status")]
    public bool isServerOnline = false;
    public string lastCheckTime = "";

    // Events you can subscribe to  
    public System.Action<bool> OnServerStatusChanged;

    private Coroutine healthCheckCoroutine;

    private void Start()
    {
        StartHealthChecking();
    }

    public void StartHealthChecking()
    {
        if (healthCheckCoroutine != null)
            StopCoroutine(healthCheckCoroutine);
        healthCheckCoroutine = StartCoroutine(PeriodicHealthCheck());

        if (this != null)
            OnServerStatusChanged += OnServerStatusChanged_Checking;

        UpdatePrintButtonState();
    }

    private void OnServerStatusChanged_Checking(bool isOnline)
    {
        UpdatePrintButtonState();

        if (isOnline)
        {
           // Debug.Log("Server came online - print functionality enabled");
        }
        else
        {
            //Debug.Log("Server went offline - print functionality disabled");
        }
    }

    public void StopHealthChecking()
    {
        if (healthCheckCoroutine != null)
        {
            StopCoroutine(healthCheckCoroutine);
            healthCheckCoroutine = null;
        }
    }

    private IEnumerator PeriodicHealthCheck()
    {
        while (true)
        {
            yield return StartCoroutine(CheckServerHealth());
            yield return new WaitForSeconds(checkInterval);
        }
    }

    public IEnumerator CheckServerHealth()
    {
        string healthUrl = serverUrl + "/health";

        using (UnityWebRequest req = UnityWebRequest.Get(healthUrl))
        {
            req.timeout = Mathf.CeilToInt(timeoutSeconds);
            yield return req.SendWebRequest();

            bool wasOnline = isServerOnline;
            lastCheckTime = System.DateTime.Now.ToString("HH:mm:ss");

#if UNITY_2020_1_OR_NEWER
            bool hasError = req.result == UnityWebRequest.Result.ConnectionError ||
                           req.result == UnityWebRequest.Result.ProtocolError;
#else
            bool hasError = req.isNetworkError || req.isHttpError;  
#endif

            if (!hasError && req.responseCode == 200)
            {
                isServerOnline = true;
                //Debug.Log($"[{lastCheckTime}] Server is online");
            }
            else
            {
                isServerOnline = false;
                //Debug.LogWarning($"[{lastCheckTime}] Server is offline. Error: {req.error}");
            }

            // Trigger event if status changed  
            if (wasOnline != isServerOnline)
            {
                OnServerStatusChanged?.Invoke(isServerOnline);
            }
        }
    }

    // Manual check (call this from UI button)  
    public void CheckNow()
    {
        StartCoroutine(CheckServerHealth());
    }

    private void UpdatePrintButtonState()
    {
        bool serverOnline = this != null && isServerOnline;

        if (serverImage != null)
        {
            serverImage.SetActive(!serverOnline);
        }

        //if (serverStatusText != null)
        //{
        //    serverStatusText.text = serverOnline ? "Server: Online" : "Server: Offline";
        //    serverStatusText.color = serverOnline ? Color.green : Color.red;
        //}
    }

    private void OnDestroy()
    {
        if (this != null)
            OnServerStatusChanged -= OnServerStatusChanged_Checking;

        StopHealthChecking();
    }
}
