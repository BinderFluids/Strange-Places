using UnityEngine;

public class TryPush : BoardConflictResolver
{
    public TryPush(BoardNode otherNode, Vector2Int direction, int charge) : base(otherNode, direction, charge) { }

    public override void Execute(BoardNode active, Grid<BoardNode> ctx)
    {
        if (active.Piece.Charge > otherNode.Piece.Charge)
        {
            var moveOther = new TranslatePiece(direction);
            var moveActive = new TranslatePiece(direction);
            
            Chain(moveOther, otherNode, ctx); 
            Chain(moveActive, active, ctx);
        }
    }
}