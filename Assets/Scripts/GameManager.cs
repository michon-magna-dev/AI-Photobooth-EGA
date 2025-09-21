using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;



public class GameManager : MonoBehaviour
{
    public static GameManager _instance;
    public static GameManager Instance => _instance;
    [Serializable]
    public class UserData
    {
        public string name;
        public string email;
        public string contactNumber;
        public string gender; // "male" or "female"
        public string photoPath;
    }
    private enum AppState
    {
        Welcome,
        UserCount,
        Registration,
        PromptSelect,
        Camera,
        Review,
        Processing,
        Final
    }

    private AppState currentState;

    public Action<string> OnPhotoProcessed;

    [Header("UI Panels")]
    public GameObject panelWelcome;
    //public GameObject panelUserCount;
    public GameObject panelPromptSelect;
    public GameObject panelRegistration;
    public GameObject panelRegistration_Disclaimer;
    public GameObject panelCamera;
    public GameObject panelCamera_RetakeOnFail;
    public GameObject panelReview;
    public GameObject panelProcessing;
    public GameObject panelFinal;

    [Header("Welcome Panel")]
    public Button btnStartGame;

    [Header("Prompt Selection Panel")]
    public Button btnprompSelect;

    [Header("User Count Panel")]
    //public Dropdown userCountDropdown;
    public Button btnUser1;
    public Button btnUser2;
    public Button btnUser3;
    public Button btnUserCountStart;

    [Header("Registration Panel")]
    public Image imgUser1;
    public Image imgUser2;
    public Image imgUser3;

    public InputField nameInput;
    public InputField emailInput;
    public InputField contactInput;
    //public Dropdown genderDropdown;
    public Button btnMale;
    public Button btnFemale;
    public Toggle termsToggle;
    public Button btnRegisterNext;
    public GameObject keytboardGo;

    [Header("Camera Panel")]
    public RawImage cameraFeed;
    public Button btnTakePhoto;
    public GameObject threetwooneImage;

    [Header("Review Panel")]
    public RawImage reviewImage;
    public Button btnRetake;
    public Button btnContinue;

    [Header("Final Panel")]
    public RawImage finalResultImage;
    public Button btnReset;
    public Button btnPrint;

    //private List<string> mergedImagePaths = new List<string>(); //Optional to display from array list

    [Header("References")]
    public ImageSelectionPromptHandler imageSelectionPromptHandler;
    public PhotoManager photoManager;
    public MergeImage mergeImage;
    public SaveDetails saveDetails;

    [Header("Server Settings")]
    public string pythonServerURL = "http://localhost:5000";

    [Header("Printing")]
    public bool autoPrint = true;
    public string printerName = "Canon SELPHY CP1500";
    public string printScaleMode = "fit"; // "fill" or "fit"
    public bool printLandscape = false;     // true if your images are landscape (1800x1200)

    [SerializeField] string imagePathToPrint;        // set this to your last saved result (full Windows path)

    private List<UserData> registeredUsers = new List<UserData>();
    private List<string> resultImagePaths = new List<string>();
    public int currentUserIndex = 0;
    public int totalUsers = 1;
    private int currentResultIndex = 0;
    private string sessionID;
    private Texture2D lastCapturedPhoto;


    // New variables for button-based selection  
    private int selectedUserCount = 1;
    [SerializeField] string selectedGender = "male";
    [SerializeField] public Text photoUserCountText;
    public Text photoUserCountText_OnReviewPage;

    [Header("Button Sprites")]
    [Header("User Count Button Sprites")]
    public Sprite soloSelectedSprite;
    public Sprite soloUnselectedSprite;
    public Sprite duoSelectedSprite;
    public Sprite duoUnselectedSprite;
    public Sprite trioSelectedSprite;
    public Sprite trioUnselectedSprite;

    [Header("Gender Button Sprites")]
    public Sprite maleSelectedSprite;
    public Sprite maleUnselectedSprite;
    public Sprite femaleSelectedSprite;
    public Sprite femaleUnselectedSprite;

    [Header("Regsitration User Sprites")]
    public Sprite user_1_SelectedSprite;
    public Sprite user_1_UnselectedSprite;
    public Sprite user_2_SelectedSprite;
    public Sprite user_2_UnselectedSprite;
    public Sprite user_3_SelectedSprite;
    public Sprite user_3_UnselectedSprite;

    #region Getters
    public ImageSelectionPromptHandler.PromptType GetSelectedGender => selectedGender.Equals("male") ? ImageSelectionPromptHandler.PromptType.Male : ImageSelectionPromptHandler.PromptType.Female;
    public string GetUserEmail => registeredUsers[0].email;
    public string GetUserPhotoPath => registeredUsers[0].photoPath;
    #endregion

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    void Start()
    {
        InitializeApplication();
        SetupUI();
        SetState(AppState.Welcome);
    }

    void InitializeApplication()
    {
        sessionID = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");

        if (photoManager == null)
            photoManager = FindObjectOfType<PhotoManager>();

        if (photoManager != null)
        {
            photoManager.Initialize(sessionID);
            photoManager.OnPhotoTaken += OnPhotoTaken;
        }
        else
        {
            Debug.LogError("PhotoManager not found! Please assign it in the inspector.");
        }
    }

    void SetupUI()
    {
        // Gender buttons  
        btnMale.onClick.AddListener(() => OnGenderSelected("male"));
        btnFemale.onClick.AddListener(() => OnGenderSelected("female"));

        // User count buttons  
        btnUser1.onClick.AddListener(() => OnUserCountSelected(1));
        btnUser2.onClick.AddListener(() => OnUserCountSelected(2));
        btnUser3.onClick.AddListener(() => OnUserCountSelected(3));
        btnUserCountStart.onClick.AddListener(OnUserCountStart);

        btnStartGame.onClick.AddListener(OnStartGame);
        btnprompSelect.onClick.AddListener(OnPromptSelect);
        btnRegisterNext.onClick.AddListener(OnRegisterNext);
        btnTakePhoto.onClick.AddListener(OnTakePhoto);
        btnRetake.onClick.AddListener(OnRetake);

        //MICHON NOTE: REMOVED PRINT
        //btnPrint.onClick.AddListener(OnPrint);

        btnContinue.onClick.AddListener(OnContinue);
        btnReset.onClick.AddListener(OnReset);

        // Setup email toggle  
        termsToggle.isOn = false;
        btnPrint.interactable = true;

        if (photoManager != null && cameraFeed != null)
            photoManager.SetCameraFeed(cameraFeed);

        // Initialize UI state  
        UpdateUserCountButtons();
        UpdateGenderButtons();
    }

    void OnUserCountSelected(int count)
    {
        selectedUserCount = count;
        UpdateUserCountButtons();
    }

    void OnGenderSelected(string gender)
    {
        selectedGender = gender;
        UpdateGenderButtons();
    }

    void UpdateUserCountButtons()
    {
        if (btnUser1 != null)
        {
            btnUser1.image.sprite = selectedUserCount == 1 ? soloSelectedSprite : soloUnselectedSprite;
        }

        if (btnUser2 != null)
        {
            btnUser2.image.sprite = selectedUserCount == 2 ? duoSelectedSprite : duoUnselectedSprite;
        }

        if (btnUser3 != null)
        {
            btnUser3.image.sprite = selectedUserCount == 3 ? trioSelectedSprite : trioUnselectedSprite;
        }
    }

    void UpdateGenderButtons()
    {
        if (btnMale != null)
        {
            btnMale.image.sprite = selectedGender == "male" ? maleSelectedSprite : maleUnselectedSprite;
        }

        if (btnFemale != null)
        {
            btnFemale.image.sprite = selectedGender == "female" ? femaleSelectedSprite : femaleUnselectedSprite;
        }
    }

    void UpdateUserNumberImage(int userNo)
    {

        if (totalUsers == 1)
        {
            imgUser1.gameObject.SetActive(true);
            imgUser2.gameObject.SetActive(false);
            imgUser3.gameObject.SetActive(false);
        }
        else if (totalUsers == 2)
        {
            imgUser1.gameObject.SetActive(true);
            imgUser2.gameObject.SetActive(true);
            imgUser3.gameObject.SetActive(false);
        }
        else if (totalUsers == 3)
        {
            imgUser1.gameObject.SetActive(true);
            imgUser2.gameObject.SetActive(true);
            imgUser3.gameObject.SetActive(true);
        }

        if (imgUser1 != null)
        {
            imgUser1.sprite = userNo == 1 ? user_1_SelectedSprite : user_1_UnselectedSprite;
        }

        if (imgUser2 != null)
        {
            imgUser2.sprite = userNo == 2 ? user_2_SelectedSprite : user_2_UnselectedSprite;
        }

        if (imgUser3 != null)
        {
            imgUser3.sprite = userNo == 3 ? user_3_SelectedSprite : user_3_UnselectedSprite;
        }
    }

    void SetState(AppState newState)
    {
        currentState = newState;
        UpdatePanels();
        UpdateStateSpecificUI();
    }

    void UpdatePanels()
    {
        panelWelcome.SetActive(currentState == AppState.Welcome);
        //panelUserCount.SetActive(currentState == AppState.UserCount);
        panelPromptSelect.SetActive(currentState == AppState.PromptSelect);
        panelRegistration.SetActive(currentState == AppState.Registration);
        panelCamera.SetActive(currentState == AppState.Camera);
        panelReview.SetActive(currentState == AppState.Review);
        panelProcessing.SetActive(currentState == AppState.Processing);
        panelFinal.SetActive(currentState == AppState.Final);
    }

    void UpdateStateSpecificUI()
    {
        switch (currentState)
        {
            case AppState.UserCount:
                UpdateUserCountButtons();
                break;
            case AppState.Registration:
                //nameInput.text = "";
                //if (emailInput != null) emailInput.text = "";
                //selectedGender = "male";
                //UpdateGenderButtons();
                if (currentUserIndex >= registeredUsers.Count)
                {
                    nameInput.text = "";
                    if (emailInput != null) emailInput.text = "";
                    selectedGender = "male";
                    contactInput.text = "";
                    UpdateGenderButtons();
                    UpdateUserNumberImage(currentResultIndex + 1);
                    if (termsToggle != null) termsToggle.isOn = false;
                }
                break;
            case AppState.Final:
                UpdateFinalUI();

                break;
        }
    }

    void UpdateFinalUI()
    {
        if (resultImagePaths.Count > 0)
        {
            StartCoroutine(ProcessAndMergeImages());

            foreach (var register in registeredUsers)
            {
                saveDetails.AddRegistration(register.name, register.email);
            }
            string savedPath = saveDetails.SaveAllAndClear();
            Debug.Log(savedPath);
        }
        else
        {
            //  finalResultImage.texture = null;
        }
    }

    IEnumerator ProcessAndMergeImages()
    {
        if (mergeImage == null)
        {
            Debug.LogError("MergeImage component not assigned!");
            yield break;
        }

        List<string> mergedPaths = mergeImage.MergeMultipleImages(resultImagePaths);

        //mergedImagePaths = mergedPaths; //Optional to display from array list

        if (mergedPaths.Count > 0)
        {
            //DisplayResultImage(mergedPaths[currentResultIndex]);
            //StartCoroutine(LoadAndDisplayResult(mergedPaths[currentResultIndex]));
            imagePathToPrint = mergedPaths[currentResultIndex];

            if (btnPrint != null)
                btnPrint.gameObject.SetActive(true);

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN

            //Disable Print
            //if (autoPrint)
            //{
            //    TryPrintFirstResult(mergedPaths[0]);
            //}
#endif
        }
    }

    public void OnStartGame()
    {
        keytboardGo.SetActive(false);
        SetState(AppState.Registration);
    }
   
    public void OnPromptSelect()
    {
        SetState(AppState.Camera);
    }

    public void OnUserCountStart()
    {
        DisclaimerPage_Panel(true);
        keytboardGo.SetActive(false);
        totalUsers = selectedUserCount;
        currentUserIndex = 0;
        registeredUsers.Clear();
        SetState(AppState.Registration);
    }

    public void DisclaimerPage_Panel(bool active)
    {
        panelRegistration_Disclaimer.SetActive(active);
    }

    public void RetakeOnFail_Panel(bool active)
    {
        panelCamera_RetakeOnFail.SetActive(active);
    }

    public void OnRegisterNext()
    {
        if (string.IsNullOrEmpty(nameInput.text)) return;
        if (string.IsNullOrEmpty(emailInput.text)) return;
        if (string.IsNullOrEmpty(contactInput.text)) return;

        if (!termsToggle.isOn) return;

        UserData newUser = new UserData
        {
            name = nameInput.text,
            contactNumber = contactInput.text,
            email = emailInput.text,
            gender = selectedGender
        };

        // save/update the current user's data first  
        if (currentUserIndex < registeredUsers.Count)
        {
            registeredUsers[currentUserIndex] = newUser;
        }
        else
        {
            registeredUsers.Add(newUser);
        }

        currentUserIndex++;
        UpdateUserNumberImage(currentUserIndex + 1);


        if (currentUserIndex < totalUsers)
        {

            if (currentUserIndex < registeredUsers.Count)
            {
                // Load existing next user data  
                var nextUser = registeredUsers[currentUserIndex];
                nameInput.text = nextUser.name;
                emailInput.text = nextUser.email;
                contactInput.text = nextUser.contactNumber;
                selectedGender = nextUser.gender;
                UpdateGenderButtons();
                if (termsToggle != null) termsToggle.isOn = true; // They already accepted  
            }
            else
            {
                // Clear form for new user  
                nameInput.text = "";
                emailInput.text = "";
                contactInput.text = "";
                selectedGender = "male";
                UpdateGenderButtons();
                if (termsToggle != null) termsToggle.isOn = false;
            }
        }
        else
        {
            // All users registered, go to camera  
            currentUserIndex = 0;
            // Update_ShowPhotoUserCount(); // Fix typo if this method exists  
            //SetState(AppState.Camera);
            SetState(AppState.PromptSelect);
        }
        Upadte_ShowPhotoUserCount();

        threetwooneImage.SetActive(false);
        keytboardGo.SetActive(false);


    }

    public void OnRegisterBack()
    {
        if (currentUserIndex > 0)
        {
            // Save current user data before going back  
            if (!string.IsNullOrEmpty(nameInput.text))
            {
                UserData currentUser = new UserData
                {
                    name = nameInput.text,
                    email = emailInput.text,
                    gender = selectedGender
                };

                if (currentUserIndex < registeredUsers.Count)
                {
                    registeredUsers[currentUserIndex] = currentUser;
                }
                else
                {
                    registeredUsers.Add(currentUser);
                }
            }


            currentUserIndex--;
            UpdateUserNumberImage(currentUserIndex + 1);

            var prevUser = registeredUsers[currentUserIndex];
            nameInput.text = prevUser.name;
            contactInput.text = prevUser.contactNumber;
            emailInput.text = prevUser.email;
            selectedGender = prevUser.gender;

            UpdateGenderButtons();
            termsToggle.isOn = true;

        }
        else
        {
            registeredUsers.Clear();
            currentUserIndex = 0;
            SetState(AppState.UserCount);
        }

        threetwooneImage.SetActive(false);
        keytboardGo.SetActive(false);
    }

    public void OnTakePhoto()
    {
        if (photoManager != null && currentUserIndex < registeredUsers.Count)
        {
            btnTakePhoto.interactable = false;

            threetwooneImage.SetActive(true);
            StartCoroutine(TimeGapForPhoto(registeredUsers[currentUserIndex].name));
            // photoManager.TakePhoto(registeredUsers[currentUserIndex].name);
        }
    }

    IEnumerator TimeGapForPhoto(string userName_)
    {
        yield return new WaitForSeconds(3);
        threetwooneImage.SetActive(false);
        keytboardGo.SetActive(false);
        photoManager.TakePhoto(userName_);
    }

    public void OnRetake()
    {
        threetwooneImage.SetActive(false);
        SetState(AppState.Camera);
    }

    public void OnBackPhotoPage()
    {

        if (currentUserIndex > 0)
        {
            currentUserIndex--;
            SetState(AppState.Camera);
        }
        else
        {
            currentUserIndex = totalUsers - 1;
            UpdateUserNumberImage(currentUserIndex + 1);
            SetState(AppState.Registration);

            var prevUser = registeredUsers[currentUserIndex];
            nameInput.text = prevUser.name;
            emailInput.text = prevUser.email;
            contactInput.text = prevUser.contactNumber;
            selectedGender = prevUser.gender;

            UpdateGenderButtons();
            termsToggle.isOn = true;
        }
        Upadte_ShowPhotoUserCount();
        threetwooneImage.SetActive(false);
        keytboardGo.SetActive(false);
    }

    public void OnContinue()
    {
        currentUserIndex++;
        if (currentUserIndex < registeredUsers.Count)
        {
            SetState(AppState.Camera);
        }
        else
        {
            SetState(AppState.Processing);
            StartCoroutine(ProcessPhotosCoroutine());
        }
        Upadte_ShowPhotoUserCount();
        threetwooneImage.SetActive(false);
        keytboardGo.SetActive(false);
    }

    public void Upadte_ShowPhotoUserCount()
    {
        string currUser = "USER " + (currentUserIndex + 1);
        photoUserCountText.text = currUser;
        photoUserCountText_OnReviewPage.text = currUser;
    }

    public void OnReset()
    {
        registeredUsers.Clear();
        resultImagePaths.Clear();
        currentUserIndex = 0;
        currentResultIndex = 0;
        lastCapturedPhoto = null;

        nameInput.text = "";
        if (emailInput != null) emailInput.text = "";
        selectedUserCount = 1;
        contactInput.text = "";
        selectedGender = "male";
        termsToggle.isOn = false;

        threetwooneImage.SetActive(false);
        keytboardGo.SetActive(false);

        btnPrint.interactable = true;

        UpdateUserCountButtons();
        UpdateGenderButtons();

        SetState(AppState.Welcome);
    }

    void OnPhotoTaken(string photoPath, Texture2D photoTexture)
    {
        if (currentUserIndex < registeredUsers.Count)
        {
            registeredUsers[currentUserIndex].photoPath = photoPath;
            lastCapturedPhoto = photoTexture;

            if (reviewImage != null)
                reviewImage.texture = lastCapturedPhoto;

            btnTakePhoto.interactable = true;
            SetState(AppState.Review);
        }
    }

    IEnumerator ProcessPhotosCoroutine()
    {
        int prompt_selected = -1;
        prompt_selected = imageSelectionPromptHandler.SelectedIndex;

        if (prompt_selected == -1)
        {
            Debug.LogError($"Prompt not Selected");
            prompt_selected = 0;
        }
        //prompt_selected = imageSelectionPromptHandler.selectedIndex;

        ProcessRequest request = new ProcessRequest
        {
            session_id = sessionID,
            users = new List<ProcessUser>(),
            prompt_index = prompt_selected,
            preferred_sequence = GetPreferredSequenceFromUsers() // optional: remove if you want backend to infer only
        };

        foreach (var user in registeredUsers)
        {
            request.users.Add(new ProcessUser
            {
                name = user.name,
                email = user.email,
                gender = user.gender,
                photo_path = user.photoPath
            });
        }

        string jsonData = JsonUtility.ToJson(request);

        using (UnityWebRequest webRequest = new UnityWebRequest($"{pythonServerURL}/process", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                ProcessResponse response = JsonUtility.FromJson<ProcessResponse>(webRequest.downloadHandler.text);

                if (response.success)
                {
                    string imagePath = response.output_paths[0];
                    OnPhotoProcessed?.Invoke(imagePath);
                    resultImagePaths = response.output_paths;
                    currentResultIndex = 0;
                    SetState(AppState.Final);
                }
                else
                {
                    Debug.LogWarning("Processing failed: " + response.message);
                    currentUserIndex = 0;
                    Upadte_ShowPhotoUserCount();
                    SetState(AppState.Camera);
                    RetakeOnFail_Panel(true);
                }
            }
            else
            {
                Debug.LogWarning("Network error: " + webRequest.error);
                currentUserIndex = 0;
                Upadte_ShowPhotoUserCount();
                SetState(AppState.Camera);
                RetakeOnFail_Panel(true);   //TO REMOVE
            }
        }
    }

    IEnumerator LoadAndDisplayResult(string imagePath)
    {
        if (File.Exists(imagePath))
        {
            byte[] imageData = File.ReadAllBytes(imagePath);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imageData);
            finalResultImage.texture = texture;
        }
        else
        {
            finalResultImage.texture = null;
        }
        yield return null;
    }

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
    void TryPrintFirstResult(string imagePath)
    {
        Debug.LogWarning("Attemted to print image: " + imagePath);
        //try
        //{
        //    PrintHelper.PrintImage(imagePath, printerName, printScaleMode, landscape: printLandscape);
        //}
        //catch (Exception ex)
        //{
        //    Debug.LogError("Printing failed: " + ex.Message);
        //}
    }
#endif

    [Serializable]
    public class PrintRequest { public string image_path; public string printer_name; public string scale_mode; public bool landscape; }
    [Serializable]
    public class PrintResponse { public bool success; public string message; }

    public void OnPrint()
    {
        StartCoroutine(SendPrintRequest(imagePathToPrint));

    }

    IEnumerator SendPrintRequest(string imagePath)
    {
        if (string.IsNullOrEmpty(imagePath))
        {
            Debug.LogWarning("No image to print (imagePathToPrint is empty).");
            yield break;
        }
        btnPrint.interactable = false;

        var payload = new PrintRequest
        {
            image_path = imagePath,
            printer_name = printerName,
            scale_mode = printScaleMode,
            landscape = printLandscape
        };

        string json = JsonUtility.ToJson(payload);
        using (var req = new UnityEngine.Networking.UnityWebRequest($"{pythonServerURL}/print", "POST"))
        {
            req.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                var resp = JsonUtility.FromJson<PrintResponse>(req.downloadHandler.text);
                Debug.Log(resp.success ? $"Print OK: {resp.message}" : $"Print failed: {resp.message}");
            }
            else
            {
                Debug.LogError($"Print request error: {req.error}");
            }
        }
        Invoke("ActivePrintButton", 20f);
        // Invoke(nameof(ActivePrintButton), 6f);
    }

    void ActivePrintButton()
    {
        btnPrint.interactable = true;
    }

    public void OnPrintLocal() //using Paint , default, but has bordered issue
    {
        try
        {
            PrintHelper.PrintImage(imagePathToPrint, printerName, printScaleMode, printLandscape);
            Debug.Log("Local print sent.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Local print failed: {ex.Message}");
        }
    }


    /// /////////////////////////////////////////////////////////////////

    private string GetPreferredSequenceFromUsers()
    {
        List<string> parts = new List<string>();
        foreach (var u in registeredUsers) parts.Add(u.gender.ToLower());
        return string.Join("_", parts);
    }

    [Serializable]
    public class ProcessRequest
    {
        public string session_id;
        public List<ProcessUser> users;
        public int prompt_index;
        public string preferred_sequence; // optional
    }


    [Serializable]
    public class ProcessUser
    {
        public string name;
        public string email;
        public string gender;
        public string photo_path;
    }

    [Serializable]
    public class ProcessResponse
    {
        public bool success;
        public string message;
        public List<string> output_paths;
    }


}