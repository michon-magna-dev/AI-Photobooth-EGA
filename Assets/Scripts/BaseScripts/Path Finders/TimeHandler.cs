using UnityEngine;
using TMPro;

public class TimeHandler : MonoBehaviour
{
    public TextMeshProUGUI timerText; 
    private float timeRemaining = 60f;
    public bool isTimePaused = true;
    private void Start()
    {
        //isTimePaused = true; 
        //GameManager.Instance.OnGameStart += OnGameStart;
        //GameManager.Instance.OnGameOver += OnGameOver;
    }

    private void OnGameStart(bool obj)
    {
        isTimePaused = false;
    }

    private void OnGameOver(bool obj)
    {
        isTimePaused = true;
    }

    void Update()
    {
        if (isTimePaused)
            return;
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimerText();
        }
        else
        {
            timeRemaining = 0;
            //GameManager.Instance.TimeOut();
            UpdateTimerText(); // Make sure it shows 0:00
        }
    }

    void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);

        timerText.text = string.Format("{0}:{1:00}", minutes, seconds);
    }

}