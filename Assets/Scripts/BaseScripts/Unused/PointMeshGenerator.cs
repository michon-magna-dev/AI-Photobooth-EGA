using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PointMeshGenerator : MonoBehaviour
{
    [SerializeField] VertexPoints _vectorPoint;
    [SerializeField] Material _faceMaterial;

    void Start()
    {
        GenerateFaceMeshes(_vectorPoint.GetOutwardSurfacePoints());
    }
    
    void GenerateFaceMeshes(Vector3[] p_pointList)
    {
        var spherePositions = p_pointList;
        GameObject triangleObject = new GameObject("TriangleMesh");

        // Create a MeshFilter and MeshRenderer for the triangle
        MeshFilter meshFilter = triangleObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = triangleObject.AddComponent<MeshRenderer>();

        // Create a new mesh
        Mesh mesh = new Mesh();

        // Define vertices and triangles lists to build the mesh
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        // Add the sphere positions as vertices
        vertices.AddRange(spherePositions);

        // Create triangles connecting each point to its closest neighbors
        for (int i = 0; i < spherePositions.Length; i++)
        {
            Vector3 currentPoint = spherePositions[i];

            // Find the closest point to the current point
            int closestIndex = -1;
            float closestDistance = float.MaxValue;

            for (int j = 0; j < spherePositions.Length; j++)
            {
                if (i != j)
                {
                    float distance = Vector3.Distance(currentPoint, spherePositions[j]);

                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestIndex = j;
                    }
                }
            }

            // Create a triangle connecting the current point to its closest neighbors
            if (closestIndex != -1)
            {
                triangles.Add(i);
                triangles.Add((i + 1) % spherePositions.Length);
                triangles.Add(closestIndex);
            }
        }

        // Set vertices and triangles for the mesh
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        // Calculate normals (this is just an example; you might want to compute proper normals)
        mesh.RecalculateNormals();

        // Assign the mesh to the MeshFilter
        meshFilter.mesh = mesh;

        // Set the material for the mesh (you can assign a material in the Inspector)
        meshRenderer.material = _faceMaterial;
    }
}
