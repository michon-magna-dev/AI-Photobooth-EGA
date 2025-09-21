using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class EmailPostHandler : MonoBehaviour
{
    [SerializeField] string pythonServerURL = "http://localhost:5000";
    [SerializeField] bool isSendingEmail;

    void Start()
    {
        isSendingEmail = false;
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                SendEmail("michon@magnainnovations.com", "C:\\Users\\Michon\\OneDrive\\Documents\\Unity\\_Magna Innovations Projects\\AI-Photobooth-ECG\\AI-Photobooth-Frontend\\Assets\\StreamingAssets\\ProcessedPhotos\\20250921_001647\\20250921_001647_single_male_1_male_solo_01.jpg");
            }
        }
#endif
    }

    public void SendCurrentUserEmail()
    {
        var emailReceiver = GameManager.Instance.GetUserEmail;
        var photoPath = GameManager.Instance.GetUserPhotoPath;
        SendEmail(emailReceiver, photoPath);
    }

    public void SendEmail(string p_emailReceiver, string p_imagePath)
    {
        if (isSendingEmail)
        {
            return;
        }
        StartCoroutine(SendEmailCoroutine(p_emailReceiver, p_imagePath));
    }

    public IEnumerator SendEmailCoroutine(string p_emailReceiver, string p_imagePath)
    {
        isSendingEmail = true;
        ProcessEmailRequest request = new ProcessEmailRequest
        {
            email_receiver = p_emailReceiver,
            image_path = p_imagePath,
        };

        string jsonData = JsonUtility.ToJson(request);

        using (UnityWebRequest webRequest = new UnityWebRequest($"{pythonServerURL}/send_mail", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                ProcessResponseEmail response = JsonUtility.FromJson<ProcessResponseEmail>(webRequest.downloadHandler.text);

                if (response.success)
                {
                    Debug.LogAssertion("Email Success: " + response.message);
                }
                else
                {
                    Debug.LogAssertion("Email failed: " + response.message);
                }
                isSendingEmail = false;
            }
            else
            {
                Debug.LogAssertion("Email error: " + webRequest.error);
                isSendingEmail = false;
            }
        }
    }
}
[Serializable]
public class ProcessResponseEmail
{
    public bool success;
    public string message;
}

[Serializable]
public class ProcessEmailRequest
{
    public string email_receiver;
    public string image_path;
}