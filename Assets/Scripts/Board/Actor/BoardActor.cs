
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class BoardActor : MonoBehaviour, IPieceOwner
{
    [SerializeField, Range(-5, 5)] private int reach;
    public int Reach => reach;
    
    [SerializeField] protected int actionsPerTurn = 2;
    [SerializeField] protected int actionsAvailable = 2;
    
    [SerializeField] private List<BoardItem> items = new();
    public List<BoardItem> Items => items;

    protected Grid<BoardNode> workingGrid;
    [SerializeField] protected bool turnActive;
    public event Action onTurnEnd = delegate {};
    
    public void StartTurn(BoardNodeGrid ctx, bool clone = false)
    {
        workingGrid = clone ? ctx.Copy() : ctx; 
        actionsAvailable = actionsPerTurn;
        OnStartTurn();
    }

    protected virtual void OnStartTurn() { }


    public void DoAction(Vector2Int coords, IGridAction<BoardNode> action)
    {
        if (actionsAvailable < 1) return; 
        
        workingGrid.Execute(coords, action);
        actionsAvailable--;
    }
    public void Undo()
    {
        if (actionsAvailable == actionsPerTurn) return; 
        
        workingGrid.Undo();
        actionsAvailable++;
    }

    public void EndTurn()
    {
        turnActive = false;
        OnEndTurn();
        onTurnEnd?.Invoke();
    }
    protected virtual void OnEndTurn() { }
}