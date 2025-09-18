using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class TriangleMesh : MonoBehaviour
{
    private Mesh mesh;
    private MeshFilter m_meshFilter;
    private MeshRenderer m_meshRenderer;
    public bool FollowVertices = true;
    [Header("Origin")]
    [SerializeField] VertexDetection m_triangleOrigin;

    [Header("Triangle Points")]
    [SerializeField] GameObject point1;
    [SerializeField] GameObject point2;
    [SerializeField] GameObject point3;
    [Space]
    [Header("Triangle Details")]
    [SerializeField] Material faceMaterialPrefab;
    [SerializeField] Material faceMaterial;
    [SerializeField] Color faceColor = Color.cyan;
    [Space]
    [SerializeField] float triangleArea = 0;
    [Space]
    [SerializeField] float maxAreaReached = 2;
    [SerializeField] float minAreaReached = 1;
    [Space]
    [SerializeField] float interpolationCalc = 1;
    [Space]
    [Header("Alpha Range Limit")]
    [Tooltip("Range of the Alpha where the interpolation plays around in")]
    [SerializeField] float alphaMinRange = 0.01f;
    [SerializeField] float alphaMaxRange = 0.1f;

    private bool isCoroutineRunning = false;
    public float lifetimeMax = 5;
    public float currentlifetime = 0;
    #region Lifecycles

    void Start()
    {
        //lifetimeMax = ConfigManager.GetInstance().GetValue("TRIANGLE_LIFETIME");
        triangleArea = GetTriangleArea();
        maxAreaReached = FaceGenManager.GetInstance.GetTriangleAreaLimit - 0.5f;
        minAreaReached = maxAreaReached - 0.5f;
        //interpolationCalc = 1;

        mesh = new Mesh();
        m_meshFilter = GetComponent<MeshFilter>();
        m_meshFilter.sharedMesh = mesh;

        faceMaterial = new Material(faceMaterialPrefab);
        m_meshRenderer = GetComponent<MeshRenderer>();
        m_meshRenderer.sharedMaterial = faceMaterial;
        faceMaterial = m_meshRenderer.sharedMaterial;
        UpdateTriangleMesh();
        UpdateTriangleColor();

        //if (!isCoroutineRunning)
        //    StartCoroutine(UpdateCoroutine());
    }

    //Alternative for Coroutine
    private void FixedUpdate()
    {
        //CalculateLifetime();
        triangleArea = GetTriangleArea();
        //CheckTriangleLimit();
        UpdateTriangleMesh();
        UpdateTriangleColor();
    }

    #endregion
    void CalculateLifetime()
    {
        currentlifetime += Time.deltaTime;
        if (currentlifetime >= lifetimeMax)
        {
            m_triangleOrigin.RemoveFaceFromList(this.gameObject);
        }
    }

    #region Triangle Mesh Functions
    void UpdateTriangleMesh()
    {
        if ((point1 == null || point2 == null || point3 == null))
        {
            Debug.LogError("Please assign all three GameObjects in the inspector.");
            return;
        }

        Vector3[] vertices = new Vector3[] { point1.transform.position, point2.transform.position, point3.transform.position };
        int[] triangles = new int[] { 0, 1, 2 };

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    void UpdateTriangleColor()
    {
        var inverseLerp = (triangleArea - minAreaReached) / (maxAreaReached - minAreaReached);
        interpolationCalc = 1 - inverseLerp;
        
        float interpolationAlpha = Mathf.Lerp(alphaMinRange, alphaMaxRange, interpolationCalc);
        faceColor.a = interpolationAlpha;

        m_meshRenderer.sharedMaterial.color = faceColor;
    }

    void CheckTriangleLimit()
    {
        float maxAreaLimit = FaceGenManager.GetInstance.GetTriangleAreaLimit;
        if (triangleArea >= maxAreaLimit)
        {
            m_triangleOrigin.RemoveFaceFromList(this.gameObject);
        }
    }

    float GetTriangleArea()
    {
        Vector3 AB = point2.transform.position - point1.transform.position;
        Vector3 AC = point3.transform.position - point1.transform.position;

        Vector3 crossProduct = Vector3.Cross(AB, AC);
        float area = 0.5f * crossProduct.magnitude;

        if (area > maxAreaReached)
            maxAreaReached = area;
        if (area < minAreaReached)
            minAreaReached = area;

        return area;
    }
    #endregion

    #region Public Functions

    public GameObject[] GetPoints() { return new GameObject[] { point1, point2, point3 }; }

    public TriangleMesh SetPoints(GameObject[] p_points)
    {
        point1 = p_points[0];
        point2 = p_points[1];
        point3 = p_points[2];
        return this;
    }

    public void SetTriangleVertexPointOrigin(VertexDetection p_vertexDetection)
    {
        m_triangleOrigin = p_vertexDetection;
    }

    #endregion

    #region static functions
    public static bool IsFaceEqual(TriangleMesh face1, TriangleMesh face2)
    {
        return (face1.point1 == face2.point1 &&
                face1.point2 == face2.point2 &&
                face1.point3 == face2.point3);
    }
    #endregion
}