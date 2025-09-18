using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class FloatingTopic : MonoBehaviour
{
    [Header("Req")]
    [SerializeField] ImageScaler m_imageScaler;
    [SerializeField] Transform targetVertex;
    [SerializeField] Image img;
    [Space]
    [Header("Vertex Position")]
    [SerializeField] Vector3 offset = Vector3.zero;
    [SerializeField] int vertexIndex;
    [Space]
    [Header("Selection")]
    [SerializeField] bool isSelected = false;

    [SerializeField] Sprite m_selectedSprite;
    [SerializeField] Sprite m_unSelectedSprite;

    [SerializeField] Color selectedColor = Color.cyan;
    [SerializeField] Color unSelectedColor = Color.white;
    bool isTimerOn = false;

    public float selectedDuration
    {
        get { return _selectedDurationMs; }
        set
        {
            _selectedDurationMs = value;
            Debug.Log($"_selectedDurationMs changed to : {_selectedDurationMs}");
        }
    }
    static int currentSelected = -1;
    static float _selectedDurationMs = 5;
    static bool isAnimating;
    
    public static Action<int> onSelected;

    #region LifeCycles

    void Start()
    {
        isAnimating = false; 
        vertexIndex = transform.GetSiblingIndex();
        img = GetComponentInChildren<Image>();
        m_selectedSprite = img.sprite;

        isSelected = false;
        _selectedDurationMs = ConfigManager.GetInstance().GetFloatValue("SELECTED_BUTTON_DURATION");
      
        onSelected -= OnSelected;
        onSelected += OnSelected;
    }

    void Update()
    {
        FollowVertex();
    }

    #endregion
    
    #region Setters

    public void SetUnselectedSprite(Sprite p_sprite)
    {
        m_unSelectedSprite = p_sprite;
    }
    #endregion

    #region UI Follow

    public void FollowVertex()
    {
        this.transform.position = targetVertex.position + offset;
    }

    public void SetVertexToFollow(GameObject p_vertexObj)
    {
        targetVertex = p_vertexObj.transform;
    }
    #endregion

    #region  Selection

    public void SetSelected()
    {
        onSelected?.Invoke(vertexIndex);
        m_imageScaler.ScaleImage(_selectedDurationMs);
        //img.color = selectedColor;
        img.sprite = m_unSelectedSprite;
        //NOTE: For some reason Selected Sprite is reversed when selecting
        isSelected = true;
        isAnimating = true; 
        currentSelected = vertexIndex;
        EnableTimer();
    }

    public void EnableTimer()
    {
        if (isTimerOn) return;
        StopCoroutine(StartTimer());
        StartCoroutine(StartTimer());
    }

    IEnumerator StartTimer()
    {
        isTimerOn = true;
        yield return new WaitForSeconds(_selectedDurationMs);
        UnSelect();
        //SphereButtonManager.Instance.EnableSphereRotation(true);
        isTimerOn = false;
        isAnimating = false;
    }

    [ContextMenu("Unselect")]
    public void UnSelect()
    {
        m_imageScaler.ReturnToOriginal();
        //img.color = unSelectedColor;
        img.sprite = m_selectedSprite;
        isSelected = false;
        currentSelected = -1;
    }

    private void OnSelected(int p_selectedIndex)
    {
        //Debug.Log($"{gameObject.name} unselected");
        //if (currentSelected == vertexIndex)
        //    return;

        UnSelect();
        //m_imageScaler.ScaleImage(0.2f, selectedDuration);

    }
    #endregion

    #region Debug

    //#if UNITY_EDITOR
    //    private void OnDrawGizmosSelected()
    //    {
    //        Handles.Label(this.transform.position, $"current selected {currentSelected}");
    //    }
    //#endif


    #endregion

}