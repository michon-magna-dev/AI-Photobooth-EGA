using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class MeshFace
{
    public string name;
    public GameObject obj;

    public MeshFace(string p_name, GameObject p_obj)
    {
        name = p_name;
        obj = p_obj;
    }
}


[ExecuteInEditMode]

public class FaceGenManager : MonoBehaviour
{
    private static FaceGenManager _instance;
    public static FaceGenManager GetInstance => _instance;

    [SerializeField] Dictionary<string, GameObject> m_faceMeshObjDictionary = new Dictionary<string, GameObject>(); // Change the collection type

    [Header("Triangle Params")]
    [SerializeField] int m_faceLimit = 100;
    [SerializeField] int m_currentCount = 0;
    [SerializeField] float m_triangleAreaLimit = 10f;

    public Action OnFaceCreated;
    public Action<GameObject> OnFaceRemove;
    public Action<GameObject> OnFaceDestroyed;
    public bool m_enableDetection = false;
    #region Getters
    public float GetTriangleAreaLimit => m_triangleAreaLimit;

    public bool IsLimitReached() => m_faceMeshObjDictionary.Count >= m_faceLimit;
    #endregion

    #region LifeCycle

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            m_enableDetection = !m_enableDetection;
        }
        m_currentCount = m_faceMeshObjDictionary.Count;
    }

    void Start()
    {
        m_faceMeshObjDictionary = new Dictionary<string, GameObject>(); // Initialize the dictionary
        OnFaceRemove += OnFaceMeshDestroyed;
    }

    #endregion

    public bool AddTriangleMesh(GameObject p_triangleMeshObj)
    {
        if (m_faceMeshObjDictionary.Count >= m_faceLimit)
        {
            return true;
        }
        TriangleMesh triangleMesh = p_triangleMeshObj.GetComponent<TriangleMesh>();
        if (!IsDuplicate(p_triangleMeshObj))
        {
            m_faceMeshObjDictionary[p_triangleMeshObj.name] = p_triangleMeshObj; // Use GameObject name as the key
            return false; // not duplicate
        }
        else
        {
            return true; // has duplicate
        }
    }

    private void OnFaceMeshDestroyed(GameObject p_faceObj)
    {
        RemoveFaceFromDictionary(p_faceObj);
    }

    public void RemoveFaceFromDictionary(GameObject p_faceObj)
    {
        if (p_faceObj != null && m_faceMeshObjDictionary.ContainsKey(p_faceObj.name))
        {
            var faceName = p_faceObj.name;
            m_faceMeshObjDictionary.Remove(faceName);
            //Destroy(p_faceObj);
        }
    }

    private bool IsDuplicate(GameObject newFace)
    {
        return m_faceMeshObjDictionary.ContainsKey(newFace.name); // Check if the dictionary already contains the key
    }
}