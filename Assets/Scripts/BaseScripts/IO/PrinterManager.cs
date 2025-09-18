using System;
using System.Diagnostics;
using UnityEngine.UI;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;

public class PrinterManager : MonoBehaviour
{
    private static PrinterManager _instance;
    public static PrinterManager GetInstance() => _instance;

    [SerializeField] string m_printerName = "Canon TS8100 series";
    [SerializeField] string m_filePath = "C:\\ImagesFolder" + "\\1.jpg";
    [SerializeField] InputField m_customPath;
    [SerializeField] GameObject PrinterDebug;
    public bool showDebugger;
    [SerializeField] KeyCode m_keycodeDebug = KeyCode.F12;
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
        m_printerName = ConfigManager.GetInstance().GetStringValue("PRINTER_NAME");
        m_filePath = ConfigManager.GetInstance().GetStringValue("PRINTER_IMAGE_PATH");
        m_customPath.text = m_filePath;

        showDebugger = false;
        PrinterDebug.SetActive(showDebugger);
    }

    private void Update()
    {
        if (Input.GetKeyDown(m_keycodeDebug))
        {
            showDebugger = !showDebugger;
            PrinterDebug.SetActive(showDebugger);
        }
    }

    public void PrintCustomPath()
    {
        PrintImage(m_customPath.text);
    }

    public void PrintImageThread(string p_filePath)
    {
        Thread t = new Thread(() =>
        {
            PrintImage(p_filePath);
            //SendEmail("Michon", "michon@sparkslab.me", "\\\\MICHON-DEV-PC\\Shared Folder\\adro\\29Apr22\\jzkdi_7c9372b1-5c34-49b2-a71d-9d120e52e8a3.png");
        });
        t.Start();

    }
    public async Task PrintImageAsync(string p_filePath)
    {
        await Task.Run(() =>
        {
            PrintImage(p_filePath);
        });
    }
    public void PrintImage(string p_filePath)
    {
        UnityEngine.Debug.LogAssertion($"Printing Path Received: {p_filePath}");

        try
        {
            var newString = p_filePath.Replace(@"/", "\\");
            //var newString = p_filePath.Replace("\\", "/");
            var fullCommand = "rundll32 C:\\WINDOWS\\system32\\shimgvw.dll,ImageView_PrintTo " + "\"" + newString + "\"" + " " + "\"" + m_printerName + "\"";

            Process myProcess = new Process();
            //myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            myProcess.StartInfo.CreateNoWindow = true;
            myProcess.StartInfo.UseShellExecute = false;
            myProcess.StartInfo.FileName = "cmd.exe";
            myProcess.StartInfo.Arguments = "/c " + fullCommand;
            myProcess.EnableRaisingEvents = true;
            myProcess.Start();
            //myProcess.WaitForExit();
            UnityEngine.Debug.LogAssertion($"Printed {newString} to {m_printerName} : Successful");
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogAssertion($"Printing Failed: {p_filePath}");

            //UnityEngine.Debug.Log(e);
        }

    }

    public void PrintViaFileName(string p_fileName)
    {
        var date = System.DateTime.Now.ToString("ddMMMyy");
        var filePath = $"{m_filePath}/{date}/{p_fileName}";
        var fullCommand = "rundll32 C:\\WINDOWS\\system32\\shimgvw.dll,ImageView_PrintTo " + "\"" + filePath + "\"" + " " + "\"" + m_printerName + "\"";

        if (FileIOUtility.DoesPathExist(filePath))
            UnityEngine.Debug.Log($"File Exist : {filePath}");

        try
        {
            Process myProcess = new Process();
            //myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            myProcess.StartInfo.CreateNoWindow = true;
            myProcess.StartInfo.UseShellExecute = false;
            myProcess.StartInfo.FileName = "cmd.exe";
            myProcess.StartInfo.Arguments = "/c " + fullCommand;
            myProcess.EnableRaisingEvents = true;
            myProcess.Start();
            myProcess.WaitForExit();
            UnityEngine.Debug.LogAssertion($"Printed {m_filePath + p_fileName} to {m_printerName} : Successful");
        }
        catch (Exception e)
        {
            //UnityEngine.Debug.Log(e);
            UnityEngine.Debug.LogAssertion($"Printed {m_filePath + p_fileName} to {m_printerName} : Unsuccessful");
        }
    }

}