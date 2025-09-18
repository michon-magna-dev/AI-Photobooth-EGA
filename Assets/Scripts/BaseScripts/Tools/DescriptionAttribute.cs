using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DescriptionAttribute : PropertyAttribute
{
    public string description;

    public DescriptionAttribute(string description)
    {
        this.description = description;
    }
}