
using System;
using PrimeTween;
using ScriptableVariables;
using UnityEngine;
using UnityEngine.UI;

public class ProgressIndicator : MonoBehaviour
{
    [SerializeField] private IntVariable points;
    [SerializeField] private Image image;
    private float currentProgress;
    
    
    private void Awake()
    {
        image.fillAmount = 0.5f; 
        points.OnValueChanged += UpdateProgress;
    }

    void UpdateProgress(int value)
    {
        float targetProgress = Mathf.InverseLerp(-5, 5, value);
        Tween.UIFillAmount(image, targetProgress, .25f); 
    }

    private void OnDestroy()
    {
        points.OnValueChanged -= UpdateProgress;
    }
}