using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class PhotoGridManager : MonoBehaviour
{

    [Header("Sprite List")]
    [SerializeField] List<Sprite> m_spriteList;
    [SerializeField] Queue<Sprite> m_spriteQueue;

    [Header("Pool")]
    [SerializeField] List<GameObject> targetPool;
    [SerializeField] Transform m_startingSpawnPoint;
    [SerializeField] Transform m_targetParent;
    [Space]
    [Header("Folder")]
    [Tooltip("folder found in streaming assets")]
    public bool reverseSort = false;
    [SerializeField] List<string> m_imagePathList;
    [SerializeField] string photosFolderPath = "\\PhotosTaken";
    [SerializeField] string[] fileTypes = { ".jpg", ".png" };
    [Header("Slideshow")]
    [SerializeField] GameObject targetPrefab; // Assign your TargetShape prefab here
    [SerializeField] int poolSize = 10; // Number of objects in the pool
    [Space]
    [Header("Spawn")]
    [Tooltip("totalImageLoad to repeat")]
    [SerializeField] int imageLoadMax = 15;
    [SerializeField] int spawnRowMax = 5;
    [SerializeField] int spawnColumnMax = 5;
    [SerializeField] float spawnInterval = 2f; // Time in seconds between spawns
    [Space]
    [SerializeField] Vector2 m_spawnAreaMin;
    [SerializeField] Vector2 m_spawnAreaMax;
    [Range(0, 1)]
    [SerializeField] float m_gridSpacingX = 0.5f;
    [Range(0, 1)]
    [SerializeField] float m_gridSpacingY = 0.5f;
    [Header("Debug")]

    [SerializeField] Color spawnAreaColor = Color.green;
    [SerializeField] Color spawnLocationColor = Color.green;
    public bool isLoadingImages = false;

    #region Getters

    public GameObject[] GetPhotoList => targetPool.ToArray();

    #region Sprite List Functions



    #endregion

    #endregion

    #region LifeCycles

    async void Start()
    {
        imageLoadMax = ConfigManager.GetInstance().GetValue("IMAGE_LOAD_MAX");
        reverseSort = ConfigManager.GetInstance().GetBool("REVERSE_IMAGE_SORT");
        m_spriteQueue = new Queue<Sprite>();


        isLoadingImages = false;
        poolSize = spawnRowMax * spawnColumnMax;
        FindImageFiles(Application.streamingAssetsPath + photosFolderPath);
        await PoolObjects();
        await LoadPhotoSprites();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            ReloadPhotos();
        }
        if (Input.GetKeyDown(KeyCode.F6))
        {
            reverseSort = !reverseSort;
        }
    }

    public async void ReloadPhotos()
    {
        await LoadPhotoSprites();
    }

    async Task LoadPhotoSprites()
    {
        await StartLoadingPhotos();

    }

    #endregion

    #region ImagePool
    private async Task PoolObjects()
    {

        targetPool = new List<GameObject>();

        for (int targetIndex = 0; targetIndex < poolSize; targetIndex++)
        {
            GameObject targetObj = Instantiate(targetPrefab, m_targetParent);

            int currentRow = targetIndex / spawnColumnMax;
            int currentColumn = targetIndex % spawnColumnMax;
            var squareSize = m_startingSpawnPoint.GetComponent<Renderer>().bounds.size;

            Vector3 position = new Vector3(
                m_startingSpawnPoint.transform.position.x + currentRow * (squareSize.x + m_gridSpacingX),
                m_startingSpawnPoint.transform.position.y + currentColumn * (squareSize.y + m_gridSpacingY),
                m_startingSpawnPoint.transform.position.z // Keep Z at 0 for 2D
            );
            targetObj.name += $" {targetIndex}";
            targetObj.transform.position = position;
            targetObj.transform.localScale = m_startingSpawnPoint.localScale;

            var targetPhoto = targetObj.GetComponent<TargetPhoto>();
            targetPhoto.SetImagePath(GetImagePath(targetIndex));
            targetPhoto.SetImagePath(GetImagePath(targetIndex));

            targetPool.Add(targetObj);

        }

        await Task.Yield();
    }

    #endregion

    #region Find Images

    public async Task StartLoadingPhotos()
    {
        isLoadingImages = true;
        await LoadPhotosSequentially(targetPool);
        isLoadingImages = false;
    }

    public async Task LoadPhotosSequentially(List<GameObject> p_imgTargetPhotoList)
    {
        Sprite[] spriteLoadedArr = new Sprite[imageLoadMax];
        //LOAD SPRITES 
        for (int spritePathIndex = 0; spritePathIndex < spriteLoadedArr.Length; spritePathIndex++)
        {
            //Sprite loadedSprite;
            spriteLoadedArr[spritePathIndex] = await ImageLoader.LoadSpriteFromBytesAsync(m_imagePathList[spritePathIndex % m_imagePathList.Count]);
            await Task.Yield();
        }

        ///////////////////////////////////////////////////////
        if (reverseSort)
        {
            p_imgTargetPhotoList.Reverse();
        }
        //APPLY SPRITES
        //if outside load atleast 30-35
        //foreach photo apply the sprites depending on the number of image repeated. 
        for (int targetPhotoObj = 0; targetPhotoObj < p_imgTargetPhotoList.Count; targetPhotoObj++)
        {
            var targetPhotoSelected = p_imgTargetPhotoList[targetPhotoObj].GetComponent<TargetPhoto>();
            targetPhotoSelected.SetSprite(spriteLoadedArr[targetPhotoObj % imageLoadMax]);
            await Task.Yield();
        }

        Debug.LogAssertion($"Images loaded: Done");
    }

    public string GetImagePath(int p_index)
    {
        var index = p_index % m_imagePathList.Count;
        return m_imagePathList[index];
    }

    public string[] FindImageFiles(string p_folderPath)
    {
        m_imagePathList = new List<string>();
        var di = new DirectoryInfo(p_folderPath);
        var list = new List<string>();
        foreach (var fileType in fileTypes)
        {
            foreach (var file in di.GetFiles($"*{fileType}", SearchOption.AllDirectories))
            {
                m_imagePathList.Add(file.ToString());
            }
        }

        return list.ToArray();
    }

    #endregion

    private void SpawnTarget()
    {
        GameObject targetObj = GetPooledObject();

        if (targetObj != null)
        {
            //Vector2 randomPosition = new Vector2(
            //    Random.Range(spawnAreaMin.x, spawnAreaMax.x),
            //    Random.Range(spawnAreaMin.y, spawnAreaMax.y)
            //);

            int randomRow = Random.Range(0, spawnRowMax);
            int randomColumn = Random.Range(0, spawnColumnMax);
            var squareSize = m_startingSpawnPoint.GetComponent<Renderer>().bounds.size;

            Vector3 position = new Vector3(
               m_startingSpawnPoint.transform.position.x + randomRow * (squareSize.x + m_gridSpacingX),
               m_startingSpawnPoint.transform.position.y + randomColumn * (squareSize.y + m_gridSpacingY),
                m_startingSpawnPoint.transform.position.z // Keep Z at 0 for 2D
            );

            // Activate and place the target at the random position
            targetObj.transform.position = position;
            targetObj.SetActive(true);
        }
    }

    private GameObject GetPooledObject()
    {
        foreach (GameObject obj in targetPool)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }
        return null; // No inactive objects available
    }


    #region Debug
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = spawnLocationColor;
        var squareSize = m_startingSpawnPoint.GetComponent<Renderer>().bounds.size;

        for (int rowIndex = 0; rowIndex < spawnRowMax; rowIndex++)
        {
            for (int colIndex = 0; colIndex < spawnColumnMax; colIndex++)
            {
                Vector3 position = new Vector3(
                    m_startingSpawnPoint.transform.position.x + rowIndex * (squareSize.x + m_gridSpacingX),
                    m_startingSpawnPoint.transform.position.y + colIndex * (squareSize.y + m_gridSpacingY),
                m_startingSpawnPoint.transform.position.z // Keep Z at 0 for 2D
                );

                Gizmos.DrawWireCube(position, squareSize);
            }
        }
    }

#endif

    private void OnGUI()
    {
        if (DebugBehaviour.debugModeOn)
        {
            if (isLoadingImages)
            {
                GUI.color = Color.white;
                GUI.TextArea(new Rect(Screen.width / 2, Screen.height - 100, 100, 50), "Loading Images..");
            }
            GUI.TextArea(new Rect(Screen.width / 2, Screen.height - 200, 100, 50), $"Image sorting Reversed: {reverseSort}");

        }
    }
    #endregion

}