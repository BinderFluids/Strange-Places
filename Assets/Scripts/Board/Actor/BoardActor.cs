
using System;
using System.Collections.Generic;
using ScriptableVariables;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class BoardActor : MonoBehaviour, IPieceOwner
{
    [SerializeField] protected int actionsPerTurn = 2;
    protected int actionsAvailable = 2;
    
    [SerializeField] private List<BoardItem> items = new();
    public List<BoardItem> Items => items;

    protected Grid<BoardNode> workingGrid;
    public event Action onTurnEnd = delegate {};
    
    public void StartTurn(Grid<BoardNode> ctx)
    {
        workingGrid = ctx;
        actionsAvailable = actionsPerTurn;
        OnStartTurn();
    }

    protected virtual void OnStartTurn() { }


    public void UseAction(Vector2Int coords, IGridAction<BoardNode> action)
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
        OnEndTurn();
        onTurnEnd?.Invoke();
    }
    protected virtual void OnEndTurn() { }
}