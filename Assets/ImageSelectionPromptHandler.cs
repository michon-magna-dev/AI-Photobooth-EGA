using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageSelectionPromptHandler : MonoBehaviour
{
    public Transform selectionTransform;
    public Transform[] selectPositions = new Transform[3];
    public int selectedIndex = 0;   
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

    public void MoveSelection(int p_index)
    {
        if (p_index < 0 || p_index >= selectPositions.Length) return;
        selectionTransform.position = selectPositions[p_index].position;
        selectedIndex = p_index;
    }
}
