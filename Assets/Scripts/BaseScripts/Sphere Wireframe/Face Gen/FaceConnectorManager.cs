using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class FaceConnectorManager : MonoBehaviour
{
    [SerializeField] VertexPoints m_vertexPoint;
    [Header("Basics")]
    [SerializeField] List<Line> m_linesList;
    [SerializeField] List<Face> m_faceList;
    [SerializeField] int totalFaceAvailable;
    [Header("Parmaeters")]
    [SerializeField] SpawnSetting vertexSource = SpawnSetting.OUTER;
    [Header("Debug")]
    public bool showGUI = true;
    public float lineThickness = 1f;

    private void Start()
    {
        m_vertexPoint = GetComponent<VertexPoints>();
        //GetLinesFromVertices();
        GetLinesFromVertices();

    }

    void GetLinesFromVertices()
    {
        Vector3[] points = vertexSource.Equals(SpawnSetting.SURFACE) ? m_vertexPoint.GetSurfacePoints() : m_vertexPoint.GetOutwardSurfacePoints();

        for (int point1Index = 0; point1Index < points.Length; point1Index++)
        {
            for (int point2Index = point1Index + 1; point2Index < points.Length; point2Index++)
            {
                Vector3 point1 = points[point1Index];
                Vector3 point2 = points[point2Index];
                Line line = new Line(point1, point2);
                line.name = $"Line p{point1Index} p{point2Index}";
                if (!IsAlreadyInList(line))
                {
                    m_linesList.Add(line);
                }
                Debug.Log($"Pair: ({point1.x}, {point1.y}, {point1.z}) - ({point2.x}, {point2.y}, {point2.z})");
            }
        }
    }

    void GetFacesFromLines()
    {
        var lines = m_linesList;

    }

    private bool IsAlreadyInList(Line line)
    {
        foreach (var existingLine in m_linesList)
        {
            bool IsLineTheSame = Line.CompareLine(existingLine, line);
            if (IsLineTheSame)
            {
                return true;
            }
        }
        return false;
    }

//#if UNITY_EDITOR

//    private void OnDrawGizmosSelected()
//    {
//        if (!showGUI)
//            return;
//        foreach (var line in m_linesList)
//        {
//            var labelPosition = Vector3.Lerp(line.p1, line.p2, 0.5f);
//            Handles.Label(labelPosition, $"{line.name}");
//            Handles.DrawLine(line.p1, line.p2, lineThickness);
//        }
//    }

//#endif
}