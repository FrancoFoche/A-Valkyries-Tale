using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Utilities_RadialProgressBar : MonoBehaviour
{
    public Image radialImage;

    [SerializeField] private float maxValue = 1;
    [SerializeField] private float minValue = 0;
    [SerializeField] private float currentValue = 1;
    
    [SerializeField] private float animSpeed = 4f;

    [SerializeField] private UnityEvent onFull = null;
    [SerializeField] private UnityEvent onValueChanged = null;
    [SerializeField] private UnityEvent onEmpty = null;

    public float MaxValue { get { return maxValue; } }
    public float MinValue { get { return minValue; } }

    private float targetValue;
    private bool animating;
    private float lastSavedValue;
    private System.Action onFinished;

    public void Update()
    {
        if (animating)
        {
            currentValue = Mathf.Lerp(currentValue, targetValue, animSpeed * Time.deltaTime);
            UpdateBar();

            bool endCondition = currentValue >= targetValue - 0.1f;

            if (currentValue > targetValue)
            {
                endCondition = currentValue <= targetValue + 0.1f;
            }

            if (endCondition)
            {
                currentValue = targetValue;
                animating = false;
                CheckValue();
                onFinished?.Invoke();
            }
        }
    }

    public void SetRange(float minValue, float maxValue)
    {
        this.minValue = minValue;
        this.maxValue = maxValue;
        currentValue = minValue;
        UpdateBar();
    }

    public void SetValue(float newValue, System.Action onFinished = null)
    {
        if (newValue != lastSavedValue)
        {
            lastSavedValue = newValue;
            animating = true;
            targetValue = newValue;
            this.onFinished = onFinished;
            onValueChanged?.Invoke();
        }

        if(newValue == maxValue)
        {
            onFull?.Invoke();
        }

        if (newValue == minValue)
        {
            onEmpty?.Invoke();
        }

        UpdateBar();
    }
    void CheckValue()
    {
        if (currentValue > maxValue || currentValue < minValue)
        {
            if (currentValue > maxValue)
            {
                currentValue = maxValue;
            }
            else if (currentValue < minValue)
            {
                currentValue = minValue;
            }
        }
    }
    public void UpdateBar()
    {
        float range = maxValue - minValue;
        float newFill = (currentValue - minValue) / range;
        radialImage.fillAmount = newFill;
    }
}
