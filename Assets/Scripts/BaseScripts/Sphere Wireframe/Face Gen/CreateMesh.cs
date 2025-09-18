using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateMesh : MonoBehaviour
{
    public VertexPoints vertexPoints; // Your list of vertices
    public Vector3[] vertices; // Your list of vertices
    public int triangleCount = 0;

    //void Start()
    //{
    //    vertices = vertexPoints.GetSpawnChoice() == (SpawnSetting.SURFACE) ? vertexPoints.GetSurfacePoints() : vertexPoints.GetOutwardSurfacePoints();
    //    if (vertices.Length >= 3)
    //    {
    //        // Create a new GameObject for each plane mesh
    //        for (int i = 0; i < vertices.Length - 2; i += 3)
    //        {
    //            CreatePlane(vertices[i], vertices[i + 1], vertices[i + 2]);
    //        }
    //    }
    //    else
    //    {
    //        Debug.LogError("You need at least 3 vertices to create a plane.");
    //    }
    //}
    //private int triangleCount = 0;

    void Start()
    {
        // Ensure you have at least 3 vertices to create a plane
        //vertices = vertexPoints.GetSpawnChoice() == (SpawnSetting.SURFACE) ? vertexPoints.GetSurfacePoints() : vertexPoints.GetOutwardSurfacePoints();
        //if (vertices.Length >= 3)
        //{
        //    // Create a new GameObject for each plane mesh
        //    //for (int i = 0; i < vertices.Length - 2; i += 3)
        //    //{
        //    //    CreatePlane(vertices[i], vertices[i + 1], vertices[i + 2]);
        //    //}

        //    // Create a new GameObject for each plane mesh
        //    for (int i = 0; i < vertices.Length; i++)
        //    {
        //        for (int j = i + 1; j < vertices.Length - 2; j++)
        //        {
        //            CreatePlane(vertices[i], vertices[i + 1], vertices[i + 2]);
        //        }
        //    }
        //}
        //else
        //{
        //    Debug.LogError("You need at least 3 vertices to create a plane.");
        //}
    }

    public static void CreatePlane(Vector3 v1, Vector3 v2, Vector3 v3, int triangleCount)
    {
        // Calculate the normal vector of the plane
        Vector3 normal = Vector3.Cross(v2 - v1, v3 - v1).normalized;

        // Create a new GameObject for the plane
        GameObject planeObject = new GameObject("Plane " + triangleCount);
        MeshFilter meshFilter = planeObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = planeObject.AddComponent<MeshRenderer>();
        Mesh mesh = new Mesh();

        // Define vertices
        Vector3[] planeVertices = new Vector3[] { v1, v2, v3 };
        mesh.vertices = planeVertices;

        // Define triangles
        int[] triangles = new int[] { 0, 1, 2 };
        mesh.triangles = triangles;

        meshFilter.mesh = mesh;

        // Set the normal vector to ensure proper rendering
        meshFilter.mesh.normals = new Vector3[] { normal, normal, normal };
    }

}