using System;
using System.IO;
using System.Collections;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public enum IMAGE_TARGET
{
    SPHERE,
    GRID,
    NONE
}

public class TargetPhoto : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] SpriteRenderer imgSprite;
    [SerializeField] SpriteRenderer imgGradient;
    [Space]
    [Header("Photo ")]
    [SerializeField] private string imgPath;

    [Header("Photo Movement")]
    [SerializeField] Vector3 targetTransform;
    [Space]
    [SerializeField] Transform spherePosition;
    [SerializeField] Vector3 gridPosition;
    [SerializeField] Vector3 originalPosition;
    [SerializeField] float moveSpeed = 2.0f;
    [Space]
    [Space]
    [SerializeField] bool isMoving = false;
    [SerializeField] bool isGoingToSphere = false;
    [Space]
    [Header("Fade")]
    [Range(0, 1)]
    [SerializeField] float targetOpacity = 1;
    [SerializeField] float fadeDuration = 1;
    [SerializeField] bool isFading = false;
    [Space]

    [Header("Debug")]
    public Vector3 textOffset = Vector3.up;
    public FontStyle fontStyle = FontStyle.Bold;
    public int fontSize = 18;
    public Color textColor = Color.white;

    #region Getters

    public Vector3 GridPosition => gridPosition;

    #endregion

    private void Start()
    {
        imgSprite = GetComponent<SpriteRenderer>();
        init();
        //LoadTextureAsSprite(imgPath);
        //LoadPhoto();
    }

    void Update()
    {
        if (isMoving)
        {
            MoveToTarget();
        }
    }
    
    void init()
    {
        targetOpacity = UnityEngine.Random.Range(0.5f, 1f);
        originalPosition = transform.position;

        gridPosition = transform.position;
        isMoving = false;
        isFading = false;

    }

    public void SetImageOpacity(float p_imgOpacity)
    {
        var material = GetComponent<SpriteRenderer>().material;
        var materialGradient = imgGradient.GetComponent<SpriteRenderer>().material;
        material.color = new Color(1f, 1f, 1f, p_imgOpacity);
        materialGradient.color = new Color(1f, 1f, 1f, p_imgOpacity);
    }

    public void EnableGradient(bool p_enable)
    {
        imgGradient.gameObject.SetActive(p_enable);
    }

    public void FadePhotoImage(bool p_fadeIn = true)
    {
        if (!isFading)
        {
            if (p_fadeIn)
            {
                StartCoroutine(FadeSprite(0, targetOpacity));
                //Debug.Log("Faded In");
            }
            else
            {
                StartCoroutine(FadeSprite(targetOpacity, 0));
                //Debug.Log("Faded Out");
            }
        }
    }

    public async Task FadePhotoImage(bool p_fadeIn = true, float p_duration = 1f)
    {
        if (!isFading)
        {
            if (p_fadeIn)
            {
                StartCoroutine(FadeSprite(0, targetOpacity, p_duration));
                Debug.Log("Faded In");
            }
            else
            {
                StartCoroutine(FadeSprite(targetOpacity, 0, p_duration));
                Debug.Log("Faded Out");
            }
            await Task.Yield();
        }
    }

    public void ResetPhotoPosition(IMAGE_TARGET p_targetPosition)
    {
        switch (p_targetPosition)
        {

            case IMAGE_TARGET.SPHERE:
                //transform.position.
                break;
            case IMAGE_TARGET.GRID:
                transform.position = gridPosition;
                break;
            case IMAGE_TARGET.NONE:
                Debug.Log($"Target Set to None");
                break;
            default:
                break;
        }
    }

    public void ResetToGrid()
    {
        transform.position = gridPosition;
    }

    #region Loading Photo

    public async void LoadPhoto()
    {
        //await Task.Delay(TimeSpan.FromSeconds(0.1f));
        await LoadingPhoto();
    }

    public async Task LoadingPhoto()
    {
        if (imgSprite == null)
        {
            imgSprite = GetComponent<SpriteRenderer>();

        }
        imgSprite.sprite = ImageLoader.LoadSprite(imgPath);
        await Task.Yield();
        //var img = ImageLoader.LoadTextureResized(imgPath);
        //sprite.sprite = ImageLoader.ConvertTextureToSprite(img);
    }

    private void LoadTextureAsSprite(string path)
    {
        // Ensure file exists at the specified path
        if (File.Exists(path))
        {
            byte[] fileData = File.ReadAllBytes(path); // Read the file data into byte array
            Texture2D texture = new Texture2D(2, 2); // Create a new Texture2D
            texture.LoadImage(fileData); // Load image data into the texture

            // Create a Sprite from the Texture2D
            Sprite createdSprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f)
            );

            // Assign the created Sprite to the SpriteRenderer
            imgSprite.sprite = createdSprite;
        }
        else
        {
            Debug.LogError($"Image file not found at path: {path}");
        }
    }
    #endregion

    #region Setters
    public void SetSprite(Sprite p_sprite)
    {
        if (imgSprite == null)
        {
            imgSprite = GetComponent<SpriteRenderer>();
        }
        imgSprite.sprite = p_sprite;
    }

    public void SetImagePath(string p_path)
    {
        imgPath = p_path;
    }

    public void SetGridPosition(Vector3 p_position)
    {
        gridPosition = p_position;
    }


    public void SetMask(SpriteMaskInteraction p_maskSetting)
    {
        if (imgSprite == null)
            imgSprite = GetComponent<SpriteRenderer>();

        imgSprite.maskInteraction = p_maskSetting;
    }

    public void SetToSphereVertex(Transform p_transform, bool p_startMoving = true)
    {
        isGoingToSphere = true;
        StartMoving();
        spherePosition = p_transform;
    }

    public void SetFollowTarget(Transform p_transform, bool p_startMoving = true)
    {
        StopMoving();
        targetTransform = gridPosition;
        isMoving = p_startMoving;
    }
    #endregion

    #region Move Target

    public void MoveToTarget()
    {
        if (isGoingToSphere)
            targetTransform = spherePosition.position;
        else
            targetTransform = gridPosition;
        transform.position = Vector3.Lerp(transform.position, targetTransform, moveSpeed * Time.deltaTime);
        //if (Vector3.Distance(transform.position, targetTransform.position) < 0.1f)
        //{
        //    isMoving = false; // Stop moving once close to the target
        //}
    }

    public void StartMoving()
    {
        isMoving = true;
    }

    public void StopMoving()
    {
        isMoving = false;
    }

    public void ReturnToGrid()
    {
        targetTransform = gridPosition;
        isMoving = true;
        isGoingToSphere = false;
    }

    #endregion

    // Duration of the fade 
    // Coroutine to fade the sprite

    public IEnumerator FadeSprite(float startAlpha, float targetAlpha)
    {
        isFading = true;
        //Debug.Log($"Fade Started");
        float elapsedTime = 0f;
        var material = GetComponent<SpriteRenderer>().material;
        var gradientMaterial = imgGradient.GetComponent<SpriteRenderer>().material;

        while (elapsedTime < fadeDuration)
        {
            var t = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime);
            material.color = new Color(1f, 1f, 1f, t);
            gradientMaterial.color = new Color(1f, 1f, 1f, t);
            elapsedTime += Time.deltaTime;

            //Debug.Log($"Fade Alpha {alpha}");
            yield return null;

        }

        // Ensure the final alpha is set to 1 after the fade-in is complete
        material.color = new Color(1f, 1f, 1f, targetAlpha);
        gradientMaterial.color = new Color(1f, 1f, 1f, targetAlpha);

        isFading = false;
    }

    public IEnumerator FadeSprite(float startAlpha, float targetAlpha, float p_duration)
    {
        isFading = true;
        Debug.Log($"Fade Started");
        float elapsedTime = p_duration;
        var material = GetComponent<SpriteRenderer>().material;
        var gradientMaterial = imgGradient.GetComponent<SpriteRenderer>().material;

        while (elapsedTime > 0)
        {
            var t = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime);
            material.color = new Color(1f, 1f, 1f, t);
            gradientMaterial.color = new Color(1f, 1f, 1f, t);
            elapsedTime -= Time.deltaTime;
            //Debug.Log($"Fade Alpha {alpha}");
            yield return null;
        }

        // Ensure the final alpha is set to 1 after the fade-in is complete
        material.color = new Color(1f, 1f, 1f, targetAlpha);
        gradientMaterial.color = new Color(1f, 1f, 1f, targetAlpha);

        isFading = false;

        Debug.Log($"Fade Done");
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, targetTransform);
        //Gizmos.DrawIcon(targetTransform, "target",true);
        Handles.Label(targetTransform, "Target");
    }

    private void OnDrawGizmos()
    {
        var style = new GUIStyle();
        style.fontStyle = fontStyle;
        style.fontSize = fontSize;
        style.normal.textColor = textColor;

        Handles.Label(transform.position + textOffset, $"Index: {transform.GetSiblingIndex()}", style);
    }

#endif

}