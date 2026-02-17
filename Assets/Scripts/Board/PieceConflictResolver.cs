using UnityEngine;

public abstract class PieceConflictResolver
{
    public abstract void ResolveConflict(BoardNode active, BoardNode other, Vector2Int direction, Grid<BoardNode> context);
}

public class PushOtherPiece : PieceConflictResolver
{
    public override void ResolveConflict(BoardNode active, BoardNode other, Vector2Int direction, Grid<BoardNode> context)
    {
        if (active.Piece.Charge == other.Piece.Charge) return;
        if (active.Piece.Charge > other.Piece.Charge)
        {
            other.TranslatePiece(direction, new NeutralizeCharges());
            active.TranslatePiece(direction, this); 
        }
    }
}

public class NeutralizeCharges : PieceConflictResolver
{ 
    public override void ResolveConflict(BoardNode active, BoardNode other, Vector2Int direction, Grid<BoardNode> context)
    {
        if (active.Piece.Charge == other.Piece.Charge)
        {
            active.TakePiece(); 
            other.TakePiece();
            return; 
        }
        
        if (active.Piece.Charge > other.Piece.Charge)
            DoNeutralizeCharges(active, other, direction);
        if (active.Piece.Charge < other.Piece.Charge)
            DoNeutralizeCharges(other, active, direction);
    }

    void DoNeutralizeCharges(BoardNode bigCharge, BoardNode smallCharge, Vector2Int direction)
    {
        smallCharge.TakePiece();
        bigCharge.Piece.ChangeCharge(-smallCharge.Piece.Charge);
        bigCharge.TranslatePiece(direction, new PushOtherPiece());
    }
}