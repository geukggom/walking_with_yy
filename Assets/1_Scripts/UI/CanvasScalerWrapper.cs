using System;
using UnityEngine;
using UnityEngine.UI;

public class CanvasScalerWrapper : CanvasScaler
{
    private const float FixedAspectRatio = 9f / 16f;
    private float _currentAspectRatio = 0f;

    private void Update()
    {
        var currentAspectRatio = Screen.width / (float)Screen.height;
        if (Math.Abs(_currentAspectRatio - currentAspectRatio) < Mathf.Epsilon) return;

        _currentAspectRatio = currentAspectRatio;
        matchWidthOrHeight = currentAspectRatio > FixedAspectRatio ? 0 : 1;
    }
}