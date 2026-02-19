
using System;
using System.Collections.Generic;
using EventBus;
using UnityEngine;

public class BoardPlayer : MonoBehaviour
{
    [SerializeField, Range(-5, 5)] private int reach;
    public int Reach => reach;
    
    [SerializeField] private int actionsPerTurn = 2;
    [SerializeField] private int actionsAvailable = 2;
        
    private Stack<IGridAction<BoardNode>> actions = new();

    private bool turnActive;
    public event Action onTurnEnd = delegate {};
    
    public void StartTurn()
    {
        actionsAvailable = actionsPerTurn;
        turnActive = true; 
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

    public void DoAction(IGridAction<BoardNode> action, BoardNode node)
    {
        if (actionsAvailable == 0) return;
        
        Board.Instance.DoAction(action, node);
        actions.Push(action);
        actionsAvailable--;  
    }

    public void Undo()
    {
        if (actions.Count < 1) return;
        actionsAvailable++; 
        Board.Instance.Undo(actions.Pop());
    }
}