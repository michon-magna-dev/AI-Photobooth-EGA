using System;
using UnityEngine;

[Serializable]
public class Line 
{
    public string name;
    public Vector3 p1;
    public Vector3 p2;

    public Line(Vector3 startPoint, Vector3 endPoint)
    {
        this.p1 = startPoint;
        p2 = endPoint;
    }

    public static bool CompareLine(Line p_line1, Line p_line2)
    {
        bool p1p2 = p_line1.p1 == p_line2.p1 && p_line1.p2 == p_line2.p2;
        bool p2p1 = p_line2.p1 == p_line1.p2 && p_line2.p2 == p_line1.p1;

        bool IsTheSameLine = p1p2 || p2p1;
        return IsTheSameLine;
    }

}