using UnityEngine;

public class TryPush : BoardConflictResolver
{
    public TryPush(BoardNode otherNode, Vector2Int direction, int charge) : base(otherNode, direction, charge) { }

    public override void Execute(Vector2Int activeCoords, Grid<BoardNode> ctx)
    {
        if (!ctx.TryGet(activeCoords, out var activeNode)) return;
        Vector2Int otherCoords = activeCoords + direction;
        
        if (activeNode.Piece.Charge > otherNode.Piece.Charge)
        {
            var moveOther = new TranslatePiece(direction);
            var moveActive = new TranslatePiece(direction);
            
            Chain(moveOther, otherCoords, ctx); 
            Chain(moveActive, activeCoords, ctx);
        }
    }
}