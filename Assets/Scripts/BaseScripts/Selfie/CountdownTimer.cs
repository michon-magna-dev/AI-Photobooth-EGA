using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CountdownTimer : MonoBehaviour
{
    //[SerializeField] TextMeshProUGUI m_countdownTextUI;
    [SerializeField] TextMeshProUGUI m_countdownTextUI2;

    public bool showCountdownInside =true;
    public bool showCountdownOutside =true;

    [SerializeField] ScreenShotEditable m_captureImage;
    [SerializeField] float time;
    public float countdownDuration = 3.0f;
    public Rect debugButtonPosition = new Rect();
    bool isCountingDown = false;
    public bool showDebug = false;

    public Action OnCountdownEnded;
    private void Start()
    {
#if !UNITY_EDITOR
    showDebug = false;
#endif

        isCountingDown = false;
        // Start the countdown coroutine when the scene starts.
        //StartCoroutine(CountdownCoroutine());
    }

    private IEnumerator CountdownCoroutine()
    {
        isCountingDown = true;
        float currentTimeIndex = countdownDuration;

        //m_countdownTextUI.gameObject.SetActive(true);
        m_countdownTextUI2.gameObject.SetActive(true);
        while (currentTimeIndex > 0)
        {
            // Display "3" for one second.
            //m_countdownTextUI.text = currentTimeIndex.ToString();
            m_countdownTextUI2.text = currentTimeIndex.ToString();
            yield return new WaitForSeconds(1.0f);
            currentTimeIndex--;

        }
        //m_countdownTextUI.text = "Smile";
        m_countdownTextUI2.text = "Smile";

        yield return new WaitForSeconds(1.0f);
        OnCountdownEnded?.Invoke();
        //m_captureImage.OnClickScreenCaptureButton();
        m_captureImage.CaptureScreen();
        // You can add additional logic or actions here after the countdown completes.

        // Hide the text or perform some other action.
        //m_countdownTextUI.gameObject.SetActive(false);
        m_countdownTextUI2.gameObject.SetActive(false);
        isCountingDown = false;
    }

    public void StartCountDown()
    {
        if (!isCountingDown)
        {
            StartCoroutine(CountdownCoroutine());
        }
    }

    private void OnGUI()
    {
        if (!showDebug)
        {
            return;
        }
        if (GUI.Button(debugButtonPosition, "Start Countdown"))
        {
            StartCountDown();
        }
    }
}
