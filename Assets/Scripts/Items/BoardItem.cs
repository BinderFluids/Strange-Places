using System;
using UnityEngine;

public abstract class BoardItem : MonoBehaviour, IBoardItem, ISelectable
{
    public abstract void Use(BoardNodeGrid ctx);
    
    public event Action OnSelected;

    public virtual void Select() { }
}