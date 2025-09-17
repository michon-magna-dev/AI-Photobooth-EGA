#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public static class PrintHelper
{
    // Prints the image to the specified printer by invoking MS Paint's silent print.
    // scaleMode and landscape are kept for API compatibility but handled by driver settings.
    public static void PrintImage(string imagePath, string printerName = "Canon SELPHY CP1500", string scaleMode = "fill", bool landscape = true)
    {
        if (string.IsNullOrEmpty(imagePath)) throw new ArgumentException("imagePath is null/empty");
        if (!File.Exists(imagePath)) throw new FileNotFoundException("Image not found", imagePath);
        if (string.IsNullOrEmpty(printerName)) throw new ArgumentException("printerName is null/empty");

        // Use MS Paint to print silently to a specific printer:
        // /pt <filename> <printername> [<drivername> [<portname>]]
        // Note: Scaling and borderless are controlled by the printer driver defaults.
        var psi = new ProcessStartInfo
        {
            FileName = "mspaint.exe",
            Arguments = $"/pt \"{imagePath}\" \"{printerName}\"",
            UseShellExecute = false,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden
        };

        try
        {
            using (var proc = Process.Start(psi))
            {
                // Wait up to 30 seconds for MS Paint to hand off the job
                if (proc != null)
                    proc.WaitForExit(30000);
            }
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError($"Print failed via mspaint: {ex.Message}");
            throw;
        }
    }
}
#endif