using UnityEngine;

public class Neutralize : BoardConflictResolver
{
    public Neutralize(BoardNode otherNode, Vector2Int direction, int charge = 0) : base(otherNode, direction, charge) { }

    public override void Execute(Vector2Int activeCoords, Grid<BoardNode> ctx)
    {
        if (!ctx.TryGet(activeCoords, out var activeNode)) return; 
        
        Vector2Int otherCoords = activeCoords + direction;
        
        bool doTranslate = charge == 0 || charge >= activeNode.Piece.Charge;
        if (charge == 0) charge = activeNode.Piece.Charge;
        
        int smallerCharge = Mathf.Min(charge, otherNode.Piece.Charge);
        bool similarCharges = charge == otherNode.Piece.Charge;
        
        
        Debug.Log($"Similar charges: {similarCharges}. Translate charge: {doTranslate}.");
        
        var takeActivePiece = new TakePiece(smallerCharge);
        Chain(takeActivePiece, activeCoords, ctx);
        
        var takeOtherPiece = new TakePiece(smallerCharge);
        Chain(takeOtherPiece, otherCoords, ctx);

        if (!otherNode.IsOccupied() && !similarCharges)
        {
            if (doTranslate)
            {
                var translateOtherCharge = new TranslatePiece(direction);
                Chain(translateOtherCharge, activeCoords, ctx);
            }
            else
            {
                var givePiece = new GivePiece(takeActivePiece.TakenPiece); 
                Chain(givePiece, otherCoords, ctx);
            }
        }
    }
}