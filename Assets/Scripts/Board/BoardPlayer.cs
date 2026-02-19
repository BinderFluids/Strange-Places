
using System;
using System.Collections.Generic;
using EventBus;
using Unity.VisualScripting;
using UnityEngine;

public class BoardPlayer : MonoBehaviour, IPieceOwner
{
    [SerializeField, Range(-5, 5)] private int reach;
    public int Reach => reach;
    
    [SerializeField] private int actionsPerTurn = 2;
    [SerializeField] private int actionsAvailable = 2;

    [SerializeField] private List<IBoardItem> items = new();
    public List<IBoardItem> Items => items;

    private Grid<BoardNode> workingGrid;

    private bool turnActive;
    public event Action onTurnEnd = delegate {};

    public void StartTurn(Grid<BoardNode> ctx, bool clone = false)
    {
        Debug.Log("starting turn");
        
        workingGrid = clone ? ctx.Copy() : ctx; 
        actionsAvailable = actionsPerTurn;
        turnActive = true; 
    }

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
        onTurnEnd?.Invoke();
    }

    private void Update()
    {
        if (!turnActive) return;
        
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (actionsAvailable > 0) return;
            EndTurn();
        }

        if (Input.GetKeyDown(KeyCode.Z)) Undo();
    }
}