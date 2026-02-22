using System;
using UnityEngine;
using UnityEngine.Events;

public abstract class BoardItem : MonoBehaviour, IBoardItem, ISelectable
{
    [SerializeField] private AudioClip useSound;
    [SerializeField] UnityEvent onUseUnityEvent = new();
    
    public abstract void Use();
    
    public event Action OnSelected;

    public void Select()
    {
        onUseUnityEvent?.Invoke();
        OnSelected?.Invoke();
        Use();
    }
}