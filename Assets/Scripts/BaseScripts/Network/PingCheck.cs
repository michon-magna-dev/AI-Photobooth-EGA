using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net.NetworkInformation;
using System.Threading;
using System;
using System.Threading.Tasks;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class PingCheck : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] GameObject m_prefab;
    [SerializeField] GameObject m_contentParent;

    [Header("Values")]
    [SerializeField] bool m_connected = false;
    [SerializeField] List<string> ip;
    [SerializeField] bool[] pingSuccess;
    [SerializeField] int m_deviceCount;

    //public delegate void OnReceivePing(Toggle p_toggle);
    //public event OnReceivePing receivePing;

    Task[] pingTasks;
    CancellationTokenSource tokenSource;
    public bool boolDebug = false;

    private void Awake()
    {
        //UnityThread.initUnityThread();
        tokenSource = new CancellationTokenSource();
    }

    private void Start()
    {
        InstantiateUIGroups();
        pingSuccess = new bool[ip.Count];
        //CheckConnection();
        //SampleAsync();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            boolDebug = !boolDebug;
            UpdateUI(boolDebug);
        }
    }

    void InstantiateUIGroups()
    {
        m_deviceCount = int.Parse(ConfigManager.GetInstance().GetStringValue("DEVICE_COUNT"));
        for (int i = 1; i <= m_deviceCount; i++)
        {
            ip.Add(ConfigManager.GetInstance().GetStringValue("KIOSK " + i));
            var group = Instantiate(m_prefab, m_contentParent.transform);
            group.GetComponentInChildren<Text>().text = "Kiosk " + i + ":";
        }
    }

    public async void CheckConnection()
    {
        UpdateUI(false);
        pingTasks = new Task[ip.Count];

        for (int i = 0; i < ip.Count; i++)
        {
            //A: Setup and stuff you don't want timed
            var timer = new Stopwatch();
            timer.Start();

            //B: Run stuff you want timed
            pingTasks[i] =  StartThreads(i);

            timer.Stop();

            TimeSpan timeTaken = timer.Elapsed;
            string foo = $"Pingging {ip[i]} Time taken: " + timeTaken.ToString(@"m\:ss\.fff");
            Debug.Log(foo);
        }

        await Task.WhenAll(pingTasks);
        Debug.Log($"Done Ping....");
        UpdateUI();
    }

    async Task StartThreads(int p_index)
    {
        Task t = Task.Run(() =>
        {
            Debug.Log($"Checking Connection {ip[p_index]}....");
            pingTasks[p_index] = PingHostAsync(ip[p_index], p_index);
        });

        //new Thread(() =>
        //{
        //    Debug.Log($"Checking Connection {ip[p_index]}....");
        //    pingTasks[p_index] = PingHostAsync(ip[p_index], p_index);
        //}
        //).Start();

        //Debug.Log($"Checking Connection {ip[p_index]}....");
        //pingTasks[p_index] = PingHostAsync(ip[p_index], p_index);
        if (tokenSource.Token.IsCancellationRequested)
        {
            return;
        }
        //yield return null
        //await Task.Yield();
        await t;

    }

    public async Task PingHostAsync(string nameOrAddress, int p_index)
    {
        //A: Setup and stuff you don't want timed
        System.Net.NetworkInformation.Ping pinger = null;

        try
        {
            pinger = new System.Net.NetworkInformation.Ping();
            PingReply reply = pinger.Send(nameOrAddress);
            pingSuccess[p_index] = reply.Status == IPStatus.Success;
            Debug.Log($"pinged {nameOrAddress} returned {reply.Status == IPStatus.Success}");
        }
        catch (PingException e)
        {
            Debug.Log($"Ping Failed: {e.Message}");
            // Discard PingExceptions and return false;
        }

        finally
        {
            if (pinger != null)
            {
                pinger.Dispose();
            }
        }

        await Task.Yield();
    }
    void UpdateUI()
    {
        Debug.Log($"Ping Tasks Complete");
        for (int i = 0; i < ip.Count; i++)
        {
            var toggleChild = m_contentParent.transform.GetChild(i);
            toggleChild.GetComponentInChildren<Toggle>().isOn = pingSuccess[i];
        }
    }

    void UpdateUI(bool p_Enable)
    {
        Debug.Log($"Ping Tasks Complete");
        for (int i = 0; i < ip.Count; i++)
        {
            var toggleChild = m_contentParent.transform.GetChild(i);
            toggleChild.GetComponentInChildren<Toggle>().isOn = p_Enable;
        }
    }

    #region End Tasks

    private void OnDisable()
    {
        tokenSource.Cancel();
    }

    [ContextMenu("Stop Tasks")]
    public void StopTasks()
    {
        tokenSource.Cancel();
    }
    #endregion

    #region Sample Async With Cancellation

    public async void SampleAsync()//Check Connection
    {
        Debug.Log("Started");
        await sampleTask();
        Debug.Log("Ended");
    }

    public async Task sampleTask()// Ping
    {
        for (int i = 0; i < 10000; i++)//simulates very long process (Image Loading or Ping)
        {
            Debug.Log(i);
            await Task.Yield();
            if (tokenSource.Token.IsCancellationRequested)
            {
                return;
            }
        }

    }
    #endregion

}