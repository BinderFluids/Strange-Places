using System.Collections.Generic;
using UnityEngine;

public abstract class BoardActionChain : BoardAction
{
    private Stack<IGridAction<BoardNode>> actions = new();

    protected void Chain(IGridAction<BoardNode> action, Vector2Int activeCoords, Grid<BoardNode> ctx)
    {
        actions.Push(action);
        
        ctx.StopObservingAction();
        ctx.Execute(activeCoords, action);
        ctx.StartObservingAction();
    }

    public abstract void Execute(Vector2Int activeCoords, Grid<BoardNode> ctx);
    protected override void OnUndo()
    {
        while (actions.Count > 0) actions.Pop().Undo();
    }
}


