using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineFollow : MonoBehaviour
{
    [SerializeField] LineRenderer m_lineRenderer;
    VertexPoints m_vectorPoint;
    int objIndex = 0;

    private void Start()
    {
        m_lineRenderer = GetComponent<LineRenderer>();
    }

    private void FixedUpdate()
    {
        FollowLineVector();
    }

    void FollowLineVector()
    {
        var vectorLine = new Vector3[] { transform.position, m_vectorPoint.GetSurfaceVertex(objIndex) };
        m_lineRenderer.SetPositions(vectorLine);
    }

    #region Init
    public void SetVectorLineIndex(int p_index)
    {
        //if (p_index > 12)
        //{
        //    Debug.Log($"VectorLine");
        //}
        objIndex = p_index;
    }

    public void SetVectorPoint(VertexPoints p_vectorPoint)
    {
        m_vectorPoint = p_vectorPoint;
    }

    #endregion
}