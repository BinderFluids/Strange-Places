using System;
using UnityEngine;

public abstract class BoardItem : MonoBehaviour, IBoardItem, ISelectable
{
    public abstract void Use();
    
    public event Action OnSelected;

    public void Select()
    {
        Use();
    }
}