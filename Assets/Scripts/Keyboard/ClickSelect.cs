using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickSelect : MonoBehaviour
//, IPointerClickHandler
{
  //  public SessionButton button;
    //public void OnPointerClick(PointerEventData eventData)
    //{
    //    //  button.Select();
    //    Debug.Log("SELECT");
    //}

    //VirtualKeyboard vk = new VirtualKeyboard();

    public KeyboardScript keyboard_script;

    public void OpenKeyboard(InputField inputField)
    {
        {
            keyboard_script.gameObject.SetActive(true);
            keyboard_script.TextField = inputField;
            //vk.ShowTouchKeyboard();
           // vk.ShowOnScreenKeyboard();
        }
    }

    public void CloseKeyboard()
    {
        {
            keyboard_script.gameObject.SetActive(false);
            //vk.HideTouchKeyboard();
        }
    }
}