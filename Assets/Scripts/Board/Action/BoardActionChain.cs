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
        
        BoardPiece activePiece = active.Piece;
        targetNode = ctx.Get(active.Coords + direction);
        
        if (targetNode == null) return;
        
        if (targetNode.IsOccupied())
        {
            //pass control to conflict resolver
            if (active.Piece.PlayerOwner == targetNode.Piece.PlayerOwner) return;

            BoardConflictResolver resolver; 
            if (active.Piece.ResolverType != ResolverType.None)
                resolver = BoardConflictResolver.Create(activePiece.ResolverType, targetNode, direction);
            else
                resolver = BoardConflictResolver.Create<T>(targetNode, direction);

            Chain(resolver, active, ctx);
            return; 
        }
        
        
        var takePiece = new TakePiece(charge); 
        var givePiece = new GivePiece(active.Piece); 
        
        Chain(takePiece, active, ctx); 
        Chain(givePiece, targetNode, ctx);
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
        var nodesToShift = new List<BoardNode>();
        
        ctx.ForEach(node =>
        {
            if (!node.IsOccupied()) return;
            if (node.Piece.PlayerOwner != targetOwner) return;
            nodesToShift.Add(node);
        });

        // Move "farthest in the direction" first to avoid double-moves and reduce conflicts.
        nodesToShift.Sort((a, b) =>
        {
            var ac = a.Coords;
            var bc = b.Coords;

            if (direction == Vector2Int.up) return bc.y.CompareTo(ac.y);
            if (direction == Vector2Int.down) return ac.y.CompareTo(bc.y);
            if (direction == Vector2Int.right) return bc.x.CompareTo(ac.x);
            if (direction == Vector2Int.left) return ac.x.CompareTo(bc.x);

            return 0;
        });

        foreach (var node in nodesToShift)
        {
            var translate = new TranslatePiece(direction);
            Chain(translate, node, ctx);
        }
    }
}