using System.Collections.Generic;
using UnityEngine;

public abstract class BoardActionChain : IGridAction<BoardNode>
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
    public void Undo()
    {
        while (actions.Count > 0) actions.Pop().Undo();
    }
}


