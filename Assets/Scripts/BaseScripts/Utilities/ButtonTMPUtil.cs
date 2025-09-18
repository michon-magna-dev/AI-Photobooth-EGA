using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ButtonTMPUtil : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshPro;

    private void OnValidate()
    {
        if (textMeshPro == null)
        {
            try
            {
                textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
            }
            catch (System.Exception)
            {
                Debug.LogError("TextMeshProUGUI component is not assigned.");
                throw;
            }
            return;
        }
        
        textMeshPro.text = gameObject.name;
    }
}