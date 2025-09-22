using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class SaveDetails : MonoBehaviour
{

    [Serializable]
    public class Registration
    {
        public string name;
        public string email;
        public string contact;

        public string timestamp;
    }

    public enum SaveMode
    {
        AppendToMaster,    // Append to a single master CSV file
        NewFilePerSession  // Create a new CSV per save (timestamped)
    }

    [Header("CSV Settings")]
    public SaveMode saveMode = SaveMode.AppendToMaster;
    public string masterFileName = "registrations.csv"; // used when AppendToMaster
    public string sessionFilePrefix = "registrations_session_"; // used when NewFilePerSession
    public string csvFileFolder = "RegistrationDetails"; // optional subfolder under Application.persistentDataPath
    public bool includeHeader = true; // header will be written if creating a new file

    // Internal state
    [SerializeField]
    public List<Registration> pendingRegistrations = new List<Registration>();
    private readonly object fileLock = new object();

    private string GetFolderPath()
    {
        string basePath = Application.streamingAssetsPath;
        if (!string.IsNullOrEmpty(csvFileFolder))
        {
            basePath = Path.Combine(basePath, csvFileFolder);
        }
        if (!Directory.Exists(basePath)) Directory.CreateDirectory(basePath);
        return basePath;
    }

    // Call this during the session for each user
    public void AddRegistration(string name, string email)
    {
        if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(email)) return;
        var reg = new Registration
        {
            name = name ?? "",
            email = email ?? "",

            //timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") // UTC; change to Local if desired
            timestamp = DateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss") // UTC; change to Local if desired
        };
        pendingRegistrations.Add(reg);
    }
    public void AddRegistration(string name, string email, string contact)
    {
        if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(email)) return;
        var reg = new Registration
        {
            name = name ?? "",
            email = email ?? "",
            contact = contact ?? "",
            //timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") // UTC; change to Local if desired
            timestamp = DateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss") // UTC; change to Local if desired
        };
        pendingRegistrations.Add(reg);
    }

    // Get number of pending registrations
    public int GetPendingCount()
    {
        return pendingRegistrations.Count;
    }

    // Save all pending registrations according to saveMode. Returns path(s) written.
    // If appendToMaster = true (or saveMode==AppendToMaster), will append to master file and return master path.
    // If saveMode==NewFilePerSession, will create a timestamped CSV and return its path.

    public string SaveAllRegistrations()
    {
        lock (fileLock)
        {
            if (pendingRegistrations.Count == 0)
            {
                Debug.LogWarning("No registrations to save.");
                return null;
            }

            string folder = GetFolderPath();
            if (saveMode == SaveMode.AppendToMaster)
            {
                string masterPath = Path.Combine(folder, masterFileName);
                bool masterExists = File.Exists(masterPath);

                // Ensure header if creating new file
                if (!masterExists && includeHeader)
                {
                    WriteBomAndHeader(masterPath);
                }

                // Append each pending registration
                using (var sw = new StreamWriter(masterPath, true, new UTF8Encoding(true)))
                {
                    foreach (var r in pendingRegistrations)
                    {
                        sw.WriteLine($"{EscapeCsvField(r.name)},{EscapeCsvField(r.email)},{EscapeCsvField(r.contact)},{EscapeCsvField(r.timestamp)}");
                    }
                }

                Debug.Log($"Appended {pendingRegistrations.Count} registrations to {masterPath}");
                return masterPath;
            }
            else // NewFilePerSession
            {
                string timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
                string fileName = $"{sessionFilePrefix}{timestamp}.csv";
                string path = Path.Combine(folder, fileName);

                using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
                using (var sw = new StreamWriter(fs, new UTF8Encoding(true)))
                {
                    if (includeHeader)
                    {
                        sw.WriteLine("Name,Email,Contact,Timestamp");
                    }
                    foreach (var r in pendingRegistrations)
                    {
                        sw.WriteLine($"{EscapeCsvField(r.name)},{EscapeCsvField(r.email)},{EscapeCsvField(r.timestamp)}");
                    }
                }

                Debug.Log($"Wrote {pendingRegistrations.Count} registrations to {path}");
                return path;
            }
        }
    }

    // Save and clear the pending registrations list (call at end of session)
    public string SaveAllAndClear()
    {
        string path = SaveAllRegistrations();
        if (!string.IsNullOrEmpty(path))
        {
            pendingRegistrations.Clear();
        }
        return path;
    }

    // Clear pending registrations without saving
    public void ClearPending()
    {
        pendingRegistrations.Clear();
    }

    // Helper to write BOM + header if file doesn't exist
    private void WriteBomAndHeader(string path)
    {
        byte[] bom = new byte[] { 0xEF, 0xBB, 0xBF };
        string header = "Name,Email,Contact,Timestamp\r\n";
        using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            fs.Write(bom, 0, bom.Length);
            var headerBytes = Encoding.UTF8.GetBytes(header);
            fs.Write(headerBytes, 0, headerBytes.Length);
        }
    }

    // CSV escaping (wraps in quotes if needed and doubles internal quotes)
    private string EscapeCsvField(string s)
    {
        if (s == null) return "\"\"";
        bool mustQuote = s.Contains(",") || s.Contains("\"") || s.Contains("\r") || s.Contains("\n");
        s = s.Replace("\"", "\"\""); // escape quotes
        return mustQuote ? $"\"{s}\"" : s;
    }

    // Optional: get a copy of pending registrations (read-only)
    public List<Registration> GetPendingRegistrationsCopy()
    {
        return new List<Registration>(pendingRegistrations);
    }
}