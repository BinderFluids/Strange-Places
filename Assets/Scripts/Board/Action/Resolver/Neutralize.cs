using UnityEngine;

public class  Neutralize : BoardConflictResolver
{
    public Neutralize(BoardNode otherNode, Vector2Int direction, int charge = 0) : base(otherNode, direction, charge) { }

    public override void Execute(Vector2Int activeCoords, Grid<BoardNode> ctx)
    {
        if (!ctx.TryGet(activeCoords, out var activeNode)) return; 
        
        Vector2Int otherCoords = activeCoords + direction;
        
        bool doTranslate = charge == 0 || charge > activeNode.Piece.Charge;
        if (charge == 0) charge = activeNode.Piece.Charge;
        
        int smallerCharge = Mathf.Min(charge, otherNode.Piece.Charge);
        bool similarCharges = charge == otherNode.Piece.Charge;
        
        
        
        var takeActivePiece = new TakePiece(smallerCharge);
        Chain(takeActivePiece, activeCoords, ctx);
        
        var takeOtherPiece = new TakePiece(smallerCharge);
        Chain(takeOtherPiece, otherCoords, ctx);
        
        Debug.Log($"Other Occupied: {otherNode.IsOccupied()} Similar charges: {similarCharges}. Translate charge: {doTranslate}.");

        //If the other node is empty, and both charges did not have the same value
        if (!otherNode.IsOccupied() && !similarCharges)
        {
            //move active piece
            if (doTranslate)
            {
                var translateOtherCharge = new TranslatePiece(direction);
                Chain(translateOtherCharge, activeCoords, ctx);
            }
            
            //
            // else
            // {
            //     var givePiece = new GivePiece(takeActivePiece.TakenPiece); 
            //     Chain(givePiece, otherCoords, ctx);
            // }
        }
    }
}