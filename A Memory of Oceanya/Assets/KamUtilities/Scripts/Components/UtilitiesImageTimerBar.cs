using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UtilitiesImageTimerBar : Utilities_ImageProgressBar
{
    [Header("TIMER")]
    public TextMeshProUGUI timerText;
    
    private float timer;
    private float currentTime;
    private bool started;

    private void Update()
    {
        if (started)
        {
            currentTime += Time.deltaTime;
            if (currentTime >= timer)
            {
                currentTime = timer;
                started = false;
                onFull?.Invoke();
            }

            if (timerText != null)
            {
                timerText.text = string.Format("{0:0.00}", timer - currentTime);
            }
            
            SetValueInstant(currentTime);
        }
    }

    public void Reset()
    {
        SetValueInstant(MaxValue);
    }
    public void StartTimer(float time)
    {
        timer = time;
        SetRange(0, timer);
        currentTime = 0f;
        started = true;
        SetValueInstant(currentTime);
    }
}