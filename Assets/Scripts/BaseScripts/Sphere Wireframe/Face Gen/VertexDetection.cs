using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class VertexDetection : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] List<Vector3> m_nearbyVertices;
    [SerializeField] List<GameObject> m_nearbyObjects;
    [SerializeField] List<GameObject> m_createdTriangles = new List<GameObject>();
    //HashSet<GameObject> createdTrianglesV2 = new HashSet<GameObject>();

    [Space]
    [SerializeField] float m_detectionRadius = 5f;
    [SerializeField] string m_targetTag = $"Vertex";

    [Header("Face Mesh")]
    static GameObject triangleMeshParentObj;
    [SerializeField] GameObject m_triangleMeshPrefab;
    [SerializeField] Material triangleMaterial;

    [Tooltip("Number of triangles this point can make with others")]
    [SerializeField] int m_availableTriangles = 0;
    [SerializeField] int m_triangleCreated = 1;
    [SerializeField] int m_triangleLimit = 3;

    [Header("Debug")]
    public bool m_showDetectionWireSphere = true;
    public float lineThickness = 1f;
    #region LifeCycles
    private void Awake()
    {
        CreateParent();
        m_triangleCreated = 0;
    }


    void LateUpdate()
    {
        //if (m_triangleCreated >= m_triangleLimit || FaceGenManager.GetInstance.IsLimitReached())
        //{
        //    return;
        //}

        if (!FaceGenManager.GetInstance.m_enableDetection)
        {
            return;
        }
        DetectNearbyPoints();
        TryCreateFaceMesh();
    }

    #endregion

    #region Detection
    private void DetectNearbyPoints()
    {
        m_nearbyVertices ??= new List<Vector3>();
        m_nearbyObjects ??= new List<GameObject>();

        m_nearbyVertices.Clear();
        m_nearbyObjects.Clear();

        Collider[] colliders = Physics.OverlapSphere(transform.position, m_detectionRadius);
        foreach (var item in colliders)
        {
            if (item.tag == m_targetTag)
            {
                m_nearbyVertices.Add(item.gameObject.transform.position);
                m_nearbyObjects.Add(item.gameObject);
            }
        }
    }

    #endregion

    #region Triangle Functions 

    private void TryCreateFaceMesh()
    {
        if (m_nearbyVertices.Count >= 3)
        {
            m_availableTriangles = m_nearbyVertices.Count / 3;
            for (int verticeIndex = 0; verticeIndex < m_nearbyVertices.Count - 2; verticeIndex += 3)
            {
                try
                {
                    //CreateTriangleMesh(m_nearbyVertices.Count / 3, verticeIndex);
                    CreateTriangleMesh(verticeIndex);
                }
                catch (Exception e)
                {
                    //Debug.LogException(e);
                }
            }
        }
        else
        {
            Debug.Log($"{gameObject.name} reset");
            //m_triangleCreated = 0;
        }
    }

    public void CreateTriangleMesh(int p_vertexIndex)
    {
        if (m_triangleCreated >= m_triangleLimit || FaceGenManager.GetInstance.IsLimitReached())
        {
            return;
        }

        GameObject p1 = m_nearbyObjects[p_vertexIndex];
        GameObject p2 = m_nearbyObjects[p_vertexIndex + 1];
        GameObject p3 = m_nearbyObjects[p_vertexIndex + 2];

        Vector3[] planeVertices = new Vector3[] { p1.transform.position, p2.transform.position, p3.transform.position };
        Vector3 normal = Vector3.Cross(p2.transform.position - p1.transform.position, p3.transform.position - p1.transform.position).normalized;

        GameObject planeObject = Instantiate(m_triangleMeshPrefab, triangleMeshParentObj.transform);
        planeObject.name = $"Triangle ({p1.transform.GetSiblingIndex()},{p2.transform.GetSiblingIndex()},{p3.transform.GetSiblingIndex()})";

        Mesh mesh = new Mesh();
        MeshFilter meshFilter = planeObject.GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = planeObject.GetComponent<MeshRenderer>();
        planeObject.GetComponent<TriangleMesh>()
            .SetPoints(new GameObject[] { p1, p2, p3 })
            .SetTriangleVertexPointOrigin(this);

        mesh.Clear();
        mesh.RecalculateBounds();
        mesh.vertices = planeVertices;

        int[] triangles = new int[] { 0, 1, 2 };
        mesh.triangles = triangles;

        meshFilter.sharedMesh = mesh;
        meshFilter.sharedMesh.normals = new Vector3[] { normal, normal, normal };
        meshRenderer.material = triangleMaterial;

        AddTriangleToList(planeObject, p1, p2, p3);
    }

    public void AddTriangleToList(GameObject p_meshObj, GameObject p1, GameObject p2, GameObject p3)
    {
        //if (FaceGenManager.GetInstance.AddTriangleMesh(p1, p2, p3))
        if (FaceGenManager.GetInstance.AddTriangleMesh(p_meshObj))
        {
            Destroy(p_meshObj);
        }
        else
        {
            AddToCollection(p_meshObj);
            m_triangleCreated++;
        }
    }

    #endregion

    #region Triangle Management
    public void AddToCollection(GameObject obj)
    {
        m_createdTriangles.Add(obj);
    }

    public void RemoveFromCollection(GameObject obj)
    {
        m_createdTriangles.Remove(obj);
    }

    public void DestroyAndRemove(GameObject obj)
    {
        if (m_createdTriangles.Contains(obj))
        {
            m_createdTriangles.Remove(obj);

        }
        if (obj != null)
        {
            var objMeshFilter = obj.GetComponent<MeshFilter>();
            objMeshFilter.sharedMesh.Clear();
            Destroy(objMeshFilter.sharedMesh);
            Destroy(obj);
        }
        try
        {
            Resources.UnloadUnusedAssets();
        }
        catch (Exception)
        {
        }
    }

    public void RemoveFaceFromList(GameObject p_faceObj)
    {
        if (p_faceObj != null && m_createdTriangles.Contains(p_faceObj))
        {
            //Debug.Log($"Removed {p_faceObj.name} from List");
            m_triangleCreated--;
            FaceGenManager.GetInstance.OnFaceRemove!.Invoke(p_faceObj);
            DestroyAndRemove(p_faceObj);
        }
    }
    #endregion

    #region Static Methods

    private static void CreateParent()
    {
        if (triangleMeshParentObj == null)
        {
            triangleMeshParentObj = new GameObject($"Triangle Mesh");
        }
    }

    #endregion

    //    #region Debug
    //#if UNITY_EDITOR

    //    private void OnDrawGizmosSelected()
    //    {
    //        DrawDetectionRadius();
    //        DrawNearbyLineVision();
    //    }

    //    void DrawDetectionRadius()
    //    {
    //        if (!m_showDetectionWireSphere)
    //            return;

    //        Gizmos.color = Color.yellow;
    //        Gizmos.DrawWireSphere(transform.position, m_detectionRadius);
    //    }

    //    void DrawNearbyLineVision()
    //    {
    //        for (int i = 0; i < m_nearbyVertices.Count; i++)
    //        {
    //            Handles.DrawLine(transform.position, m_nearbyVertices[i], lineThickness);
    //        }
    //    }

    //#endif

    //    #endregion
}