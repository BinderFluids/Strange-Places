using UnityEngine;

public class ShiftBoard : BoardActionChain
{
    private Vector2Int direction;
    private BoardPlayer targetOwner;
    
    public ShiftBoard(Vector2Int direction, BoardPlayer targetOwner)
    {
        this.direction = direction;
        this.targetOwner = targetOwner; 
    }

    public override void Execute(Vector2Int activeCoords, Grid<BoardNode> ctx)
    {
        ctx.ForEach(node =>
        {
            if (node == null) return;
            if (!node.IsOccupied()) return;
            if (node.Piece.PlayerOwner != targetOwner) return;
            
            TranslatePiece translation = new TranslatePiece(direction);
            Chain(translation, node.Coords, ctx);
        });
    }
}