using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MZBApplicationHelpers
{
    public static class SystemFileFinder
    {
        public static string[] GetFile(string p_folderPath, string p_fileType = ".mp4")
        {
            Debug.Log(Directory.Exists(Application.streamingAssetsPath));
            Debug.Assert(Directory.Exists(p_folderPath));

            var di = new DirectoryInfo(p_folderPath);
            var list = new List<string>();

            foreach (var file in di.GetFiles($"*{p_fileType}", SearchOption.AllDirectories))
            {
                list.Add(file.ToString());
            }
            return list.ToArray();
        }

        public static string[] GetVideoFilePaths(string p_folderPath)
        {
            var di = new DirectoryInfo(p_folderPath);
            var list = new List<string>();

            foreach (var file in di.GetFiles("*.mp4", SearchOption.AllDirectories))
            {
                list.Add(file.ToString());
            }
            return list.ToArray();
        }

        public static string[] GetPNGImageFilePaths(string p_folderPath)
        {
            Debug.Log(Directory.Exists(Application.streamingAssetsPath));
            Debug.Assert(Directory.Exists(p_folderPath));

            var di = new DirectoryInfo(p_folderPath);
            var list = new List<string>();

            foreach (var file in di.GetFiles("*.png", SearchOption.AllDirectories))
            {
                list.Add(file.ToString());
            }
            return list.ToArray();
        }

    }
}