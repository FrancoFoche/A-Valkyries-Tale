using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Utilities_SliderLinearProgressBar : Utilities_ProgressBar
{
    public Slider slider;

    protected override void UpdateBar()
    {
        slider.minValue = minValue;
        slider.maxValue = maxValue;
        slider.value = currentValue;
    }
}

