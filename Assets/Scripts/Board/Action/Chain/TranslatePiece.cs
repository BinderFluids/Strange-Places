using UnityEngine;

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
                resolver = BoardConflictResolver.Create(activePiece.ResolverType, targetNode, direction, charge);
            else if (targetNode.Piece.ResolverType != ResolverType.None)
                resolver = BoardConflictResolver.Create(targetNode.Piece.ResolverType, targetNode, direction, charge);
            else
                resolver = BoardConflictResolver.Create<T>(targetNode, direction, charge);

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