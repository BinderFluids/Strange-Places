using UnityEngine;

public class TranslatePiece<T> : BoardActionChain where T : BoardConflictResolver
{
    private Vector2Int targetCoords; 
    private Vector2Int direction;
    private int charge;

    public TranslatePiece(Vector2Int direction, int charge = 0)
    {
        this.direction = direction;
        this.charge = charge; 
    }

    public override void Execute(Vector2Int activeCoords, Grid<BoardNode> ctx)
    {
        if (!ctx.TryGet(activeCoords, out var activeNode)) return; 
        if (!activeNode.IsOccupied()) return;

        targetCoords = activeCoords + direction;
        BoardPiece activePiece = new BoardPiece(activeNode.Piece); 
        ctx.TryGet(targetCoords, out var targetNode);

        if (targetNode == null)
        {
            TakePiece removePiece = new TakePiece(charge);
            Chain(removePiece, activeCoords, ctx);
            return;
        }
        if (targetNode.IsOccupied() && activeNode.Piece.PlayerOwner != targetNode.Piece.PlayerOwner)
        {
            BoardConflictResolver resolver; 
            if (activeNode.Piece.ResolverType != ResolverType.None)
                resolver = BoardConflictResolver.Create(activePiece.ResolverType, targetNode, direction, charge);
            else if (targetNode.Piece.ResolverType != ResolverType.None)
                resolver = BoardConflictResolver.Create(targetNode.Piece.ResolverType, targetNode, direction, charge);
            else
                resolver = BoardConflictResolver.Create<T>(targetNode, direction, charge);

            Chain(resolver, activeCoords, ctx);
        }
        else
        {
            //remove charge from active piece
            //give charge to target piece
            var takePiece = new TakePiece(charge);
            Chain(takePiece, activeCoords, ctx); 
            
            var givePiece = new GivePiece(new BoardPiece(takePiece.TakenPiece)); 
            Chain(givePiece, targetCoords, ctx);
            
            ctx.SetDirty(targetNode); 
        }
    }
}

public class TranslatePiece : TranslatePiece<TryPush>
{
    public TranslatePiece(Vector2Int direction, int charge = 0) : base(direction, charge) { }
}