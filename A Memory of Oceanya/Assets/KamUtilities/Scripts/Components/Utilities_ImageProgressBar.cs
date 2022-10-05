using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Utilities_ImageProgressBar : Utilities_ProgressBar
{
    [FormerlySerializedAs("radialImage")] public Image image;

    protected override void UpdateBar()
    {
        float range = maxValue - minValue;
        float newFill = (currentValue - minValue) / range;
        image.fillAmount = newFill;
    }
}

