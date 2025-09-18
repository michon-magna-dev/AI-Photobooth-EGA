using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public class DebugMeshVertex
{
    public float vertexSize = 0.5f;
    public Color vertexColor = Color.white;
}

public enum SpawnSetting
{
    BOTH,
    SURFACE,
    OUTER
}

public class VertexPoints : MonoBehaviour
{
    [SerializeField] MeshFilter m_meshFilter;
    [Header("Visual")]
    [SerializeField] bool m_spawnVertexSpheres;
    [SerializeField] SpawnSetting m_spawnSetting = SpawnSetting.BOTH;
    [SerializeField] GameObject m_spherePrefab;

    [Header("3D Vertices")]
    [SerializeField] int vertCount =0;
    [SerializeField] Vector3[] m_surfaceVertices;
    [SerializeField] Vector3[] m_outsideVertices;
    [SerializeField] float m_outwardDistanceMultiplier = 1.0f;
    [Space]
    [Header("Angles")]
    private float _initialDistanceMultiplier;
    [SerializeField] bool m_enableOutwardAngleRange = false;
    [SerializeField] float[] m_outwardPointAngles;
    [Range(0, 180)]
    [SerializeField] float m_middleAngleRangeA = 45f;
    [Range(0, 180)]
    [SerializeField] float m_middleAngleRangeB = 180f;
    [SerializeField] float m_middleOutwardDistanceMultiplier = 1.5f;

    [Description("This is the 0 starting angle of the Circle. it should be in the horizontal to calculate anything in between the selected angle")]
    [SerializeField] Vector3 startingAngle = Vector3.forward;
    [Space]
    [Header("Debug")]
    [SerializeField] bool m_showInInspector = true;
    [SerializeField] DebugMeshVertex m_surfaceVertex = new DebugMeshVertex();
    [SerializeField] DebugMeshVertex m_outwardVertex = new DebugMeshVertex();

    private Transform surfaceObjsParent;
    private Transform outerObjsParent;

    public Transform[] GetPointTransforms(SpawnSetting p_spawnLocation = SpawnSetting.SURFACE)
    {
        var objParent = p_spawnLocation == SpawnSetting.SURFACE ? surfaceObjsParent:outerObjsParent;
        return objParent.GetComponentsInChildren<Transform>();
    }

    #region LifeCycle
    private void OnValidate()
    {
        m_meshFilter = GetComponent<MeshFilter>();

        if (m_meshFilter != null)
        {
            Mesh mesh = m_meshFilter.sharedMesh;
            Vector3[] localVertices = mesh.vertices;
            List<Vector3> worldVertices = new List<Vector3>();

            Transform transform = this.transform;

            for (int i = 0; i < localVertices.Length; i++)
            {
                Vector3 worldVertex = transform.TransformPoint(localVertices[i]);

                if (!worldVertices.Contains(worldVertex))
                    worldVertices.Add(worldVertex);
            }
            m_surfaceVertices = worldVertices.ToArray();
        }
        vertCount = m_surfaceVertices.Length;
    }

    private void Start()
    {
        _initialDistanceMultiplier = m_outwardDistanceMultiplier;
        GetSurfaceVertices();
        GetOutwardVertices();
        SpawnSpheres();
    }

    private void LateUpdate()
    {
        CalculateVertices();
    }

    #endregion

    private void CalculateVertices()
    {
        GetSurfaceVertices();

        if (m_enableOutwardAngleRange)
            GetOutwardVerticesWithAngle();
        else
            GetOutwardVertices();
    }

    #region Vertex Point Instantiation and Spawning

    [ContextMenu("Get Surface Vertices")]
    public void GetSurfaceVertices()
    {
        m_meshFilter = GetComponent<MeshFilter>();

        if (m_meshFilter != null)
        {
            Mesh mesh = m_meshFilter.sharedMesh;
            Vector3[] localVertices = mesh.vertices;
            List<Vector3> worldVertices = new List<Vector3>();

            Transform transform = this.transform;

            for (int i = 0; i < localVertices.Length; i++)
            {
                Vector3 worldVertex = transform.TransformPoint(localVertices[i]);

                if (!worldVertices.Contains(worldVertex))
                    worldVertices.Add(worldVertex);
            }
            m_surfaceVertices = worldVertices.ToArray();
        }
    }
    
    public void GetOutwardVertices()
    {
        List<Vector3> outwardVertices = new List<Vector3>();
        Transform transform = this.transform;

        for (int vertexIndex = 0; vertexIndex < m_surfaceVertices.Length; vertexIndex++)
        {
            var outwardOffset = (m_surfaceVertices[vertexIndex] - transform.position) * m_outwardDistanceMultiplier;
            var calculatedPoint = transform.position + outwardOffset;
            outwardVertices.Add(calculatedPoint);
        }
        m_outsideVertices = outwardVertices.ToArray();
    }

    public void GetOutwardVerticesWithAngle()
    {
        List<Vector3> outwardVertices = new List<Vector3>();
        List<float> outwardPointAngles = new List<float>();

        Transform transform = this.transform;

        for (int vertexIndex = 0; vertexIndex < m_surfaceVertices.Length; vertexIndex++)
        {
            var outwardOffset = (m_surfaceVertices[vertexIndex] - transform.position) * m_outwardDistanceMultiplier;
            var calculatedPoint = transform.position + outwardOffset;

            float pointAngle = GetVertexOutwardAngle(calculatedPoint, vertexIndex);
            if (pointAngle <= m_middleAngleRangeA && pointAngle >= m_middleAngleRangeB)
                calculatedPoint *= m_middleOutwardDistanceMultiplier;


            outwardVertices.Add(calculatedPoint);
            outwardPointAngles.Add(pointAngle);
        }
        m_outsideVertices = outwardVertices.ToArray();
        m_outwardPointAngles = outwardPointAngles.ToArray();
    }

    float GetVertexOutwardAngle(Vector3 p_target, int p_vertexIndex)
    {
        Vector3 vectorToTarget = p_target - transform.position;
        float angle = Vector3.Angle(startingAngle, vectorToTarget);
        //Debug.Log($"Vertex {p_vertexIndex} Angle to Center: {angle}");
        return angle;
    }

    public float GetVertexOutwardAngle(Vector3 p_target)
    {
        Vector3 vectorToTarget = p_target - transform.position;
        float angle = Vector3.Angle(startingAngle, vectorToTarget);
        return angle;
    }

    void SpawnSpheres()
    {
        switch (m_spawnSetting)
        {
            case SpawnSetting.BOTH:
                InstantiateSpheres(m_surfaceVertices, new GameObject("Surface"), true);
                InstantiateSpheres(m_outsideVertices, new GameObject("Outer"));
                break;
            case SpawnSetting.SURFACE:
                InstantiateSpheres(m_surfaceVertices, new GameObject("Surface"), true);
                break;
            case SpawnSetting.OUTER:
                InstantiateSpheres(m_outsideVertices, new GameObject("Outer"));
                break;
            default:
                break;
        }
    }

    void InstantiateSpheres(Vector3[] vertices, GameObject p_parent, bool p_isSurfaceSpheres = false)
    {
        if (p_isSurfaceSpheres)
        {
            surfaceObjsParent = p_parent.transform;
        }
        else
        {
            outerObjsParent = p_parent.transform;
        }

        p_parent.transform.localPosition = Vector3.zero;
        p_parent.transform.parent = transform;
        int index = 0;

        foreach (var vertex in vertices)
        {
            var vertexObj = Instantiate(m_spherePrefab, vertex, Quaternion.identity, p_parent.transform);
            vertexObj.name = $"{transform.name} Point {vertexObj.transform.GetSiblingIndex()}";

            LineFollow spawnedSphereLineBehaviour = vertexObj.GetComponent<LineFollow>();
            VertexDetection vDetection = vertexObj.GetComponent<VertexDetection>();
            VertexPointBehaviour vBehaviour = vertexObj.GetComponent<VertexPointBehaviour>();
            vBehaviour.m_vertexPoint = this;

            if (!p_isSurfaceSpheres)
            {
                vertexObj.tag = $"Vertex";
                spawnedSphereLineBehaviour.SetVectorPoint(this);
                spawnedSphereLineBehaviour.SetVectorLineIndex(index);
            }
            else
            {
                vDetection.enabled = false;
                spawnedSphereLineBehaviour.enabled = false;
                vBehaviour.enabled = true;
            }
            index++;
        }
    }

    #endregion

    #region Getters

    public float GetDistanceMultiplier => _initialDistanceMultiplier;

    public void SetMultiplierDistance(float p_distance)
    {
        m_outwardDistanceMultiplier = p_distance;
    }

    public SpawnSetting GetSpawnChoice() => m_spawnSetting;

    public Vector3[] GetSurfacePoints()
    {
        return m_surfaceVertices;
    }

    public Vector3[] GetOutwardSurfacePoints()
    {
        return m_outsideVertices;
    }

    public Vector3[] GetAngledOutwardSurfacePoints()
    {
        List<Vector3> outwardVertices = new List<Vector3>();
        foreach (var vertexItem in outwardVertices)
        {
            float pointAngle = GetVertexOutwardAngle(vertexItem);

            if (pointAngle <= m_middleAngleRangeA && pointAngle >= m_middleAngleRangeB)
            {
                outwardVertices.Add(vertexItem);
            }
        }
        m_outsideVertices = outwardVertices.ToArray();
        return m_outsideVertices;
    }

    public Vector3 GetRandomPositionfromVector(int p_rangeStart = 0, int p_rangeEnd = 1)
    {
        return m_outsideVertices[UnityEngine.Random.Range(p_rangeStart, p_rangeEnd)];
    }

    public Vector3 GetOuterSurfacePointPosition(int p_index)
    {
        return m_outsideVertices[p_index];
    }

    //public Vector3 GetRandomPositionfromAngledSurfacePointsVector(int p_index)
    //{
    //    return m_outwardPointAngles[p_index];
    //}

    public Vector3 GetSurfaceVertex(int p_positionIndex)
    {
        return m_surfaceVertices[p_positionIndex];
    }

    public Vector3[] GetVectorOutwardLine(int p_positionIndex)
    {
        return new Vector3[] { m_surfaceVertices[p_positionIndex], m_outsideVertices[p_positionIndex] };
    }

    public GameObject GetVertexObject(int p_index)
    {
        return outerObjsParent.transform.GetChild(p_index).gameObject;
    }

    #endregion

//    #region Debug

//#if UNITY_EDITOR

//    private void OnDrawGizmos()
//    {
//        CalculateVertices();
//        ShowPointsInInspector(m_showInInspector);
//    }

//    void ShowPointsInInspector(bool p_showInInspector)
//    {
//        if (!p_showInInspector)
//            return;
//        DrawVertexGizmo(m_surfaceVertices, m_surfaceVertex.vertexColor, m_surfaceVertex.vertexSize);
//        DrawVertexGizmo(m_outsideVertices, m_outwardVertex.vertexColor, m_outwardVertex.vertexSize);
//    }

//    void DrawVertexGizmo(Vector3[] p_pointList, Color p_color, float p_size = 0.3f)
//    {
//        for (int vertexIndex = 0; vertexIndex < p_pointList.Length; vertexIndex++)
//        {
//            var meshPoint = p_pointList[vertexIndex];
//            Gizmos.DrawSphere(meshPoint, p_size);
//            Gizmos.color = p_color;
//            Handles.Label(meshPoint, $"Point {vertexIndex}");
//        }
//    }

//#endif
//    #endregion

}