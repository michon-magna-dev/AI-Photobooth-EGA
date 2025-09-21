using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityGoogleDrive;

public class GoogleDriveHandler : MonoBehaviour
{
    public string UploadFilePath;
    public string result;
    public string url;
    private GoogleDriveFiles.CreateRequest request;

    public void UploadPhotoToDrive()
    {
        UploadTo(false);
    }

    private void UploadTo(bool toAppData)
    {
        string filename = Path.GetFileName(UploadFilePath);
        var content = File.ReadAllBytes(UploadFilePath);
        if (content == null || content.Length == 0) return;

        // Create the file object
        var file = new UnityGoogleDrive.Data.File
        {
            Name = filename,
            Content = content
        };

        // Set target folder
        string targetFolderId = toAppData
            ? "14LZ_a0FGYqsZZs3BEdRaUO-XKrztyNIk"  // Your shared folder ID
            : "19WGNPqF7vc1NbVCeDp2gBgdulr2psR52";          // Replace with another folder if needed

        file.Parents = new List<string> { targetFolderId };

        // Create upload request
        var request = GoogleDriveFiles.Create(file);
        request.Fields = new List<string> { "id", "name", "size", "createdTime", "webViewLink" };

        // Send upload request
        request.Send().OnDone += PrintResult;
    }

    private void Upload(bool toAppData)
    {
        //string filename = string.Format(@"{0}_{1}.jpg", "PhotoBooth", loadImage.capturedImageCount.ToString());
        //string filePath = System.IO.Path.Combine(Application.streamingAssetsPath + "/ImagesToMail/", filename);

        string filename = "";
        var content = File.ReadAllBytes(UploadFilePath);
        if (content == null) return;

        //var file = new UnityGoogleDrive.Data.File() { Name = Path.GetFileName(UploadFilePath), Content = content };
        var file = new UnityGoogleDrive.Data.File() { Name = Path.GetFileName(UploadFilePath), Content = content };
        if (toAppData) file.Parents = new List<string> { "14LZ_a0FGYqsZZs3BEdRaUO-XKrztyNIk" }; //1QstFXrsmot5MmtIdZd3aZOHxsSTt1UWS  // folder shared code
        //https://drive.google.com/drive/folders/14LZ_a0FGYqsZZs3BEdRaUO-XKrztyNIk?usp=sharing

        request = GoogleDriveFiles.Create(file);
        request.Fields = new List<string> { "id", "name", "size", "createdTime" };
        request.Send().OnDone += PrintResult;
    }

    private void PrintResult(UnityGoogleDrive.Data.File file)
    {

        result = string.Format("Name: {0} Size: {1:0.00}MB Created: {2:dd.MM.yyyy HH:MM:ss}\nID: {3}",
                file.Name,
                file.Size * .000001f,
                file.CreatedTime,
                file.Id);

        // Debug.Log("Finish UPLOAD");


        file.ViewersCanCopyContent = true;
        file.WritersCanShare = true;


        // Debug.Log("DO QR CODE SCRIPT");
        //https://drive.google.com/file/d/1QKXSExhHtI-56OhHhw9f2C9XNCU50feO/view?usp=drive_link
        url = "https://drive.google.com/file/d/" + file.Id + "/view?usp=share_link";        //qrcode link
        
        // Debug.Log(url + "     " + file.Name);
    }


    protected void OnGUI()
    {
        if (request != null && request.IsRunning)
        {
            GUILayout.Label($"Loading: {request.Progress:P2}");
        }
        else
        {
            UploadFilePath = GUILayout.TextField(UploadFilePath);
            if (GUILayout.Button("Upload To Root")) UploadTo(false);
            if (GUILayout.Button("Upload To AddData")) UploadTo(true);
        }

        if (!string.IsNullOrEmpty(result))
        {
            GUILayout.TextField(result);
        }
    }
}
