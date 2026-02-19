using System.Collections.Generic;
using UnityEngine;

public abstract class BoardActionChain : IGridAction<BoardNode>
{
    private Stack<IGridAction<BoardNode>> actions = new();

    protected void Chain(IGridAction<BoardNode> action, BoardNode activeNode, Grid<BoardNode> ctx)
    {
        actions.Push(action);
        ctx.ExecuteGridAction(activeNode, action); 
    }

    public abstract void Execute(BoardNode active, Grid<BoardNode> ctx);
    public void Undo()
    {
        while (actions.Count > 0) actions.Pop().Undo();
    }
}


//Default Translate Piece
public class TranslatePiece : TranslatePiece<TryPush>
{
    public TranslatePiece(Vector2Int direction, int charge = 0) : base(direction, charge) { }
}