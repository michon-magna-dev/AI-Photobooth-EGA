using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PythonServerHandler : MonoBehaviour
{
    private Process _pythonProcess;
    public string exePath = "install_and_run.py";
    public string exeFullPath;
    
    void Start()
    {
        string pythonScriptPath = Path.Combine(Application.streamingAssetsPath, exePath);
        exeFullPath = pythonScriptPath;
        RunPythonScriptIfNotRunning(pythonScriptPath);
    }

    void RunPythonScriptIfNotRunning(string scriptPath)
    {
        if (_pythonProcess == null || _pythonProcess.HasExited)
        {
            _pythonProcess = new Process();
            _pythonProcess.StartInfo.FileName = "python";
            _pythonProcess.StartInfo.Arguments = scriptPath;
            _pythonProcess.StartInfo.UseShellExecute = false;
            _pythonProcess.StartInfo.CreateNoWindow = false;
            _pythonProcess.Start();
            Debug.Log("Python script started.");
        }
        else
        {
            Debug.Log("Python script is already running.");
        }
    }

    void OnApplicationQuit()
    {
        if (_pythonProcess != null && !_pythonProcess.HasExited)
        {
            _pythonProcess.Kill();
            Debug.Log("Python script stopped on application exit.");
        }
    }
}