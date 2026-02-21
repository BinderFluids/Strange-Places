using System.Collections.Generic;
using UnityEngine;

public abstract class BoardAction : IGridAction<BoardNode>
{
    public static bool doDebug; 
    private Vector2Int activeCoords; 
    public Vector2Int ActiveCoords => activeCoords;
    private Stack<ISecondaryAction> secondaryActions = new();

    public abstract void Execute(Vector2Int activeCoords, Grid<BoardNode> ctx);

    public void AddSecondaryAction(ISecondaryAction action) => secondaryActions.Push(action);
    
    public void Undo()
    {
        OnUndo();
        while (secondaryActions.Count > 0)
            secondaryActions.Pop().Undo();
    }

    protected void DoDebug(string message)
    {
        if 
            (doDebug)Debug.Log($"[{GetType().Name}] {message}");
    }
    protected abstract void OnUndo(); 
}

public interface ISecondaryAction
{
    public abstract void Execute(); 
    public abstract void Undo(); 
}