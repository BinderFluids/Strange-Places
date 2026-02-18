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

public class TranslatePiece<T> : BoardActionChain where T : BoardConflictResolver 
{
    private BoardNode targetNode;
    private Vector2Int direction;
    private int charge;

    public TranslatePiece(Vector2Int direction, int charge = 0)
    {
        this.direction = direction;
        this.charge = charge; 
    }

    public override void Execute(BoardNode active, Grid<BoardNode> ctx)
    {
        if (active == null) return; 
        if (!active.IsOccupied()) return;

        BoardPiece activePiece = new BoardPiece(active.Piece); 
        targetNode = ctx.Get(active.Coords + direction);

        if (targetNode == null)
        {
            TakePiece removePiece = new TakePiece(charge);
            Chain(removePiece, active, ctx);
            return;
        }
        if (targetNode.IsOccupied() && active.Piece.PlayerOwner != targetNode.Piece.PlayerOwner)
        {
            BoardConflictResolver resolver; 
            if (active.Piece.ResolverType != ResolverType.None)
                resolver = BoardConflictResolver.Create(activePiece.ResolverType, targetNode, direction);
            else
                resolver = BoardConflictResolver.Create<T>(targetNode, direction);

            Chain(resolver, active, ctx);
        }
        else
        {
            //remove charge from active piece
            //give charge to target piece
            var takePiece = new TakePiece(charge);
            Chain(takePiece, active, ctx); 
            
            var givePiece = new GivePiece(new BoardPiece(takePiece.TakenPiece)); 
            Chain(givePiece, targetNode, ctx);
            
            ctx.SetDirty(targetNode); 
        }
    }
}


//Default Translate Piece
public class TranslatePiece : TranslatePiece<TryPush>
{
    public TranslatePiece(Vector2Int direction, int charge = 0) : base(direction, charge) { }
}

public class ShiftBoard : BoardActionChain
{
    private Vector2Int direction;
    private BoardPlayer targetOwner;
    
    public ShiftBoard(Vector2Int direction, BoardPlayer targetOwner)
    {
        this.direction = direction;
        this.targetOwner = targetOwner; 
    }

    public override void Execute(BoardNode active, Grid<BoardNode> ctx)
    {
        ctx.ForEach(node =>
        {
            if (node == null) return;
            if (!node.IsOccupied()) return;
            if (node.Piece.PlayerOwner != targetOwner) return;
            
            Debug.Log("Shifted");
            
            TranslatePiece translation = new TranslatePiece(direction);
            Chain(translation, node, ctx);
        });
    }
}