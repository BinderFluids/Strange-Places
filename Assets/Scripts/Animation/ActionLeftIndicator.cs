using System;
using ScriptableVariables;
using TMPro;
using UnityEngine;

public class ActionLeftIndicator : MonoBehaviour
{
    [SerializeField] private IntVariable actionsLeft;
    [SerializeField] private TMP_Text text;

    private void Awake() => actionsLeft.OnValueChanged += OnValueChanged; 
    
    void OnValueChanged(int value) => text.text = value.ToString();
    
    private void OnDestroy() => actionsLeft.OnValueChanged -= OnValueChanged;
}
