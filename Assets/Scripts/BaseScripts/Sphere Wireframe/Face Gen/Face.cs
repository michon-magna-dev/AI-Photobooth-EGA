using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Face
{
    public string name;
    public GameObject point1;
    public GameObject point2;
    public GameObject point3;
}

public class FaceEqualityComparer : IEqualityComparer<Face>
{
    public bool Equals(Face x, Face y)
    {
        // Check if the points are the same regardless of order
        return (
            (x.point1 == y.point1 || x.point1 == y.point2 || x.point1 == y.point3) &&
            (x.point2 == y.point1 || x.point2 == y.point2 || x.point2 == y.point3) &&
            (x.point3 == y.point1 || x.point3 == y.point2 || x.point3 == y.point3)
        );
    }

    public int GetHashCode(Face obj)
    {
        // Calculate a hash code based on the points' values (order-independent)
        return obj.point1.GetHashCode() ^ obj.point2.GetHashCode() ^ obj.point3.GetHashCode();
    }
}