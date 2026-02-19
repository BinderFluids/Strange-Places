using UnityEngine;

public class Neutralize : BoardConflictResolver
{
    public Neutralize(BoardNode otherNode, Vector2Int direction, int charge = 0) : base(otherNode, direction, charge) { }

    public override void Execute(BoardNode active, Grid<BoardNode> ctx)
    {
        bool doTranslate = charge == 0 || charge >= active.Piece.Charge;
        if (charge == 0) charge = active.Piece.Charge;
        
        int smallerCharge = Mathf.Min(charge, otherNode.Piece.Charge);
        bool similarCharges = charge == otherNode.Piece.Charge;
        
        
        Debug.Log($"Similar charges: {similarCharges}. Translate charge: {doTranslate}.");
        
        var takeActivePiece = new TakePiece(smallerCharge);
        Chain(takeActivePiece, active, ctx);
        
        var takeOtherPiece = new TakePiece(smallerCharge);
        Chain(takeOtherPiece, otherNode, ctx);

        if (!otherNode.IsOccupied() && !similarCharges)
        {
            if (doTranslate)
            {
                var translateOtherCharge = new TranslatePiece(direction);
                Chain(translateOtherCharge, active, ctx);
            }
            else
            {
                var givePiece = new GivePiece(takeActivePiece.TakenPiece); 
                Chain(givePiece, otherNode, ctx);
            }
        }
    }
}