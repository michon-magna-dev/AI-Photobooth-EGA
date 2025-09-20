using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageSelectionPromptHandler : MonoBehaviour
{
    public Transform selectionTransform;
    public Transform[] selectPositions = new Transform[3];

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            MoveSelection(0);
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            MoveSelection(1);
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            MoveSelection(2);
        }
    }

    public void MoveSelection(int index)
    {
        if (index < 0 || index >= selectPositions.Length) return;
        selectionTransform.position = selectPositions[index].position;
    }
}
